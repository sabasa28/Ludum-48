using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager_Gameplay : MonoBehaviour
{
    [SerializeField] GameObject defeatMenu;
    [SerializeField] GameObject victoryMenu;
    [SerializeField] GameObject exitButton;

    public static event Action OnGameReset;

    void OnEnable()
    {
        GameManager.OnDefeat += ShowDefeatMenu;
        GameManager.OnVictory += ShowVictoryMenu;
    }

    void OnDisable()
    {
        GameManager.OnDefeat -= ShowDefeatMenu;
        GameManager.OnVictory -= ShowVictoryMenu;
    }

    void ShowDefeatMenu()
    {
        defeatMenu.SetActive(true);
        exitButton.SetActive(true);
    }

    void ShowVictoryMenu()
    {
        victoryMenu.SetActive(true);
        exitButton.SetActive(true);
    }

    public void ResetGame()
    {
        defeatMenu.SetActive(false);
        victoryMenu.SetActive(false);
        exitButton.SetActive(false);

        OnGameReset?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }
}