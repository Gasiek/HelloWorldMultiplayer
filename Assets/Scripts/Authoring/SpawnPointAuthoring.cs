using Unity.Entities;
using UnityEngine;

public class SpawnPointAuthoring : MonoBehaviour
{
    public int slot;

    public class Baker : Baker<SpawnPointAuthoring>
    {
        public override void Bake(SpawnPointAuthoring authoring)
        {
            Entity entity = GetEntity(
                TransformUsageFlags.WorldSpace);

            AddComponent(entity, new SpawnPoint
            {
                Slot = authoring.slot,
                Position = authoring.transform.position
            });
        }
    }
}