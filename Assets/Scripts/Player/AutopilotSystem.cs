using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

using Dir = PlayerDirection;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct AutopilotSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (autopilot, xform, input) in
          SystemAPI.Query<RefRO<Autopilot>,
                          RefRO<LocalTransform>,
                          RefRW<PlayerInput>>()
            .WithAll<Simulate>())
        {
            var pos = xform.ValueRO.Position.xz;
            var border = autopilot.ValueRO.Bounds * 0.5f;

            var dir = input.ValueRO.Direction;

            switch (dir)
            {
                case Dir.Left:  if (pos.x < -border.x) dir = Dir.Down;  break;
                case Dir.Down:  if (pos.y < -border.y) dir = Dir.Right; break;
                case Dir.Right: if (pos.x > +border.x) dir = Dir.Up;    break;
                case Dir.Up:    if (pos.y > +border.y) dir = Dir.Left;  break;
            }

            input.ValueRW.Direction  = dir;
        }
    }
}
