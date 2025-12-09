using UnityEngine;
using Unity.Entities;
using Unity.NetCode;

public enum PlayerDirection { None, Right, Up, Left, Down }

public struct PlayerInput : IInputComponentData
{
    public PlayerDirection Direction;
}

[DisallowMultipleComponent]
public sealed class PlayerInputAuthoring : MonoBehaviour
{
    class PlayerInputBaker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
          => AddComponent<PlayerInput>(GetEntity(TransformUsageFlags.Dynamic));
    }
}
