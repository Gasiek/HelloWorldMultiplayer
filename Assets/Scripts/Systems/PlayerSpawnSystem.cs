using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct PlayerSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawner>();
        state.RequireForUpdate<NetworkId>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        PlayerSpawner playerSpawner =
            SystemAPI.GetSingleton<PlayerSpawner>();

        foreach ((
                     RefRO<NetworkId> networkId,
                     Entity connectionEntity)
                 in SystemAPI.Query<
                         RefRO<NetworkId>>()
                     .WithNone<NetworkStreamInGame>()
                     .WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(
                connectionEntity);

            Entity playerEntity =
                ecb.Instantiate(playerSpawner.PlayerPrefab);

            ecb.SetComponent(playerEntity,
                new GhostOwner
                {
                    NetworkId = networkId.ValueRO.Value
                });

            ecb.AppendToBuffer(
                connectionEntity,
                new LinkedEntityGroup
                {
                    Value = playerEntity
                });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}