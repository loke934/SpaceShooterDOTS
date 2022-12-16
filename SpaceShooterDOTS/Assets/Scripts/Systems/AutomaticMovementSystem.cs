using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
public partial class AutomaticMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<AutomaticMovementTag>()
            .ForEach((ref Translation position, in VelocityComponent velocity) =>
            {
                float3 displacement = velocity.Direction * velocity.Speed * deltaTime;
                position.Value += displacement;

            }).ScheduleParallel();
    }
}

