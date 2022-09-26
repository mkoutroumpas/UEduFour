using Unity.Entities;

public struct Rotator : IComponentData
{
    public float RotationSpeed;
}

public struct Scaler : IComponentData
{
    public float Amount;
}