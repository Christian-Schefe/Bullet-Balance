using Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private int maxLogLength = 700;

    private string logBuffer = "--- Log ---\n";
    private string filename = null;
    private bool doShow = true;

    private string command = "";
    private List<string> commandHistory = new();
    protected int commandHistoryIndex = 0;

    private void Awake()
    {
        Globals<DebugConsole>.RegisterOrDestroy(this);
#if UNITY_EDITOR
        doShow = false;
#endif
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            doShow = !doShow;
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        var str = $"[{type}] {logString}\n";
        if (type == LogType.Error || type == LogType.Exception)
        {
            str += $"{stackTrace}\n";
        }

        logBuffer += str;

        if (logBuffer.Length > maxLogLength)
        {
            logBuffer = logBuffer[^maxLogLength..];
        }

        if (filename == null)
        {
            var dirPath = FilePersistence.BuildPersistencePath("logs");
            System.IO.Directory.CreateDirectory(dirPath);
            var dateStr = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            filename = dirPath + "/log-" + dateStr + ".txt";
        }

        try
        {
            System.IO.File.AppendAllText(filename, str);
        }
        catch
        {
            Debug.LogError("Failed to write log to file");
        }
    }

    void OnGUI()
    {
        if (!doShow) return;
        var scale = new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        GUI.TextArea(new Rect(10, 10, 540, 370), logBuffer);
        command = GUI.TextField(new Rect(10, 400, 540, 25), command);

        //on press enter, execute command:
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
        {
            if (ExecuteCommand(command, out var failReason))
            {
                print($"Executed command: \"{command}\"");
                commandHistory.Add(command);
                command = "";
                commandHistoryIndex = 0;
            }
            else
            {
                print(failReason);
            }
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.UpArrow)
        {
            if (commandHistoryIndex < commandHistory.Count)
            {
                commandHistoryIndex++;
                command = commandHistory[^commandHistoryIndex];
            }
        }
        if (Event.current.isKey && Event.current.keyCode == KeyCode.DownArrow)
        {
            if (commandHistoryIndex > 1)
            {
                commandHistoryIndex--;
                command = commandHistory[^commandHistoryIndex];
            }
        }
    }

    private bool ExecuteCommand(string command, out string failReason)
    {
        failReason = "";

        var encasedArgs = new List<string>();
        var inQuotes = false;
        var currentArg = "";
        foreach (var c in command)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (currentArg != "")
                {
                    encasedArgs.Add(currentArg);
                    currentArg = "";
                }
            }
            else
            {
                currentArg += c;
            }
        }
        if (currentArg != "") encasedArgs.Add(currentArg);

        var commands = new CommandList(new()
        {
            new SetCommand(),
            new GiveArtifactCommand(),
            new GiveHazardCommand()
        });

        return commands.TryExecute(encasedArgs, out failReason);
    }

    public class CommandList
    {
        private readonly List<Command> commands;

        public CommandList(List<Command> commands)
        {
            this.commands = commands;
        }

        public bool TryExecute(List<string> args, out string failureReason)
        {
            foreach (var c in commands)
            {
                if (c.Matches(args))
                {
                    return c.TryExecute(args, out failureReason);
                }
            }

            failureReason = $"Unknown command: {string.Join(" ", args)}";
            return false;
        }
    }

    public abstract class Command
    {
        public List<string> opcode;

        public Command(List<string> opcode)
        {
            this.opcode = opcode;
        }

        public virtual string GetSyntax()
        {
            return string.Join(" ", opcode);
        }

        public bool Matches(List<string> args)
        {
            if (args.Count < opcode.Count) return false;
            for (int i = 0; i < opcode.Count; i++)
            {
                if (opcode[i] != args[i]) return false;
            }
            return true;
        }

        public bool TryExecute(List<string> args, out string failureReason)
        {
            if (!Matches(args))
            {
                failureReason = $"Syntax: {GetSyntax()}";
                return false;
            }

            failureReason = DoExecute(args.GetRange(opcode.Count, args.Count - opcode.Count));
            return failureReason == null;
        }

        protected abstract string DoExecute(List<string> args);
    }

    public abstract class ArgCommand : Command
    {
        private readonly List<string> parameters;

        public ArgCommand(List<string> opcode, List<string> parameters) : base(opcode)
        {
            this.parameters = parameters;
        }

        protected override string DoExecute(List<string> args)
        {
            if (args.Count != parameters.Count)
            {
                var parts = new List<string>() { "Syntax:", GetSyntax() };
                foreach (var p in parameters) parts.Add($"[{p}]");
                return string.Join(" ", parts);
            }

            return DoExecuteArgs(args);
        }

        protected abstract string DoExecuteArgs(List<string> args);
    }

    public class SetCommand : ArgCommand
    {
        public SetCommand() : base(new() { "/set" }, new() { "key", "value" }) { }

        protected override string DoExecuteArgs(List<string> args)
        {
            var key = args[0];
            var value = args[1];

            SaveState.TryGetOrFindInstance(PersistenceMode.GlobalPersistence, out var globalState);
            bool success = globalState.TrySetJson(key, value);
            return success ? null : "Failed to set value";
        }
    }

    public class GiveArtifactCommand : ArgCommand
    {
        public GiveArtifactCommand() : base(new() { "/give", "artifact" }, new() { "id" }) { }

        protected override string DoExecuteArgs(List<string> args)
        {
            var artifactId = args[0];

            if (!DataManager.ArtifactRegistry.TryLookup(artifactId, out _)) return $"Invalid id: {artifactId}";
            DataManager.InventoryData.AddArtifact(artifactId);
            return null;
        }
    }

    public class GiveHazardCommand : ArgCommand
    {
        public GiveHazardCommand() : base(new() { "/give", "hazard" }, new() { "id", "level" }) { }

        protected override string DoExecuteArgs(List<string> args)
        {
            var hazardId = args[0];
            if (!int.TryParse(args[1], out var level) || level < 0 || level > 2) return $"Invalid level: {args[1]}";

            if (!DataManager.HazardRegistry.TryLookup(hazardId, out _)) return $"Invalid id: {hazardId}";
            DataManager.InventoryData.AddHazard(hazardId, level);
            return null;
        }
    }
}
