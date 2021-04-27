using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lives : MonoBehaviour
{
    [SerializeField] GameObject[] lifeImage;
    int livesDisplayed;

    private void Start()
    {
        livesDisplayed = lifeImage.Length + 1;
    }
    public void UpdateLives(int livesLeft)
    {
        if (livesLeft != livesDisplayed)
        {
            for (int i = 0; i < lifeImage.Length; i++)
            {
                lifeImage[i].gameObject.SetActive(i < livesLeft);
            }
        }
    }
}
