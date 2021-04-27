using System;
using UnityEngine;

public class Cover : MonoBehaviour
{
    Animator animator;

    public static event Action OnFadedToBlack;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected void OnCoverBlack()
    {
        OnFadedToBlack?.Invoke();
    }

    public void FadeToBlack()
    {
        animator.SetTrigger("Fade To Black");
    }
}