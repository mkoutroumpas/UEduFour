using Unity.Entities;

public struct Rotator : IComponentData
{
    public float Speed { get; set; }
    public float CurrentAngle { get; set; }
}

public struct Scaler : IComponentData
{
    public float From;
    public float To;
    public float Speed;
}