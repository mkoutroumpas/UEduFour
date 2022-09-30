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
        public ArchetypeChunkComponentType<Scale> ScaleArchetypeChunkComponentType;
        [ReadOnly] public ArchetypeChunkComponentType<Scaler> ScalerArchetypeChunkComponentType;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Scale> chunkScales = chunk.GetNativeArray(ScaleArchetypeChunkComponentType);
            NativeArray<Scaler> chunkScalers = chunk.GetNativeArray(ScalerArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Scale scale = chunkScales[i];
                Scaler scaler = chunkScalers[i];


            }
        }
    }

    protected override void OnUpdate()
    {
        RotatorJob rotatorJob = new RotatorJob()
        {
            RotationArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotation>(false),
            RotatorArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotator>(true),
            DeltaTime = Time.DeltaTime
        };

        ScalerJob scalerJob = new ScalerJob()
        {
            ScaleArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scale>(false),
            ScalerArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scaler>(true),
            DeltaTime = Time.DeltaTime
        };

        this.Dependency = JobHandle.CombineDependencies
            (rotatorJob.ScheduleParallel(entityQuery, this.Dependency), 
            scalerJob.ScheduleParallel(entityQuery, this.Dependency));
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        entityQuery = GetEntityQuery(ComponentType.ReadOnly<Rotator>(), ComponentType.ReadOnly<Scaler>());

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { RotationSpeed = 3 });

        EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { ScaleFromTo = new Vector2(0.5f, 2f), ScaleSpeed = 2 });
    }
}
