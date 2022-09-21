using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class RotatorSystem : SystemBase
{
    private EntityQuery entityQuery;

    [BurstCompile]
    struct RotatorJob : IJobChunk
    {
        public float DeltaTime { get; set; }

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            
        }
    }

    protected override void OnUpdate()
    {
        RotatorJob rotatorJob = new RotatorJob()
        {
            DeltaTime = Time.DeltaTime
        };

        Dependency = rotatorJob.ScheduleParallel(entityQuery, Dependency);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        EntityManager.SetComponentData(testCubeEntityInstance, new Rotator { RotationSpeed = 50 });

        entityQuery = GetEntityQuery(ComponentType.ReadOnly<Rotator>());
    }
}
