using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using System.Diagnostics;

public partial class QuadrantCollisionCheckingSystem : SystemBase
{
    private const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 5;
    private EntityManager entityManager;
    protected override void OnCreate()
    {
        entityManager = EntityManager;
    }

    protected override void OnUpdate()
    {
        bool isScore = false;
        int scores = 0;

        //Size/Capacity based on number of entities that has collision
        EntityQuery collisionQuery = GetEntityQuery(typeof(CollisionTag));
        //Can store multiple values per key, key is quadrant and value is all entities within a quadrant.
        NativeParallelMultiHashMap<int, Entity> quadrantMap = new NativeParallelMultiHashMap<int, Entity>(collisionQuery.CalculateEntityCount(), Allocator.TempJob);

        PopulateQuadrantMap(quadrantMap);
        
        //Projectiles VS Enemy (hit on shoot), only check collision with enemies within same quadrant.
        Entities
           .WithAll<ProjectileTag>()
           .WithStructuralChanges()
           .ForEach((Entity projectile, in Translation projectilePos, in SphereCollisionComponent sphere) => 
           {
               int mapKey = GetPositionHashMapKey(projectilePos.Value);
               Entity enemy;
               NativeParallelMultiHashMapIterator<int> mapIterator;
               //Iteration in native parallel hash map values.
               //Returns true if we have a value inside this has map key,cycle as long as there is entities in this key.
               if (quadrantMap.TryGetFirstValue(mapKey, out enemy, out mapIterator))
               {
                   do
                   {
                       AABBCollisionComponent aabb = entityManager.GetComponentData<AABBCollisionComponent>(enemy);
                       if (IsSphereAABBCollision(sphere,aabb))
                       {
                           ResetEnemy(enemy);
                           entityManager.SetEnabled(projectile, false);
                           isScore = true;
                           scores++;
                           break;
                       }
                           
                   } while (quadrantMap.TryGetNextValue(out enemy, ref mapIterator));
               }
           }).Run();

        if (isScore)
        {
            Entities
             .WithAll<PlayerTag>()
             .WithoutBurst()
             .ForEach((PlayerScore playerScore) =>
             {
                 for (int i = 0; i < scores; i++)
                 {
                     playerScore.IncrementTotalScore();
                 }

             }).Run();
        }

        //AABB collision check (player lose health if hit by enemy)
        //Only check collision on the enemies within the same quadrant.
        Entities
           .WithAll<PlayerTag>()
           .WithStructuralChanges()
           .ForEach((Entity player, PlayerHealth playerHealth, Transform playerTransform, in AABBCollisionComponent playerCollision) => 
           {
               int mapKey = GetPositionHashMapKey(playerTransform.position);
               Entity enemy;
               NativeParallelMultiHashMapIterator<int> mapIterator;
               if (quadrantMap.TryGetFirstValue(mapKey, out enemy, out mapIterator))
               {
                   do
                   {
                       AABBCollisionComponent enemycollision = entityManager.GetComponentData<AABBCollisionComponent>(enemy);
                       if (IsAabbCollision(playerCollision, enemycollision))
                       {
                           ResetEnemy(enemy);

                           playerHealth.TakeDamage();
                           if (playerHealth.health <= 0)
                           {
                               entityManager.SetEnabled(player, false);
                           }
                       }

                   } while (quadrantMap.TryGetNextValue(out enemy, ref mapIterator));
               }
           }).Run();

        quadrantMap.Dispose();
    }

//Place enemy entities int respective qudrant in map. Key based on their pos and the add enemy entity to that value.
    private void PopulateQuadrantMap(NativeParallelMultiHashMap<int, Entity> quadrantMap)
    {
        Entities
          .WithAll<EnemyTag>()
          .ForEach((Entity entity, in Translation pos) => 
          {
              int mapKey = GetPositionHashMapKey(pos.Value);
              quadrantMap.Add(mapKey, entity);
          }).Run();
    }

    private bool IsAabbCollision(AABBCollisionComponent a, AABBCollisionComponent b)
    {
        return (
             a.XMin < b.XMax &&
             a.XMax > b.XMin &&
             a.YMin < b.YMax &&
             a.YMax > b.YMin &&
             a.ZMin < b.ZMax &&
             a.ZMax > b.ZMin);
    }

    private bool IsSphereAABBCollision(SphereCollisionComponent sphere, AABBCollisionComponent aabb)
    {
        float x = Mathf.Max(aabb.XMin, Mathf.Min(sphere.Position.x, aabb.XMax));
        float y = Mathf.Max(aabb.YMin, Mathf.Min(sphere.Position.y, aabb.YMax));
        float z = Mathf.Max(aabb.ZMin, Mathf.Min(sphere.Position.z, aabb.ZMax));

        float distanceToClosestPoint = Mathf.Sqrt(
            (x - sphere.Position.x) * (x - sphere.Position.x) +
            (y - sphere.Position.y) * (y - sphere.Position.y) +
            (z - sphere.Position.z) * (z - sphere.Position.z)
            );

        return distanceToClosestPoint < sphere.Radius;
    }

    private void ResetEnemy(Entity enemy)
    {
        float3 cameraPos = new float3();
        Entities
           .WithAll<CameraTag>()
           .WithoutBurst()
           .ForEach((Transform transform) => { cameraPos.xyz = new float3(transform.position.x, transform.position.y, transform.position.z); }).Run();

        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        EnemySpawnSettingsComponent settings = GetSingleton<EnemySpawnSettingsComponent>();
        Translation enemyPos = entityManager.GetComponentData<Translation>(enemy);

        float xPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
        float yPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
        float zPosition = rand.NextFloat(settings.spawnRadiusZMin, settings.spawnRadiusZMax);
        float randSpeed = rand.NextFloat(settings.minSpeed, settings.maxSpeed);
        enemyPos.Value.x = cameraPos.x + xPosition;
        enemyPos.Value.y = cameraPos.y + yPosition;
        enemyPos.Value.z = cameraPos.z + zPosition;
        entityManager.SetComponentData(enemy, enemyPos);
    }

    //floor the  pos / quadrant size to get a int that is used as the key (convert pos to a int key).
    //(floor rounds down and return the largest int less than or equal to given num).
    private static int GetPositionHashMapKey(float3 position)
    {
        return (int) (Mathf.Floor(position.x / quadrantCellSize) + (quadrantYMultiplier * Mathf.Floor(position.y / quadrantCellSize)) + (quadrantYMultiplier * Mathf.Floor(position.z / quadrantCellSize)));
    }

    private static int GetEntityCountInHashMap(NativeParallelMultiHashMap<int, Entity> quadrantMultiHashMap, int hashMapKey)
    {
        return quadrantMultiHashMap.CountValuesForKey(hashMapKey);
    }

}
