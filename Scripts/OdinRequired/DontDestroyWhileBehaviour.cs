using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.SceneManagement;

public class DontDestroyWhileBehaviour : SerializedMonoBehaviour
{
    [OdinSerialize] int m_DestroyTriggerScenesCount = 3;

    [OdinSerialize, ReadOnly] int m_LoadedScenesCountInfo = 0;
    static int m_LoadedScenesCount = 0;

    void UpdateLoadedScenesCountInfo()
    {
        m_LoadedScenesCountInfo = m_LoadedScenesCount;
    }

    private void Awake()
    {
        if (m_LoadedScenesCount == 0) SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        m_LoadedScenesCount++;

        UpdateLoadedScenesCountInfo();

        if (m_LoadedScenesCount >= m_DestroyTriggerScenesCount)
        {
            Debug.Assert(gameObject != null);
            Destroy(gameObject);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
