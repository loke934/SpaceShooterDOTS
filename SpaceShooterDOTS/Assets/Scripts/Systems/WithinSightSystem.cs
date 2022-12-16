using Unity.Transforms;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using System.Diagnostics;

public partial class WithinSightSystem : SystemBase
{
    private float distanceToDisable = 70f;
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        float3 cameraPos = new float3();

        Entities
           .WithAll<CameraTag>()
           .WithoutBurst()
           .ForEach((Transform transform) => { cameraPos.xyz = new float3(transform.position.x, transform.position.y, transform.position.z); }).Run();

        MoveEnemiesWithinSight(cameraPos);
        DisableProjectilesOutOfSight(cameraPos);
    }

    private void DisableProjectilesOutOfSight(float3 cameraPos)
    {
        Entities
           .WithStructuralChanges()
           .WithAll<ProjectileTag>()
           .ForEach((Entity entity, in Translation position) =>
           {
               if (position.Value.z > cameraPos.z + distanceToDisable)
               {
                   entityManager.SetEnabled(entity, false);
               }

           }).Run();
    }

    private void MoveEnemiesWithinSight(float3 cameraPos)
    {
        EnemySpawnSettingsComponent settings = GetSingleton<EnemySpawnSettingsComponent>();
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

        Entities
           .WithAll<EnemyTag>()
           .ForEach((ref Translation position) =>
           {
               if (position.Value.z < cameraPos.z)
               {
                   float xPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
                   float yPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
                   float zPosition = rand.NextFloat(settings.spawnRadiusZMin, settings.spawnRadiusZMax);
                   float randSpeed = rand.NextFloat(settings.minSpeed, settings.maxSpeed);
                   position.Value.x = cameraPos.x + xPosition;
                   position.Value.y = cameraPos.y + yPosition;
                   position.Value.z = cameraPos.z + zPosition;
               }

           }).ScheduleParallel();
    }
}

