using Unity.Entities;

[GenerateAuthoringComponent]
public struct ProjectilePoolSettingsComponent : IComponentData
{
    public int PoolAmount;
    public Entity projectilePrefab;
}