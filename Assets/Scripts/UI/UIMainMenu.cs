using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] GameObject credits;
    [SerializeField] GameObject controls;

    AudioManager audioManager;

    void Awake()
    {
        audioManager = AudioManager.Get();
    }

    void Start()
    {
        audioManager.PlayMusic(AudioManager.Songs.MainMenu);
    }

    public void LoadGameplay()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        SceneManager.LoadScene("Gameplay");
    }

    public void ShowControls()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        controls.SetActive(true);
    }

    public void HideControls()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        controls.SetActive(false);
    }

    public void ExitGame()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        Application.Quit();
        Debug.Log("Game Closed");
    }
}