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
            // Assign unique slot
            int slot = counter.NextSlot;
            counter.NextSlot++;

            ecb.AddComponent(connectionEntity, new PlayerSlot
            {
                Value = slot
            });

            // Choose player prefab
            Entity prefabToSpawn = slot == 0
                ? spawner.PlayerPrefabRed
                : spawner.PlayerPrefabBlue;

            Entity playerEntity = ecb.Instantiate(prefabToSpawn);

            ecb.AddComponent<PlayerTag>(playerEntity);

            // Different spawn positions per slot
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

            ecb.AddComponent<PlayerSpawned>(connectionEntity);
        }

        // Save updated slot counter once
        SystemAPI.SetSingleton(counter);

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}