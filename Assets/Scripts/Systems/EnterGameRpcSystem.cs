using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct EnterGameRpcSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (rpc, request, entity) in SystemAPI.Query<RefRO<EnterGameRpc>, RefRO<ReceiveRpcCommandRequest>>()
                     .WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
            ecb.DestroyEntity(entity); // Consume RPC
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}