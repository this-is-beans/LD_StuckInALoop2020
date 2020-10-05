using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffect : MonoBehaviour
{
    public Animator screenFade;
    public Game game;


    void Start()
    {
        
    }

    void Update()
    {
        if (game.timer < 5)
        {
            screenFade.SetBool("Fade", true);
        } else
        {
            screenFade.SetBool("Fade", false);
        }
    }
}
