# SpaceShooterDOTS
## Prototype space shooter made in Unity DOTS 0.51 as a school assignment focusing on performance.

Simple movement for player in x-axis and y-axis with **_WASD_** and shooting with **_mousecklick_**. **_ESC_** for quit.

The project has a hybrid approach, focusing on having the most performant-heavy systems in ECS and converting necessary data from Monobehaviours to components if needed, using convert and inject game object. For example the shooting input is handled by a Monobehaviour script but using the projectiles enteties that are handled by ECS.

<img src="https://user-images.githubusercontent.com/76095991/208082286-faa89aad-8aeb-4598-8f36-3fb8246792ff.png"  width="20%" height="10%"> *Player game object has a player entity child with entity components and converted Monobehaviours used by ECS.*

## DOTS systems:

 **Spawning and continious movement**
Creating all enteties on start and using a pool like behaviour where enemies change position if they are hit or not visible, rather than doing structual changes like being instantiated, destroyed or set to disabled. Projectiles are diabled on collision or if they are out of sight. When shooting, the first projectile not enabled will be set to enabled and get a new position to "spawn" at. A new projectile will only be instatiated if all projectiles in the pool are enabled.
 
 Systems handeling this are divided in:
 - EnemySpawnSystem (using the EnemySpawnSettingsComponent to get data)
 - ProjectileSpawnSystem (using the ProjectilePoolSettingsComponent to get data)
 - AutomaticMovementSystem (using the Velocity Component for data)
 - WithinSightSystem (using the EnemySpawnSettingsComponent,Transform, and Translation components for data).
 
 **Collison**
 Enteties with collision have either a AABBCollisionComponent or a SphereCollisionComponent. The SetCollisionBoundsSystem is responsible for updating the values for both  components. It is set to update befor the collision checking. 
 Collision checking was initially done separate for AABB/AABB and AABB/Sphere with no sorting, which meant collision checking for all enabled enteties. When increasing the amount of enemy enteties the checking was not scalable and fps would drop to <20 fps when several projectiles were enabled. 
 An 
 
 



