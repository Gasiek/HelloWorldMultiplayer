using Unity.Entities;
using Unity.Mathematics;

public struct SpawnPoint : IComponentData
{
    public int Slot;
    public float3 Position;
}