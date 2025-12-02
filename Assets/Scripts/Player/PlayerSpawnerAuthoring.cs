using Unity.Entities;
using UnityEngine;

public struct PlayerSpawner : IComponentData
{
    public Entity Prefab;
}

[DisallowMultipleComponent]
public sealed class PlayerSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] GameObject _prefab = null;

    class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            var prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var component = new PlayerSpawner { Prefab = prefab };
            AddComponent(entity, component);
        }
    }
}
