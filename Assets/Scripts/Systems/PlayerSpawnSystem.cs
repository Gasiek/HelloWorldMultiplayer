using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct PlayerSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawner>();
        state.RequireForUpdate<PlayerSlotCounter>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        PlayerSpawner spawner = SystemAPI.GetSingleton<PlayerSpawner>();
        PlayerSlotCounter counter = SystemAPI.GetSingleton<PlayerSlotCounter>();

        foreach (var (networkId, connectionEntity)
                 in SystemAPI.Query<RefRO<NetworkId>>()
                 .WithAll<NetworkStreamInGame>()
                 .WithNone<PlayerSpawned>()
                 .WithEntityAccess())
        {
            // Assign unique player slot
            int slot = counter.NextSlot;
            counter.NextSlot++;


            // Choose prefab based on slot
            Entity prefabToSpawn = slot == 0
                ? spawner.PlayerPrefabRed
                : spawner.PlayerPrefabBlue;


            Entity playerEntity = ecb.Instantiate(prefabToSpawn);


            // Add gameplay components to the PLAYER entity
            ecb.AddComponent<PlayerTag>(playerEntity);

            ecb.AddComponent(playerEntity, new PlayerSlot
            {
                Value = slot
            });


            // Spawn position
            float xPosition = slot == 0 ? -2f : 2f;

            ecb.SetComponent(playerEntity,
                LocalTransform.FromPosition(
                    new float3(xPosition, 2f, 0f)
                ));


            // Give ownership to this connection
            ecb.SetComponent(playerEntity,
                new GhostOwner
                {
                    NetworkId = networkId.ValueRO.Value
                });


            // Link player to connection
            ecb.AppendToBuffer(connectionEntity,
                new LinkedEntityGroup
                {
                    Value = playerEntity
                });


            // Mark connection as already spawned
            ecb.AddComponent<PlayerSpawned>(connectionEntity);
        }


        // Save updated slot counter
        SystemAPI.SetSingleton(counter);


        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}