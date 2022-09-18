using Unity.Entities;
using UnityEngine;

public class EntityConverter : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem gameObjectConversionSystem)
    {
        entityManager.AddComponent(entity, typeof(Rotator));
    }
}
