using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class SingletonBehaviour : SerializedMonoBehaviour
{
    public static GameObject Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}