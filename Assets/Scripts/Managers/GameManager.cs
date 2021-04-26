using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform = null;
    [SerializeField] Enemy[] enemies = null;

    void Awake()
    {
        foreach (Enemy enemy in enemies) enemy.playerTransform = playerTransform;
    }
}