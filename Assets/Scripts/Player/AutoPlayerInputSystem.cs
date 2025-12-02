using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct AutoPlayerInputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
      => state.RequireForUpdate<PlayerInput>();

    public void OnUpdate(ref SystemState state)
    {
        var timeTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick.TickIndexForValidTick;
        var tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationFixedTimeStep;
        var time = timeTick * tickRate;

        foreach (var (player, input) in
          SystemAPI.Query<RefRO<Player>,
                          RefRW<PlayerInput>>()
            .WithAll<GhostOwnerIsLocal>())
        {
            var phase = (int)(time / player.ValueRO.TurnInterval);
            input.ValueRW.Direction = (PlayerDirection)(phase & 3);
        }
    }
}
