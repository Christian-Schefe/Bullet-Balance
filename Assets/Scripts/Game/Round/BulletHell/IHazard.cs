using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHazard : ITickable
{
    public abstract void Init(Arena arena, int level);
}
