using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text timeDisplayText;
    public float ResetTime;
    float timer;
    public GameObject warpEffect;

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
            StartCoroutine (ResetLoop());
            timer = ResetTime;
            pause = true;
        }

        timeDisplayText.text = System.TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\.ff");
        //timeDisplayText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    IEnumerator ResetLoop()
    {
        Item[] allItems = FindObjectsOfType<Item>();

        foreach (Item i in allItems)
        {
            Destroy(Instantiate(warpEffect, i.transform.position, Quaternion.identity), 1f);
            yield return new WaitForSeconds(0.1f);

            i.ResetState();
            yield return new WaitForSeconds(0.1f);
        }

        Container[] allContainers = FindObjectsOfType<Container>();

        foreach (Container c in allContainers)
        {
            Destroy(Instantiate(warpEffect, c.transform.position, Quaternion.identity), 1f);
            yield return new WaitForSeconds(0.1f);

            c.ResetState();
            yield return new WaitForSeconds(0.1f);
        }

        Machine[] machines = FindObjectsOfType<Machine>();

        foreach (Machine c in machines)
        {
            Destroy(Instantiate(warpEffect, c.transform.position, Quaternion.identity), 1f);
            yield return new WaitForSeconds(0.1f);

            c.ResetState();
            yield return new WaitForSeconds(0.1f);
        }

        pause = false;
    }
}
