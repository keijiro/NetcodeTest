using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{
    public float Speed;
}

[DisallowMultipleComponent]
public sealed class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] float _speed = 1f;

    class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var component = new Player { Speed = authoring._speed };
            AddComponent(entity, component);
        }
    }
}
