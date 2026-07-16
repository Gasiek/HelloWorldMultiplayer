using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public int playerId;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(
                TransformUsageFlags.Dynamic
            );

            AddComponent<PlayerTag>(entity);

            AddComponent(entity, new PlayerId
            {
                Value = authoring.playerId
            });
        }
    }
}