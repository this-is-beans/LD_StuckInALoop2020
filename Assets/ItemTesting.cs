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

    public void OpenAllContainers()
    {

    }

    public void ResetLoop()
    {

    }
}
