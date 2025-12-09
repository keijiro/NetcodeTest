using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        foreach (var (player, input, xform) in
          SystemAPI.Query<RefRO<Player>,
                          RefRO<PlayerInput>,
                          RefRW<LocalTransform>>()
            .WithAll<Simulate>())
        {
            var dir = input.ValueRO.Direction switch
            {
                PlayerDirection.None  => float3.zero,
                PlayerDirection.Right => new float3(1, 0, 0),
                PlayerDirection.Up    => new float3(0, 0, 1),
                PlayerDirection.Left  => new float3(-1, 0, 0),
                PlayerDirection.Down  => new float3(0, 0, -1),
                _                     => float3.zero
            };

            xform.ValueRW.Position += dir * player.ValueRO.Speed * dt;

            if (!dir.Equals(float3.zero))
            {
                var halfLife = 0.05f;
                var target = quaternion.LookRotationSafe(dir, math.up());
                var para = math.exp2(-dt / halfLife);
                xform.ValueRW.Rotation =
                  math.nlerp(target, xform.ValueRO.Rotation, para);
            }
        }
    }
}
