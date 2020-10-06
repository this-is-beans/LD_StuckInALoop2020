using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffect : MonoBehaviour
{
    public Animator screenFade;

    void Start()
    {
        Game game = FindObjectOfType<Game>();
        if (game != null)
        {
            game.OnTimeRunningOut += FadeInEffects;
            game.OnReset += FadeOutEffects;
        }
    }

    void Update()
    {

    }

    public void FadeInEffects()
    {
        screenFade.SetBool("Fade", true);
    }

    public void FadeOutEffects()
    {
        screenFade.SetBool("Fade", false);
    }
}
