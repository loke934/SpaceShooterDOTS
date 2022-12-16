# SpaceShooterDOTS
## A prototype space shooter game developed in Unity DOTS 0.51 for a school assignment with a focus on performance. 
The game features simple movement using the **_WASD_** keys and shooting using **_mouse click_**, and allows the player to quit using the **_ESC_** key. 

The project uses a hybrid approach, with the most performance-critical systems implemented in ECS and necessary data from Monobehaviours converted to components using the "convert and inject game object" script. For example, shooting input is handled by a Monobehaviour script, but the projectiles themselves are entities managed by ECS. 

<img src="https://user-images.githubusercontent.com/76095991/208082286-faa89aad-8aeb-4598-8f36-3fb8246792ff.png"  width="20%" height="10%"> 
The player game object has a child entity with entity components and converted Monobehaviours that are used by ECS.

### The game features the following DOTS systems:

 **Spawning and continuous movement**
 
Enemies and projectiles are created at the start of the game and have a object pool like behaviour, with enemies changing position if they are hit or not visible, and projectiles being disabled on collision or if they go out of sight. When shooting, the first inactive projectile is enabled and given a new position to "spawn" at. New projectiles are only instantiated if all projectiles in the pool are active. These systems are implemented by:

- the [EnemySpawnSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/EnemySpawnSystem.cs) (using the [EnemySpawnSettingsComponent](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Components/EnemySpawnSettingsComponent.cs) for data), 
- the [ProjectileSpawnSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/ProjectileSpawnSystem.cs) (using the [ProjectilePoolSettingsComponent](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Components/ProjectilePoolSettingsComponent.cs) for data), 
- the [AutomaticMovementSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/AutomaticMovementSystem.cs) (using the [Velocity Component](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Components/VelocityComponent.cs) for data),  
- the [WithinSightSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/WithinSightSystem.cs) (using the EnemySpawnSettingsComponent, Transform, and Translation components for data).
 
 **Collision**
 
Entities with collision have either an [AABBCollisionComponent](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Components/AABBCollisionComponent.cs) or a [SphereCollisionComponent](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Components/SphereCollisionComponent.cs), and the [SetCollisionBoundsSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/SetCollisionBoundsSystem.cs) is responsible for updating the collision bounds values in the components. Collision checking was initially done separately for AABB/AABB and AABB/Sphere with no sorting, which led to poor performance as the number of enemy entities increased. Attempts were made to try and reduce the number of collision checks per frame by using shared components and distance checking. However, these approaches either required significant and frequent structural changes or did not decrease the number of iterations over entities, resulting in no improvement.

To improve performance, a [QuadrantCollisionCheckingSystem](https://github.com/loke934/SpaceShooterDOTS/blob/master/SpaceShooterDOTS/Assets/Scripts/Systems/QuadrantCollisionCheckingSystem.cs) was implemented, with entities checking for collision only against other entities within a square area around them. Squares and their entities are stored in a native parallel multi hash map, and determining which square an entity belongs to is based on its world position and the size of the squares. This reduced the number of collision checks and improved performance even at high enemy counts (5000, 10,000, and 50,000). The 10,000 enemy entities was used in the build.

<img src="https://user-images.githubusercontent.com/76095991/208101886-e72c288a-7471-4946-a045-1f60d6c2659f.png"  width="40%" height="40%"> <img src="https://user-images.githubusercontent.com/76095991/208102195-176b5ab3-3f9b-4387-9e37-060de2beb125.png"  width="40%" height="40%"> <img src="https://user-images.githubusercontent.com/76095991/208102568-aa63cd4d-37a8-4f24-a5d7-c8087151a2ae.png"  width="40%" height="40%"> 


