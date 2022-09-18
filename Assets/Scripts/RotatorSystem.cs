using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class RotatorSystem : JobComponentSystem
{
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

        Entity cubeEntityInstance = EntityManager.Instantiate(cubeEntity);

    }
}
