using Unity.Entities;
using UnityEngine;

public struct Rotator : IComponentData
{
    public float Speed;
}

public struct Scaler : IComponentData
{
    public float From;
    public float To;
    public float Speed;
}