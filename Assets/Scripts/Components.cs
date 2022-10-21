using Unity.Entities;

[GenerateAuthoringComponent]
public struct Rotator : IComponentData
{
    public float Speed;
    public float Angle;
    public bool Enabled;
}

[GenerateAuthoringComponent]
public struct Scaler : IComponentData
{
    public float From;
    public float To;
    public float Speed;
    public float Scale;
    public bool Enabled;
}

[GenerateAuthoringComponent]
public struct Translator : IComponentData
{
    public float From;
    public float To;
    public float Speed;
    public float Translation;
    public Axis Along;
    public bool Enabled;
}