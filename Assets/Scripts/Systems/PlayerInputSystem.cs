using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerInputSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float2 moveInput = float2.zero;

        if (Keyboard.current != null)
        {
            moveInput.x = 
                (Keyboard.current.dKey.isPressed ? 1 : 0) -
                (Keyboard.current.aKey.isPressed ? 1 : 0);

            moveInput.y =
                (Keyboard.current.wKey.isPressed ? 1 : 0) -
                (Keyboard.current.sKey.isPressed ? 1 : 0);
        }

        foreach (var input in SystemAPI.Query<RefRW<PlayerInput>>()
                     .WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW.Movement = moveInput;
        }
    }
}