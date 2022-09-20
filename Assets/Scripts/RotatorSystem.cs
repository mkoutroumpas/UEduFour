using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class RotatorSystem : JobComponentSystem
{
    [BurstCompile]
    struct RotatorJob : IJobChunk
    {
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        EntityManager.SetComponentData(testCubeEntityInstance, new Rotator { RotationSpeed = 50 });

    }
}
