using Unity.Entities;
using UnityEngine;

public struct Rotator : IComponentData
{
    public float RotationSpeed;
}

public struct Scaler : IComponentData
{
    public float ScaleFrom;
    public float ScaleTo;
    public float ScaleSpeed;
}