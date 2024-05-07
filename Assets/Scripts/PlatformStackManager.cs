using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class PlatformStackManager : MonoBehaviour//this should be a singleton
{
    public static PlatformStackManager instance;

    Stack<GameObject> normalsStack;

    Stack<GameObject> breakablesStack;

    [SerializeField]
    float platformSurfaceArc;

    [SerializeField]
    GameObject[] normalPlatforms;

    [SerializeField]
    GameObject[] breakablePlatforms;

    [SerializeField]
    int normalPoolSize, breakablePoolSize;

    public System.Action platformRecall;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }

        normalsStack = new Stack<GameObject>();
        breakablesStack = new Stack<GameObject>();
        for(int i = 0; i < normalPoolSize; i++)
        {
            normalsStack.Push(Instantiate(normalPlatforms[Random.Range(0, 2)]));
            normalsStack.Peek().GetComponent<Platform>().SetMom(normalsStack);
            normalsStack.Peek().GetComponent<Platform>().SetBreakable(false);
            normalsStack.Peek().GetComponent<PlatformEffector2D>().surfaceArc = platformSurfaceArc;
        }
        for (int i = 0; i < breakablePoolSize; i++)
        {
            breakablesStack.Push(Instantiate(breakablePlatforms[Random.Range(0, 2)]));
            breakablesStack.Peek().GetComponent<Platform>().SetMom(breakablesStack);
            breakablesStack.Peek().GetComponent<Platform>().SetBreakable(true);
            breakablesStack.Peek().GetComponent<PlatformEffector2D>().surfaceArc = platformSurfaceArc;
        }
    }

    public GameObject GetPlatform(bool breakable)
    {
        GameObject platform = breakable ? breakablesStack.Pop() : normalsStack.Pop();
        //sub to the platformcall event as well.
        platformRecall += platform.GetComponent<Platform>().BackToStack;
        return platform;
    }

    public float GetPlatformSize()
    {
        return normalPlatforms[0].GetComponent<BoxCollider2D>().size.x * normalPlatforms[0].transform.localScale.x;
    }

    public void RecallPlatforms()
    {
        platformRecall?.Invoke();
    }
}
