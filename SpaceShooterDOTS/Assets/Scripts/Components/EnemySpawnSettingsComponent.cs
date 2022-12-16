using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemySpawnSettingsComponent : IComponentData
{
    public int NumberOfEnemies;
    public float spawnRadiusXY;
    public float spawnRadiusZMin;
    public float spawnRadiusZMax;
    public float minSpeed;
    public float maxSpeed;
    public Entity EnemyPrefab;
}
