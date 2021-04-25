using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    Projectile[] playerProjectiles;
    Projectile[] enemyProjectiles;

    [SerializeField] Transform playerProjectileContainer = null;
    [SerializeField] Transform enemyProjectileContainer = null;

    void Awake()
    {
        playerProjectiles = playerProjectileContainer.GetComponentsInChildren<Projectile>(true);
        enemyProjectiles = enemyProjectileContainer.GetComponentsInChildren<Projectile>(true);

        Player.OnShoot += ReleasePlayerProjectile;
        LongRangeEnemy.OnShoot += ReleaseEnemyProjectile;
    }

    public void ReleasePlayerProjectile(float scaleMultiplier, float speed, Vector2 position, Vector2 direction)
    {
        foreach (Projectile projectile in playerProjectiles)
        {
            if (!projectile.gameObject.activeInHierarchy)
            {
                projectile.gameObject.SetActive(true);
                projectile.transform.position = position;
                projectile.transform.localScale *= scaleMultiplier;

                projectile.Fire(speed, direction);
                break;
            }
        }
    }

    public void ReleaseEnemyProjectile(float speed, Vector2 position, Vector2 direction)
    {
        foreach (Projectile projectile in enemyProjectiles)
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