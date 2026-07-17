using Unity.Entities;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientScoreDisplaySystem : ISystem
{
    private int lastCount;


    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<GameState>(
                out Entity gameStateEntity))
            return;


        DynamicBuffer<FallEventBufferElement> buffer =
            SystemAPI.GetBuffer<FallEventBufferElement>(
                gameStateEntity);


        if (buffer.Length == lastCount)
            return;


        int redScore = 0;
        int blueScore = 0;


        foreach (var item in buffer)
        {
            if (item.WinnerSlot == 0)
                redScore++;

            if (item.WinnerSlot == 1)
                blueScore++;
        }


        Debug.Log(
            $"Red: {redScore} Blue: {blueScore}"
        );


        lastCount = buffer.Length;
    }
}