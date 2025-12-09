using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
                   WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct GoInGameClientSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var query = new EntityQueryBuilder(Allocator.Temp)
          .WithAll<NetworkId>()
          .WithNone<NetworkStreamInGame>();
        state.RequireForUpdate(state.GetEntityQuery(query));
    }

    public void OnUpdate(ref SystemState state)
    {
        var cb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (target_id, target_entity) in
          SystemAPI.Query<RefRO<NetworkId>>()
            .WithNone<NetworkStreamInGame>()
            .WithEntityAccess())
        {
            cb.AddComponent<NetworkStreamInGame>(target_entity);

            var req_entity = cb.CreateEntity();

            cb.AddComponent<GoInGameRequest>(req_entity);

            cb.AddComponent(req_entity,
              new SendRpcCommandRequest { TargetConnection = target_entity });
        }

        cb.Playback(state.EntityManager);
    }
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
                   WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct PlayerSpawnRequestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var query = new EntityQueryBuilder(Allocator.Temp)
          .WithAll<PlayerSpawnRequest>()
          .WithNone<SendRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(query));
    }

    public void OnUpdate(ref SystemState state)
    {
        var cb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (_, entity) in
          SystemAPI.Query<PlayerSpawnRequest>()
            .WithNone<SendRpcCommandRequest>()
            .WithEntityAccess())
        {
            cb.AddComponent<SendRpcCommandRequest>(entity);
        }

        cb.Playback(state.EntityManager);
    }
}
