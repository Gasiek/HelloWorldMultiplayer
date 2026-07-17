using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct PlayerSlotCounterSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateEntity(typeof(PlayerSlotCounter));

        SystemAPI.SetSingleton(new PlayerSlotCounter
        {
            NextSlot = 0
        });
    }

    public void OnUpdate(ref SystemState state)
    {
    }
}