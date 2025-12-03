using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Autopilot : IComponentData
{
    public float2 Bounds;
}

[DisallowMultipleComponent]
public sealed class AutopilotAuthoring : MonoBehaviour
{
    [SerializeField] float2 _bounds = new float2(1, 1);

    class PlayerBaker : Baker<AutopilotAuthoring>
    {
        public override void Bake(AutopilotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var component = new Autopilot { Bounds = authoring._bounds };
            AddComponent(entity, component);
        }
    }
}
