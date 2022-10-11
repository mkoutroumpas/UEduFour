﻿using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

public class RotatorScalerSystem : SystemBase
{
    private EntityQuery entityQuery;

    private const int DemoCubesAmountDimension = 60;
    private const int DemoCubesAmountStep = 5;

    [BurstCompile]
    struct ScalerAndRotatorJob : IJobChunk
    {
        public float DeltaTime;

        public bool EnableLogging;

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

                if (EnableLogging) Debug.Log($"scaler.Speed = {scaler.Speed}, scale = {scale}"); // scale is sometimes zero. Investigate why.

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

                if (EnableLogging) Debug.Log($"chunkRotators[{i}].CurrentAngle = {chunkRotators[i].Angle}");
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
        Unity.Mathematics.Random r = new Unity.Mathematics.Random(1);
        
        for (int x = -cubesAmountDimension; x <= cubesAmountDimension; x += cubesAmountStep)
        {
            for (int z = -cubesAmountDimension; z <= cubesAmountDimension; z += cubesAmountStep)
            {
                Entity testCubeEntityInstance = EntityManager.Instantiate(basedOnEntity);

                EntityManager.SetComponentData(testCubeEntityInstance, new Translation { Value = new float3(x, 0f, z) });

                EntityManager.AddComponentData(testCubeEntityInstance, new Translator { From = r.NextFloat(-2.5f, -1f), To = r.NextFloat(2f, 3.5f), Along = Axis.Y });
                EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { From = r.NextFloat(0.5f, 2f), To = r.NextFloat(3f, 7f), Speed = r.NextFloat(0.005f, 0.2f), Scale = 2.5f });
                EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { Speed = r.NextFloat(20f, 80f), Angle = r.NextFloat(0f, 90f) });
            }
        }
    }
}
