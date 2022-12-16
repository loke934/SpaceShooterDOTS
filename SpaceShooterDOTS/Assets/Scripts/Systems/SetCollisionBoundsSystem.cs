using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(QuadrantCollisionCheckingSystem))]
public partial class SetCollisionBoundsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
             .WithoutBurst()
             .WithAll<PlayerTag>()
             .ForEach((ref AABBCollisionComponent collision, in Transform transform) =>
             {
                 float size = transform.localScale.x * 0.5f; 
                 collision.XMin = transform.position.x - size;
                 collision.YMin = transform.position.y - size;
                 collision.ZMin = transform.position.z - size;

                 collision.XMax = transform.position.x + size;
                 collision.YMax = transform.position.y + size;
                 collision.ZMax = transform.position.z + size;

             }).Run();

        Entities
            .WithAll<EnemyTag>()
            .ForEach((ref AABBCollisionComponent collision, ref Translation position) =>
            {
                float size = 1f / 2f;

                collision.XMin = position.Value.x - size;
                collision.YMin = position.Value.y - size;
                collision.ZMin = position.Value.z - size;

                collision.XMax = position.Value.x + size;
                collision.YMax = position.Value.y + size;
                collision.ZMax = position.Value.z + size;


            }).ScheduleParallel();

        Entities
           .ForEach((ref SphereCollisionComponent sphere, in Translation position) =>
           {
               sphere.Position.x = position.Value.x;
               sphere.Position.y = position.Value.y;
               sphere.Position.z = position.Value.z;

           }).ScheduleParallel();
    }
}