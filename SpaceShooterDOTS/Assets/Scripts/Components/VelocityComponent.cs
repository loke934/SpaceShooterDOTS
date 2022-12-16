using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct VelocityComponent : IComponentData
{
    public Vector3 Direction;
    public float Speed;
}