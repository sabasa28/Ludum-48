using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Background: ")]
    [SerializeField] float baseBGCoverAlphaAddition = 0.2f;
    [SerializeField] Sprite background1;
    [SerializeField] Sprite background2;
    [SerializeField] SpriteRenderer backgroundSR;
    [SerializeField] SpriteRenderer backgroundCoverSR;
    [SerializeField] Cover levelCover;

    [Header("Player: ")]
    [SerializeField] int initialPlayerLives = 3;
    [SerializeField] Vector2 playerInitialPosition = Vector2.zero;
    [SerializeField] Player player = null;

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
    List<Enemy> enemies = new List<Enemy>();

    int level = 1;

    public static event Action OnDefeat;
    public static event Action OnVictory;

    void Awake()
    {
        Time.timeScale = 1.0f;
        enemyAmount = baseEnemyAmount;
    }

    void OnEnable()
    {
        Player.OnDeath += Lose;
        Enemy.OnDeath += DestroyEnemy;
        Cover.OnFadedToBlack += SetNewLevel;
        UIManager_Gameplay.OnGameReset += ResetGame;
    }

    void Start()
    {
        SetNewLevel();

        AudioManager.Get().PlayMusic(AudioManager.Songs.Gameplay);
    }

    void OnDisable()
    {
        Player.OnDeath -= Lose;
        Enemy.OnDeath -= DestroyEnemy;
        Cover.OnFadedToBlack -= SetNewLevel;
        UIManager_Gameplay.OnGameReset -= ResetGame;
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

        player.SetLives(initialPlayerLives);
        player.transform.position = playerInitialPosition;

        StartCoroutine(SpawnEnemies());
    }

    void EndLevel()
    {
        if (backgroundCoverSR.color.a == 1.0f) Win();

        level++;
        enemyAmount++;

        levelCover.FadeToBlack();
    }

    void ResetGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    void Lose()
    {
        Time.timeScale = 0.0f;
        player.gameObject.SetActive(false);

        StopCoroutine(SpawnEnemies());

        OnDefeat?.Invoke();
    }

    void Win()
    {
        Time.timeScale = 0.0f;

        OnVictory?.Invoke();
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval));

            Vector2 position = new Vector2(UnityEngine.Random.Range(minEnemySpawnX, maxEnemySpawnX), enemySpawnY);
            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];

            Enemy enemy = Instantiate(prefab, position, Quaternion.identity, enemyContainer).GetComponent<Enemy>();
            enemy.playerTransform = player.transform;
            enemies.Add(enemy);
        }
    }
}