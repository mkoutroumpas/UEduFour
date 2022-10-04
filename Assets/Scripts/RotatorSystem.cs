﻿using Unity.Entities;
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
    struct ScalerAndRotatorJob : IJobChunk
    {
        public float DeltaTime;
        public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldArchetypeChunkComponentType;
        [ReadOnly] public ArchetypeChunkComponentType<Scaler> ScalerArchetypeChunkComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<LocalToWorld> chunkLocalToWorlds = chunk.GetNativeArray(LocalToWorldArchetypeChunkComponentType);
            NativeArray<Scaler> chunkScalers = chunk.GetNativeArray(ScalerArchetypeChunkComponentType);

            for (var i = 0; i < chunk.Count; i++)
            {
                LocalToWorld localToWorld = chunkLocalToWorlds[i];
                Scaler scaler = chunkScalers[i];

                chunkLocalToWorlds[i] = new LocalToWorld
                {
                    Value = float4x4.TRS(localToWorld.Position, localToWorld.Rotation, scaler.To)
                };

                Debug.Log($"chunkLocalToWorlds[{i}].Value = {chunkLocalToWorlds[i].Value}");
            }
        }
    }

    protected override void OnUpdate()
    {
        ScalerAndRotatorJob scalerAndRotatorJob = new ScalerAndRotatorJob()
        {
            LocalToWorldArchetypeChunkComponentType = GetArchetypeChunkComponentType<LocalToWorld>(false),
            ScalerArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scaler>(true),
            DeltaTime = Time.DeltaTime
        };

        this.Dependency = scalerAndRotatorJob.ScheduleParallel(entityQuery, this.Dependency);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        entityQuery = GetEntityQuery(ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<Scaler>());

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { To = 5f });

    }
}
