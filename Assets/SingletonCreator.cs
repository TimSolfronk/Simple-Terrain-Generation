using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SingletonCreator
{
    public static MonoBehaviour CreateSingleton(MonoBehaviour instanceVar, MonoBehaviour thisInstance)
    {
        if (instanceVar != null && instanceVar != thisInstance)
        {
            Object.Destroy(thisInstance.gameObject);
            return instanceVar;
        }
        else
        {
            return thisInstance;
        }
    }
}
