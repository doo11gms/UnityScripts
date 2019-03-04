using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class RayCaster : SerializedMonoBehaviour
{
    public enum CAST_TRIGGER
    {
        LeftMouseButton,
        LeftMouseButtonDown,
        RightMouseButton,
        RightMouseButtonDown
    }

    [Title("Camera")]
    [OdinSerialize, Required] UnityEngine.Camera m_TargetCamera;

    [Title("Settings")]
    [OdinSerialize] CAST_TRIGGER m_CastTrigger;
    [OdinSerialize] List<string> m_IgnoreTags;

    [Title("Info")]
    [OdinSerialize, ReadOnly] GameObject m_HitObject;
    public GameObject HitObject
    {
        get { return m_HitObject; }
    }

    public Vector3? HitPosition { get; private set; }

    void RayCast()
    {
        RaycastHit hit;
        Ray ray = m_TargetCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            foreach (string tag in m_IgnoreTags)
            {
                if (hit.transform.gameObject.tag == tag) return;
            }

            m_HitObject = hit.transform.gameObject;
            HitPosition = hit.point;
        }
    }

    public void Initialize()
    {
        m_HitObject = null;
        HitPosition = null;
    }

    private void Awake()
    {
        if (m_IgnoreTags == null) m_IgnoreTags = new List<string>();
    }

    void Update()
    {
        switch (m_CastTrigger)
        {
            case CAST_TRIGGER.LeftMouseButton:
                if (Input.GetMouseButton(0)) RayCast();
                break;
            case CAST_TRIGGER.LeftMouseButtonDown:
                if (Input.GetMouseButtonDown(0)) RayCast();
                break;
            case CAST_TRIGGER.RightMouseButton:
                if (Input.GetMouseButton(1)) RayCast();
                break;
            case CAST_TRIGGER.RightMouseButtonDown:
                if (Input.GetMouseButtonDown(1)) RayCast();
                break;
        }
    }
}