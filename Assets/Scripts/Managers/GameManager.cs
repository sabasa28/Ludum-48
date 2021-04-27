using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Background: ")]
    [SerializeField] float baseBGCoverAlphaAddition = 0.1f;
    [SerializeField] Sprite background2;
    [SerializeField] SpriteRenderer backgroundSR;
    [SerializeField] SpriteRenderer backgroundCoverSR;
    [SerializeField] Cover levelCover;

    [Header("Player: ")]
    [SerializeField] Vector2 playerInitialPosition = Vector2.zero;
    [SerializeField] Transform playerTransform = null;

    [Header("Enemies: ")]
    [SerializeField] float minEnemySpawnInterval;
    [SerializeField] float maxEnemySpawnInterval;
    [SerializeField] float enemySpawnY;
    [SerializeField] float minEnemySpawnX;
    [SerializeField] float maxEnemySpawnX;

    [SerializeField] int baseEnemyAmount = 3;
    [SerializeField] Transform enemyContainer;
    int enemyAmount;

    [SerializeField] GameObject[] enemyPrefabs = null;
    List<Enemy> enemies;

    int level = 1;

    public static event Action OnDefeat;

    void Awake()
    {
        enemyAmount = baseEnemyAmount;

        Player.OnDeath += Lose;
        Enemy.OnDeath += DestroyEnemy;
        Cover.OnFadedToBlack += SetNewLevel;
    }

    void Start()
    {
        SetNewLevel();

        AudioManager.Get().PlayMusic(AudioManager.Songs.Gameplay);
    }

    void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
        enemies.Remove(enemy);

        if (enemies.Count == 0) EndLevel();
    }

    void SetNewLevel()
    {
        if (level > 1)
        {
            if (backgroundSR.sprite != background2) backgroundSR.sprite = background2;

            if (level > 2)
            {
                Color backgroundCoverColor = backgroundCoverSR.color;
                backgroundCoverColor.a += baseBGCoverAlphaAddition;
                backgroundCoverSR.color = backgroundCoverColor;
            }
        }

        playerTransform.position = playerInitialPosition;

        enemies = new List<Enemy>();
        StartCoroutine(SpawnEnemies());
    }

    void EndLevel()
    {
        level++;
        enemyAmount++;

        levelCover.FadeToBlack();
    }

    void Lose()
    {

    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval));

            Vector2 position = new Vector2(UnityEngine.Random.Range(minEnemySpawnX, maxEnemySpawnX), enemySpawnY);
            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];

            Enemy enemy = Instantiate(prefab, position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
            enemy.playerTransform = playerTransform;
            enemies.Add(enemy);
        }
    }
}