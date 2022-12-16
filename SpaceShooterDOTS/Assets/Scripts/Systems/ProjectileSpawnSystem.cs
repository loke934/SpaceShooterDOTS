using Unity.Entities;

public partial class ProjectileSpawnSystem : SystemBase
{
    private Entity prefab;
    EntityManager entityManager;

    protected override void OnCreate()
    {
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<ProjectilePoolSettingsComponent>().projectilePrefab;
        }

        int numberToSpawn = GetSingleton<ProjectilePoolSettingsComponent>().PoolAmount;
        Entities
             .WithStructuralChanges()
             .ForEach((Shooting shoot ) =>
             {
                 shoot.entityManager = entityManager;
                 shoot.poolSettings = GetSingleton<ProjectilePoolSettingsComponent>();

                 for (int i = shoot.projectilePool.Count; i < numberToSpawn; ++i)
                 {
                     Entity entity = entityManager.Instantiate(prefab);
                     entityManager.SetEnabled(entity, false);
                     shoot.projectilePool.Add(entity);
                 }

             }).Run();
    }
}
