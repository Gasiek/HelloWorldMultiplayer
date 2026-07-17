using Unity.Entities;
using UnityEngine;

public class PlayerInputAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            AddComponent<PlayerInput>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}