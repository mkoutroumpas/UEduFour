using Unity.Entities;
using UnityEngine;

public struct Rotator : IComponentData
{
    public float RotationSpeed;
}

public struct Scaler : IComponentData
{
    public Vector2 ScaleFromTo;
    public float ScaleSpeed;
}