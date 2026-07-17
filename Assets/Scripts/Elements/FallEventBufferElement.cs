using Unity.Entities;
using Unity.NetCode;

public struct FallEventBufferElement : IBufferElementData
{
    [GhostField]
    public int WinnerSlot;
}