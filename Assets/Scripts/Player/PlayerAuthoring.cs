using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{
}

[DisallowMultipleComponent]
public sealed class PlayerAuthoring : MonoBehaviour
{
    class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
          => AddComponent<Player>(GetEntity(TransformUsageFlags.Dynamic));
    }
}
