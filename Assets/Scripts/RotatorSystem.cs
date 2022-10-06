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

    private const int DemoCubesAmountDimension = 40;
    private const int DemoCubesAmountStep = 5;

    [BurstCompile]
    struct ScalerAndRotatorJob : IJobChunk
    {
        public float DeltaTime;

        public bool EnableDebugLogging;

        public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldArchetypeChunkComponentType;
        public ArchetypeChunkComponentType<Scaler> ScalerArchetypeChunkComponentType;
        public ArchetypeChunkComponentType<Rotator> RotatorArchetypeChunkComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<LocalToWorld> chunkLocalToWorlds = chunk.GetNativeArray(LocalToWorldArchetypeChunkComponentType);
            NativeArray<Scaler> chunkScalers = chunk.GetNativeArray(ScalerArchetypeChunkComponentType);
            NativeArray<Rotator> chunkRotators = chunk.GetNativeArray(RotatorArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                LocalToWorld localToWorld = chunkLocalToWorlds[i];
                Scaler scaler = chunkScalers[i];
                Rotator rotator = chunkRotators[i];

                float scale = scaler.Scale;

                if (scale >= scaler.To || scale <= scaler.From)
                {
                    scaler.Speed = -scaler.Speed;
                }

                scale += scaler.Speed;

                chunkScalers[i] = new Scaler
                {
                    From = scaler.From,
                    To = scaler.To,
                    Speed = scaler.Speed,
                    Scale = scale
                };

                if (EnableDebugLogging) Debug.Log($"scaler.Speed = {scaler.Speed}");

                if (EnableDebugLogging) Debug.Log($"scale = {scale}"); // scale is sometimes zero. Investigate why.

                float angleDegrees = rotator.Angle + rotator.Speed * DeltaTime;

                chunkLocalToWorlds[i] = new LocalToWorld
                {
                    Value = float4x4.TRS(localToWorld.Position, quaternion.EulerXYZ(0, math.radians(angleDegrees), 0), scale)
                };

                chunkRotators[i] = new Rotator
                {
                    Angle = angleDegrees,
                    Speed = rotator.Speed
                };

                if (EnableDebugLogging) Debug.Log($"chunkRotators[{i}].CurrentAngle = {chunkRotators[i].Angle}");
            }
        }
    }

    protected override void OnUpdate()
    {
        Dependency = new ScalerAndRotatorJob()
        {
            LocalToWorldArchetypeChunkComponentType = GetArchetypeChunkComponentType<LocalToWorld>(),
            ScalerArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scaler>(),
            RotatorArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotator>(),
            DeltaTime = Time.DeltaTime

        }.ScheduleParallel(entityQuery, Dependency);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        entityQuery = GetEntityQuery(
            ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<Scaler>(), ComponentType.ReadWrite<Rotator>());

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        GenerateAndAddDemoEntitiesXZ(cubeEntity, DemoCubesAmountDimension, DemoCubesAmountStep);
    }

    private void GenerateAndAddDemoEntitiesXZ(Entity basedOnEntity, int cubesAmountDimension, int cubesAmountStep)
    {
        for (int x = -cubesAmountDimension; x <= cubesAmountDimension; x += cubesAmountStep)
        {
            for (int z = -cubesAmountDimension; z <= cubesAmountDimension; z += cubesAmountStep)
            {
                Entity testCubeEntityInstance = EntityManager.Instantiate(basedOnEntity);

                EntityManager.SetComponentData(testCubeEntityInstance, new Translation { Value = new float3(x, 0f, z) });
                EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { From = 2f, To = 5f, Speed = 0.1f, Scale = 3f });
                EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { Speed = 60f, Angle = 0f });
            }
        }
    }
}
