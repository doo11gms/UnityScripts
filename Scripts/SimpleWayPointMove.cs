using UnityEngine;
using System.Collections.Generic;

public class SimpleWayPointMove : MonoBehaviour
{
    #region

    enum STATE
    {
        Idling,
        Walking,
        Running
    }

    enum RAYCAST_TRIGGER
    {
        LeftMouseButton,
        LeftMouseButtonDown,
        RightMouseButton,
        RightMouseButtonDown
    }

    #endregion

    [Header("Required")]
    [SerializeField] CharacterController m_CharacterController;

    [Header("Settings")]
    [SerializeField] float m_StoppingDistance = 1f;
    [SerializeField] List<KeyCode> m_WalkingTriggers = new List<KeyCode> { KeyCode.LeftShift, KeyCode.RightShift };
    [SerializeField] float m_WalkingSpeed = 0.1f;
    [SerializeField] float m_RunningSpeed = 0.3f;

    [Header("Animation")]
    [SerializeField] Animator m_Animator;
    [SerializeField] string m_RunningAnimationName;
    [SerializeField] string m_WalkingAnimationName;

    [Header("Raycast")]
    [SerializeField] Camera m_RaycastingCamera;
    [SerializeField] RAYCAST_TRIGGER m_RaycastTrigger = RAYCAST_TRIGGER.LeftMouseButton;
    [SerializeField] List<string> m_IgnoreTags = new List<string>();
    Vector3? m_HitPosition;

    [Header("Info")]
    [SerializeField] STATE m_State;

    #region functions

    Vector3 Destination()
    {
        return m_HitPosition == null ? m_CharacterController.transform.position : (Vector3)m_HitPosition;
    }

    Vector3 MovingDirection()
    {
        var direction = Destination() - m_CharacterController.transform.position;
        direction.y = 0f;

        return direction.normalized;
    }

    float HorizontalDistance(Vector3 arg1, Vector3 arg2)
    {
        arg1.y = 0f;
        arg2.y = 0f;

        return Vector3.Distance(arg1, arg2);
    }

    bool HasReachedDestination()
    {
        return HorizontalDistance(Destination(), m_CharacterController.transform.position) <= m_StoppingDistance;
    }

    #endregion

    void RayCast()
    {
        RaycastHit hit;
        Ray ray = m_RaycastingCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            foreach (string tag in m_IgnoreTags)
            {
                if (hit.transform.gameObject.tag == tag) return;
            }

            m_HitPosition = hit.point;
        }
    }

    void LookAtDestination()
    {
        var lookedAt = Destination();
        lookedAt.y = m_CharacterController.transform.position.y;

        m_CharacterController.transform.LookAt(lookedAt);
    }

    private void FixedUpdate()
    {
        switch (m_State)
        {
            case STATE.Idling:
                if (m_Animator == null) break;
                m_Animator.SetBool(m_WalkingAnimationName, false);
                m_Animator.SetBool(m_RunningAnimationName, false);
                break;
            case STATE.Walking:
                LookAtDestination();
                m_CharacterController.Move(MovingDirection() * m_WalkingSpeed);
                if (m_Animator == null) break;
                m_Animator.SetBool(m_WalkingAnimationName, true);
                m_Animator.SetBool(m_RunningAnimationName, false);
                break;
            case STATE.Running:
                LookAtDestination();
                m_CharacterController.Move(MovingDirection() * m_RunningSpeed);
                if (m_Animator == null) break;
                m_Animator.SetBool(m_WalkingAnimationName, false);
                m_Animator.SetBool(m_RunningAnimationName, true);
                break;
        }
    }

    private void Update()
    {
        switch (m_RaycastTrigger)
        {
            case RAYCAST_TRIGGER.LeftMouseButton:
                if (Input.GetMouseButton(0)) RayCast();
                break;
            case RAYCAST_TRIGGER.LeftMouseButtonDown:
                if (Input.GetMouseButtonDown(0)) RayCast();
                break;
            case RAYCAST_TRIGGER.RightMouseButton:
                if (Input.GetMouseButton(1)) RayCast();
                break;
            case RAYCAST_TRIGGER.RightMouseButtonDown:
                if (Input.GetMouseButtonDown(1)) RayCast();
                break;
        }

        if (HasReachedDestination())
        {
            m_State = STATE.Idling;
        }
        else
        {
            m_State = STATE.Running;
            m_WalkingTriggers.ForEach(trigger =>
            {
                if (Input.GetKey(trigger)) m_State = STATE.Walking;
            });
        }
    }
}