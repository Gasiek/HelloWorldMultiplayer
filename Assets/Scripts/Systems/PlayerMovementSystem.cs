using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float speed = 5f;

        foreach (var (input, velocity) 
                 in SystemAPI.Query<
                         RefRO<PlayerInput>,
                         RefRW<PhysicsVelocity>>()
                     .WithAll<Simulate, GhostOwnerIsLocal>())
        {
            float3 linear = velocity.ValueRO.Linear;

            linear.x = input.ValueRO.Movement.x * speed;
            linear.z = input.ValueRO.Movement.y * speed;

            velocity.ValueRW.Linear = linear;
        }
    }
}