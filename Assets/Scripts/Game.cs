using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text timeDisplayText;
    public float ResetTime;
    float timer;

    public bool pause;

    void Start()
    {
        timer = ResetTime;
    }

    void Update()
    {
       if (pause)
        {
            return;
        }


        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ResetLoop();
            timer = ResetTime;
        }

        timeDisplayText.text = System.TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\.ff");
        //timeDisplayText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public void ResetLoop()
    {
        Container[] allContainers = FindObjectsOfType<Container>();

        foreach (Container c in allContainers)
        {
            c.ResetState();
        }

        Item[] allItems = FindObjectsOfType<Item>();

        foreach (Item i in allItems)
        {
            i.ResetState();
        }

        Machine[] machines = FindObjectsOfType<Machine>();

        foreach (Machine c in machines)
        {
            c.ResetState();
        }
    }
}
