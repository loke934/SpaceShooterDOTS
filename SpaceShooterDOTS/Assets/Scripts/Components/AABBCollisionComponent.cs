using Unity.Entities;

[GenerateAuthoringComponent]
public struct AABBCollisionComponent : IComponentData
{
    public float XMin;
    public float YMin;
    public float ZMin;

    public float XMax;
    public float YMax;
    public float ZMax;
}
