using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaOnWinFight : Signal { }
public class ArenaOnKillEnemy : Signal<Enemy> { }
public class InventoryOnAquireArtifact : Signal<string> { }

public class PauseMenuIsOpen : Signal<bool> { }
public class InventoryIsOpen : Signal<bool> { }