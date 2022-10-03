using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

public class RotatorSystem : SystemBase
{
    private EntityQuery entityQuery;

    [BurstCompile]
    struct RotatorJob : IJobChunk
    {
        public float DeltaTime;
        public ArchetypeChunkComponentType<Rotation> RotationArchetypeChunkComponentType;
        [ReadOnly] public ArchetypeChunkComponentType<Rotator> RotatorArchetypeChunkComponentType;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Rotation> chunkRotations = chunk.GetNativeArray(RotationArchetypeChunkComponentType);
            NativeArray<Rotator> chunkRotators = chunk.GetNativeArray(RotatorArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Rotation rotation = chunkRotations[i];
                Rotator rotator = chunkRotators[i];

                chunkRotations[i] = new Rotation
                {
                    Value = math.mul(math.normalize(rotation.Value),
                        quaternion.AxisAngle(math.up(), rotator.RotationSpeed * DeltaTime))
                };
            }
        }
    }

    [BurstCompile]
    struct ScalerJob : IJobChunk
    {
        public float DeltaTime;
        public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldArchetypeChunkComponentType;
        //[ReadOnly] public ArchetypeChunkComponentType<Scaler> ScalerArchetypeChunkComponentType;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<LocalToWorld> chunkLocalToWorlds = chunk.GetNativeArray(LocalToWorldArchetypeChunkComponentType);
            //NativeArray<Scaler> chunkScalers = chunk.GetNativeArray(ScalerArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                LocalToWorld localToWorld = chunkLocalToWorlds[i];
                //Scaler scaler = chunkScalers[i];

                chunkLocalToWorlds[i] = new LocalToWorld
                {
                    Value = float4x4.TRS(localToWorld.Position, localToWorld.Rotation, 5f)
                };

                Debug.Log($"chunkLocalToWorlds[{i}].Value = {chunkLocalToWorlds[i].Value}");
            }
        }
    }

    protected override void OnUpdate()
    {
        //RotatorJob rotatorJob = new RotatorJob()
        //{
        //    RotationArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotation>(false),
        //    RotatorArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotator>(true),
        //    DeltaTime = Time.DeltaTime
        //};

        ScalerJob scalerJob = new ScalerJob()
        {
            LocalToWorldArchetypeChunkComponentType = GetArchetypeChunkComponentType<LocalToWorld>(false),
            //ScalerArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scaler>(true),
            DeltaTime = Time.DeltaTime
        };

        //this.Dependency = JobHandle.CombineDependencies
        //    (rotatorJob.ScheduleParallel(entityQuery, this.Dependency), 
        //    scalerJob.ScheduleParallel(entityQuery, this.Dependency));

        this.Dependency = scalerJob.ScheduleParallel(entityQuery, this.Dependency);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        //entityQuery = GetEntityQuery(ComponentType.ReadOnly<Rotator>(), ComponentType.ReadOnly<Scaler>());
        entityQuery = GetEntityQuery(ComponentType.ReadOnly<LocalToWorld>());

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        //EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { RotationSpeed = 3 });

        //EntityManager.AddComponent<Scale>(testCubeEntityInstance);
        //EntityManager.SetComponentData(testCubeEntityInstance, new Scale { Value = 1 });

        //EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { ScaleFrom = 0.5f, ScaleTo = 2f, ScaleSpeed = 2 });

        Debug.Log($"EntityManager.HasComponent<LocalToWorld>(testCubeEntityInstance): {EntityManager.HasComponent<LocalToWorld>(testCubeEntityInstance)}");
        //Debug.Log($"EntityManager.HasComponent<Translation>(testCubeEntityInstance): {EntityManager.HasComponent<Translation>(testCubeEntityInstance)}");
        //Debug.Log($"EntityManager.HasComponent<Rotation>(testCubeEntityInstance): {EntityManager.HasComponent<Rotation>(testCubeEntityInstance)}");
        //Debug.Log($"EntityManager.HasComponent<Scale>(testCubeEntityInstance): {EntityManager.HasComponent<Scale>(testCubeEntityInstance)}");
    }
}
