using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Fadeout : SerializedMonoBehaviour
{
    [Title("Settings")]
    [OdinSerialize] bool m_SelfDeactivate = true;
    [OdinSerialize] float m_Duration = 2f;

    [Title("Callback")]
    [OdinSerialize] UnityEngine.Events.UnityEvent m_OnDurationElapsed = new UnityEngine.Events.UnityEvent();

    Image m_Image;
    float m_ElapsedTime;

    void UpdateAlpha()
    {
        var buffer = m_Image.color;
        buffer.a = 1f - m_ElapsedTime / m_Duration;
        m_Image.color = buffer;
    }

    private void OnEnable()
    {
        m_ElapsedTime = 0f;
        UpdateAlpha();
    }

    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    private void Update()
    {
        if (m_ElapsedTime >= m_Duration)
        {
            m_ElapsedTime = m_Duration;
            if (m_SelfDeactivate) gameObject.SetActive(false);
            m_OnDurationElapsed?.Invoke();

            return;
        }

        m_ElapsedTime += Time.deltaTime;
        UpdateAlpha();
    }
}