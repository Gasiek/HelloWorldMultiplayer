using Unity.Entities;
using UnityEngine;

public class PlayerSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefabRed;
    [SerializeField] private GameObject playerPrefabBlue;

    class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerSpawner
            {
                PlayerPrefabRed = GetEntity(authoring.playerPrefabRed, TransformUsageFlags.Dynamic),
                PlayerPrefabBlue = GetEntity(authoring.playerPrefabBlue, TransformUsageFlags.Dynamic)
            });
            AddBuffer<FallEventBufferElement>(entity);
        }
    }
}