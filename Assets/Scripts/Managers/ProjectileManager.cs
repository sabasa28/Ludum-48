using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    PlayerProjectile[] playerProjectiles;
    EnemyProjectile[] enemyProjectiles;

    [SerializeField] Transform playerProjectileContainer = null;
    [SerializeField] Transform enemyProjectileContainer = null;

    void Awake()
    {
        playerProjectiles = playerProjectileContainer.GetComponentsInChildren<PlayerProjectile>(true);
        enemyProjectiles = enemyProjectileContainer.GetComponentsInChildren<EnemyProjectile>(true);

        Player.OnShoot += ReleasePlayerProjectile;
        LongRangeEnemy.OnShoot += ReleaseEnemyProjectile;
    }

    public void ReleasePlayerProjectile(int charge, float speed, Vector2 position, Vector2 direction)
    {
        foreach (PlayerProjectile projectile in playerProjectiles)
        {
            if (!projectile.gameObject.activeInHierarchy)
            {
                projectile.charge = charge;
                projectile.gameObject.SetActive(true);
                projectile.transform.position = position;
                projectile.transform.localScale *= charge + 1.0f;

                projectile.Fire(speed, direction);
                break;
            }
        }
    }

    public void ReleaseEnemyProjectile(float speed, Vector2 position, Vector2 direction)
    {
        foreach (EnemyProjectile projectile in enemyProjectiles)
        {
            if (!projectile.gameObject.activeInHierarchy)
            {
                projectile.gameObject.SetActive(true);
                projectile.transform.position = position;

                projectile.Fire(speed, direction);
                break;
            }
        }
    }
}