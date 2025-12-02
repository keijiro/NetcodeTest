using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity Bot;
}

[DisallowMultipleComponent]
public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Bot;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var component = default(Spawner);
            component.Bot = GetEntity(authoring.Bot, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, component);
        }
    }
}
