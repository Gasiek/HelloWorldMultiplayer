using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientEnterGameSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (networkId, connectionEntity) in SystemAPI.Query<RefRO<NetworkId>>()
                     .WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(connectionEntity);
            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new EnterGameRpc());
            ecb.AddComponent(rpcEntity, new SendRpcCommandRequest { TargetConnection = connectionEntity });
            state.Enabled = false; // Invoke only once
            break;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}