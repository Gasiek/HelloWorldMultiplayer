using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct FallDetectionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<GameState>(
                out Entity gameStateEntity))
            return;


        DynamicBuffer<FallEventBufferElement> scoreBuffer =
            SystemAPI.GetBuffer<FallEventBufferElement>(gameStateEntity);
        

        foreach (var (transform, playerSlot, velocity)
                 in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<PlayerSlot>,
                         RefRW<PhysicsVelocity>>()
                     .WithAll<PlayerTag, Simulate>())
        {
            if (transform.ValueRO.Position.y < -5f)
            {
                int loserSlot = playerSlot.ValueRO.Value;

                int winnerSlot = loserSlot == 0 ? 1 : 0;

                scoreBuffer.Add(new FallEventBufferElement
                {
                    WinnerSlot = winnerSlot
                });


                float3 spawnPosition = loserSlot == 0
                    ? new float3(-2f, 2f, 0f)
                    : new float3(2f, 2f, 0f);


                transform.ValueRW.Position = spawnPosition;
                transform.ValueRW.Rotation = quaternion.identity;


                velocity.ValueRW.Linear = float3.zero;
                velocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}