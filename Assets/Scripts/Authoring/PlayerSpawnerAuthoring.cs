using Unity.Entities;
using UnityEngine;

public class PlayerSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlayerSpawner
            {
                PlayerPrefab = GetEntity(
                    authoring.playerPrefab,
                    TransformUsageFlags.Dynamic)
            });
        }
    }
}