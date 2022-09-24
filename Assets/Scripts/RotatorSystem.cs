using Unity.Burst;
using Unity.Collections;
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
        public ArchetypeChunkComponentType<Rotator> RotatorArchetypeChunkComponentType { get; set; }

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Rotator> chunkRotators = chunk.GetNativeArray(RotatorArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                float rotationSpeed = chunkRotators[i].RotationSpeed;
            }
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
        
        EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { RotationSpeed = 50 });

        entityQuery = GetEntityQuery(ComponentType.ReadWrite<Rotator>());
    }
}
