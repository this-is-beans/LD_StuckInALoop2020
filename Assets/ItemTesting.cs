using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTesting : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BreakAllItems()
    {
        Item[] allItems = FindObjectsOfType<Item>();

        foreach(Item i in allItems)
        {
            i.Deplete(10);
            i.GetComponent<BounceBehaviour>().Throw(new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
        }
    }

    public void RestoreAllItems()
    {
        Item[] allItems = FindObjectsOfType<Item>();

        foreach (Item i in allItems)
        {
            i.Charge(10);
        }
    }

    public void BashContainers()
    {
        Container[] allContainers = FindObjectsOfType<Container>();

        foreach (Container c in allContainers)
        {
            c.Bash(Random.Range(0,3));
        }
    }

    public void OpenAllContainers()
    {
        Container[] allContainers = FindObjectsOfType<Container>();

        foreach (Container c in allContainers)
        {
            c.ForceOpen();
        }
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
    }
}
