using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHazard
{
    public abstract void Tick(float time);
    public abstract void Init(Arena arena, int level);
}
