using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class Shooting : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)] private float shootInterval = 0.2f;
    [SerializeField, Range(0.1f, 2f)] private float projectileSpawnDist = 0.5f;
   
    public List<Entity> projectilePool;
    public EntityManager entityManager;
    public ProjectilePoolSettingsComponent poolSettings;

    private float _timer;

    private void Awake()
    {
        _timer = shootInterval;
        projectilePool = new List<Entity>();
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (_timer <= 0f)
            {
                Shoot();
                _timer = shootInterval;
            }
        }
    }

    void Shoot()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!entityManager.GetEnabled(projectilePool[i]))
            {
                Translation position = new Translation { Value = new float3(transform.position.x, transform.position.y, transform.position.z + transform.localScale.z * projectileSpawnDist)};
                entityManager.SetComponentData(projectilePool[i], position);
                entityManager.SetEnabled(projectilePool[i], true);
                return;
            }
        }
        
        //If all projectiles in pool are active, create a new one.
        IncreasePool();
    }

    void IncreasePool()
    {
        Translation position = new Translation { Value = new float3(transform.position.x, transform.position.y, transform.position.z + transform.localScale.z * projectileSpawnDist) };
        Entity entity = entityManager.Instantiate(poolSettings.projectilePrefab);
        entityManager.SetComponentData(entity, position);
        projectilePool.Add(entity);
    }
}
