﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text timeDisplayText;
    public float ResetTime;
    public float timer;
    public GameObject warpEffect;
    private int resetCounter;
    public Text resetCounterText;
    public bool pause;
    public GameObject timeMachineGameObject;
    public Machine timeMachine;

    bool timeWarning;

    public Action OnTimeRunningOut;
    public Action OnReset;

    [ContextMenu("Re-sort All Sprites")]
    public void SortSprites()
    {
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingOrder = Mathf.RoundToInt(sr.gameObject.transform.position.y * 16f) * -1;
        }
    }

    void Start()
    {
        timeMachine = timeMachineGameObject.GetComponent<Machine>();
        timer = ResetTime;
        resetCounter = 0;
        resetCounterText.text = "" + resetCounter + " Resets";
    }

    void Update()
    {
        if (!timeMachine.isBroken)
        {
            // WIN CONDITION
            pause = true;
        }

        if (pause)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 5 && !timeWarning)
        {
            timeWarning = true;
            OnTimeRunningOut?.Invoke();
        }
        else if (timer <= 0)
        {
            StartCoroutine(ResetLoop());
            pause = true;
        }

        timeDisplayText.text = System.TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");
        //timeDisplayText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public void WinCondition()
    {
        pause = true;
    }

    IEnumerator ResetLoop()
    {

        CharacterController2D character = FindObjectOfType<CharacterController2D>();
        //character.Freeze();        
        
        Item[] allItems = FindObjectsOfType<Item>();

        foreach (Item i in allItems)
        {
            if (i.itemSpriteRenderer.isVisible && !i.doNotReset)
            {
                Destroy(Instantiate(warpEffect, i.transform.position, Quaternion.identity), 1f);
                yield return new WaitForSeconds(0.1f);                
                i.ResetState();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                i.ResetState();
            }
        }

        Container[] allContainers = FindObjectsOfType<Container>();

        foreach (Container c in allContainers)
        {
            if (c.spriteRenderer.isVisible)
            {
                Destroy(Instantiate(warpEffect, c.transform.position, Quaternion.identity), 1f);
                //yield return new WaitForSeconds(0.1f);
                c.ResetState();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                c.ResetState();
            }
        }

        Machine[] machines = FindObjectsOfType<Machine>();

        foreach (Machine c in machines)
        {
            if (c.spriteRenderer.isVisible)
            {
                Destroy(Instantiate(warpEffect, c.transform.position, Quaternion.identity), 1f);
                //yield return new WaitForSeconds(0.1f);
                c.ResetState();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                c.ResetState();
            }
        }

        Destroy(Instantiate(warpEffect, character.transform.position, Quaternion.identity), 0.5f);
        yield return new WaitForSeconds(0.2f);

        character.ResetState();
        yield return new WaitForSeconds(0.2f);
        Destroy(Instantiate(warpEffect, character.transform.position, Quaternion.identity), 1f);
        character.UnFreeze();
        timer = ResetTime;

        pause = false;
        OnReset?.Invoke();
        timeWarning = false;
        resetCounter++;
        resetCounterText.text = "" + resetCounter + " Resets";
    }
}
