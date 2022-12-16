using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class EnemySpawnSystem : SystemBase
{
    private EntityQuery enemyQuery;
    private BeginSimulationEntityCommandBufferSystem beginSimECB;
    private Entity prefab;

    protected override void OnCreate()
    {
        enemyQuery = GetEntityQuery(ComponentType.ReadWrite<EnemyTag>());
        beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<EnemySpawnSettingsComponent>().EnemyPrefab;
        }

        EnemySpawnSettingsComponent settings = GetSingleton<EnemySpawnSettingsComponent>();
        EntityCommandBuffer commandBuffer = beginSimECB.CreateCommandBuffer(); 
        int count = enemyQuery.CalculateEntityCountWithoutFiltering();
        Entity enemyPrefab = prefab;
        Random rand = new Random((uint)Stopwatch.GetTimestamp());

        float3 cameraPos = new float3();

        Entities
           .WithAll<CameraTag>()
           .WithoutBurst()
           .ForEach((UnityEngine.Transform transform) => { cameraPos.xyz = new float3(transform.position.x, transform.position.y, transform.position.z); }).Run();

        Job
        .WithCode(() => {
            for (int i = count; i < settings.NumberOfEnemies; ++i)
            {
                float xPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
                float yPosition = rand.NextFloat(-settings.spawnRadiusXY, settings.spawnRadiusXY);
                float zPosition = rand.NextFloat(settings.spawnRadiusZMin, settings.spawnRadiusZMax);
                float randSpeed = rand.NextFloat(settings.minSpeed, settings.maxSpeed);
                
                Translation position = new Translation { Value = new float3(xPosition, yPosition, zPosition) };
                position.Value.xyz += cameraPos;
               
                VelocityComponent velocity = new VelocityComponent();
                velocity.Direction = new float3(0f, 0f, -1f);
                velocity.Speed = randSpeed;

                Entity entity = commandBuffer.Instantiate(enemyPrefab);
                commandBuffer.SetComponent(entity, position);
                commandBuffer.SetComponent(entity, velocity);

            }
        }).Schedule();

        beginSimECB.AddJobHandleForProducer(Dependency);
    }
}
