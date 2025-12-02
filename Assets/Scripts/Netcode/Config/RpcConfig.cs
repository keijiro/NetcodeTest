using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
                   WorldSystemFilterFlags.ServerSimulation |
                   WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[CreateAfter(typeof(RpcSystem))]
public partial struct SetRpcSystemDynamicAssemblyListSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var collection = SystemAPI.GetSingletonRW<RpcCollection>();
        collection.ValueRW.DynamicAssemblyList = true;
        state.Enabled = false;
    }
}
