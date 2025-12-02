using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Debug = UnityEngine.Debug;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
    ComponentLookup<NetworkId> _networkIDTable;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawner>();

        var query = new EntityQueryBuilder(Allocator.Temp)
          .WithAll<GoInGameRequest>()
          .WithAll<ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(query));

        _networkIDTable = state.GetComponentLookup<NetworkId>(isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state)
    {
        var prefab = SystemAPI.GetSingleton<PlayerSpawner>().Prefab;
        var cb = new EntityCommandBuffer(Allocator.Temp);
        _networkIDTable.Update(ref state);

        // Debug info
        state.EntityManager.GetName(prefab, out var prefabName);
        var worldName = state.WorldUnmanaged.Name;

        foreach (var (req, req_entity) in
          SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
            .WithAll<GoInGameRequest>()
            .WithEntityAccess())
        {
            var src_entity = req.ValueRO.SourceConnection;
            var src_id = _networkIDTable[src_entity].Value;

            cb.AddComponent<NetworkStreamInGame>(src_entity);

            Debug.Log($"[{worldName}] Connection '{src_id}' to in game," +
                      $" spawning {prefabName}.");

            var player = cb.Instantiate(prefab);
            cb.SetComponent(player, new GhostOwner { NetworkId = src_id });
            cb.AppendToBuffer(src_entity, new LinkedEntityGroup { Value = player });

            cb.DestroyEntity(req_entity);
        }

        cb.Playback(state.EntityManager);
    }
}
