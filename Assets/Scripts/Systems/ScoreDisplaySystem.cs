using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ScoreDisplaySystem : ISystem
{
    private int lastRedScore;
    private int lastBlueScore;

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerSpawner>(
                out Entity gameStateEntity))
            return;

        DynamicBuffer<FallEventBufferElement> scoreBuffer =
            SystemAPI.GetBuffer<FallEventBufferElement>(gameStateEntity);


        int redScore = 0;
        int blueScore = 0;


        foreach (var ev in scoreBuffer)
        {
            switch (ev.WinnerSlot)
            {
                case 0:
                    redScore++;
                    break;

                case 1:
                    blueScore++;
                    break;
            }
        }


        if (redScore != lastRedScore || blueScore != lastBlueScore)
        {
            Debug.Log($"Red: {redScore} | Blue: {blueScore}");

            lastRedScore = redScore;
            lastBlueScore = blueScore;
        }
    }
}