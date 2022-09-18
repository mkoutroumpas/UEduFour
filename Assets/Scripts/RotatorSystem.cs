using Unity.Entities;
using Unity.Jobs;

public class RotatorSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

    }
}
