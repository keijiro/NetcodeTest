using Unity.Entities;
using UnityEngine;

public struct Bot : IComponentData
{
}

[DisallowMultipleComponent]
public class BotAuthoring : MonoBehaviour
{
    class BotBaker : Baker<BotAuthoring>
    {
        public override void Bake(BotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Bot>(entity);
        }
    }
}
