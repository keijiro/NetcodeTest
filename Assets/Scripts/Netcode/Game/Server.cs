using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var query = new EntityQueryBuilder(Allocator.Temp)
          .WithAll<GoInGameRequest>()
          .WithAll<ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(query));
    }

    public void OnUpdate(ref SystemState state)
    {
        var cb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (req, req_entity) in
          SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
            .WithAll<GoInGameRequest>()
            .WithEntityAccess())
        {
            var src_entity = req.ValueRO.SourceConnection;
            cb.AddComponent<NetworkStreamInGame>(src_entity);
            cb.DestroyEntity(req_entity);
        }

        cb.Playback(state.EntityManager);
    }
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct PlayerSpawnSystem : ISystem
{
    ComponentLookup<NetworkId> _networkIDTable;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawner>();

        var query = new EntityQueryBuilder(Allocator.Temp)
          .WithAll<PlayerSpawnRequest>()
          .WithAll<ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(query));

        _networkIDTable = state.GetComponentLookup<NetworkId>(isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state)
    {
        var prefab = SystemAPI.GetSingleton<PlayerSpawner>().Prefab;
        var cb = new EntityCommandBuffer(Allocator.Temp);
        _networkIDTable.Update(ref state);

        foreach (var (req, req_entity) in
          SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
            .WithAll<PlayerSpawnRequest>()
            .WithEntityAccess())
        {
            var src_entity = req.ValueRO.SourceConnection;
            var src_id = _networkIDTable[src_entity].Value;

            var player = cb.Instantiate(prefab);
            cb.SetComponent(player, new GhostOwner { NetworkId = src_id });
            cb.AppendToBuffer(src_entity, new LinkedEntityGroup { Value = player });

            cb.DestroyEntity(req_entity);
        }

        cb.Playback(state.EntityManager);
    }
}
