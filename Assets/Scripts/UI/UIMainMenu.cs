using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Image controls;
    [SerializeField] Sprite[] controlSprite;
    int currentControlSprite = 0;
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
        controls.gameObject.SetActive(true);
    }

    public void HideControls()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        controls.gameObject.SetActive(false);
    }

    public void NextControlSprite()
    {
        currentControlSprite++;
        ChangeControlSprite();
    }

    public void LastControlSprite()
    {
        currentControlSprite--;
        ChangeControlSprite();
    }

    void ChangeControlSprite()
    {
        if (currentControlSprite > controlSprite.Length - 1 || currentControlSprite < 0)
        {
            currentControlSprite = 0;
            ChangeControlSprite();
            HideControls();
        }
        else
        {
            controls.sprite = controlSprite[currentControlSprite];
        }
    }
    public void ExitGame()
    {
        audioManager.PlaySound(AudioManager.Sounds.Button);
        Application.Quit();
        Debug.Log("Game Closed");
    }
}