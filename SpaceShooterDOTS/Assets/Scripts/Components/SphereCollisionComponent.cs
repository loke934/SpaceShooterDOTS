using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SphereCollisionComponent : IComponentData
{
    public float Radius;
    public float3 Position;
}
