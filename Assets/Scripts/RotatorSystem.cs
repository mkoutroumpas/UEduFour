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
    struct ScalerAndRotatorJob : IJobChunk
    {
        public float DeltaTime;

        public ArchetypeChunkComponentType<LocalToWorld> LocalToWorldArchetypeChunkComponentType;
        [ReadOnly] public ArchetypeChunkComponentType<Scaler> ScalerArchetypeChunkComponentType;
        [ReadOnly] public ArchetypeChunkComponentType<Rotator> RotatorArchetypeChunkComponentType;

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

                Rotation rotation = new Rotation
                {
                    Value = math.mul(math.normalize(localToWorld.Rotation),
                        quaternion.AxisAngle(math.up(), rotator.Speed * DeltaTime))
                };

                chunkLocalToWorlds[i] = new LocalToWorld
                {
                    Value = float4x4.TRS(localToWorld.Position, quaternion.EulerXYZ(0, 45 * math.PI / 180, 0), scaler.To)
                };

                //Debug.Log($"chunkLocalToWorlds[{i}].Value = {chunkLocalToWorlds[i].Value}");
            }
        }
    }

    protected override void OnUpdate()
    {
        Dependency = new ScalerAndRotatorJob()
        {
            LocalToWorldArchetypeChunkComponentType = GetArchetypeChunkComponentType<LocalToWorld>(false),
            ScalerArchetypeChunkComponentType = GetArchetypeChunkComponentType<Scaler>(true),
            RotatorArchetypeChunkComponentType = GetArchetypeChunkComponentType<Rotator>(true),
            DeltaTime = Time.DeltaTime

        }.ScheduleParallel(entityQuery, Dependency);
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        entityQuery = GetEntityQuery(
            ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<Scaler>(), ComponentType.ReadOnly<Rotator>());

        GameObject cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        Entity cubeEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cube, settings);

        Entity testCubeEntityInstance = EntityManager.Instantiate(cubeEntity);

        EntityManager.AddComponentData(testCubeEntityInstance, new Scaler { To = 5f });
        EntityManager.AddComponentData(testCubeEntityInstance, new Rotator { Speed = 3f });

    }
}
