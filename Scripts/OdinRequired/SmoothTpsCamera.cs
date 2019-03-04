using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[RequireComponent(typeof(UnityEngine.Camera))]
public class SmoothTpsCamera : SerializedMonoBehaviour
{
    [Title("Focus")]
    [OdinSerialize] Transform m_Focus;

    [Title("Chase")]
    [OdinSerialize] Transform m_Chased;
    [OdinSerialize] AnimationCurve m_ChaseAccelerationCurve;

    /// <summary>
    /// �������ڕW�����̏�ɗ��܂����ꍇ�A���̒n�_�ɓ��B����܂łɉ��b�����邩�H
    /// </summary>
    [OdinSerialize, InfoBox("�������ڕW�����̏�ɗ��܂����ꍇ�A���̒n�_�ɓ��B����܂łɉ��b�����邩�H")] float m_ChaseDuration;

    [Title("Zoom")]
    [OdinSerialize] Vector3 m_MaxZoomAmount = new Vector3(5f, 5f, 5f);

    [Title("Read Only")]
    [OdinSerialize, ReadOnly] bool m_Locked = false;
    [OdinSerialize, ReadOnly] bool m_Fixed = false;
    [OdinSerialize, ReadOnly] bool m_Focusing = false;
    [OdinSerialize, ReadOnly] Vector3 m_ZoomedAmount;

    Vector3 m_InitialOffset;

    #region Functions

    Vector3 Offset() => transform.position - m_Chased.position;
    Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        Vector3 lerped;
        lerped.x = Mathf.Lerp(a.x, b.x, t);
        lerped.y = Mathf.Lerp(a.y, b.y, t);
        lerped.z = Mathf.Lerp(a.z, b.z, t);

        return lerped;
    }

    #endregion

    /// <summary>
    /// �ڕW�n�_�Ƃ̈ʒu�̍����k�߂܂��B
    /// </summary>
    /// <param name="closer">�ڋ߂�����Ώۂ��w�肵�܂��B</param>
    /// <param name="destination">�ڕW�n�_���w�肵�܂��B</param>
    /// <param name="gap">�ڕW�n�_�Ƃ̈ʒu�̍��������ɕύX���邩�A0����1�̊ԂŎw�肵�܂��B</param>
    void CloseGap(Transform closer, Vector3 destination, float gap)
    {
        closer.position = Lerp(transform.position, destination, gap);
    }

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (m_Locked) return;
        if (!m_Fixed) CloseGap(transform, m_Chased.position + m_InitialOffset + m_ZoomedAmount, Time.deltaTime / m_ChaseDuration);
        if (m_Focusing) FocusImmediately();
    }

    #region Buttons

    [Button("Initialize")]
    public void Initialize()
    {
        m_InitialOffset = Offset();
        m_ZoomedAmount = Vector3.zero;
        Unlock();
        FocusImmediately();
    }

    [Button("Zoom")]
    public bool Zoom(float amount = 1f)
    {
        if (m_Locked) return false;

        var adder = transform.forward * amount;

        if (Mathf.Abs(m_ZoomedAmount.x + adder.x) > Mathf.Abs(m_MaxZoomAmount.x)) return false;
        if (Mathf.Abs(m_ZoomedAmount.y + adder.y) > Mathf.Abs(m_MaxZoomAmount.y)) return false;
        if (Mathf.Abs(m_ZoomedAmount.z + adder.z) > Mathf.Abs(m_MaxZoomAmount.z)) return false;

        m_ZoomedAmount += adder;

        return true;
    }

    /// <summary>
    /// �����I�Ƀt�H�[�J�X���܂��B
    /// </summary>
    [Button("Focus Immediately")]
    public void FocusImmediately()
    {
        if (m_Locked) return;

        transform.LookAt(m_Focus);
    }

    /// <summary>
    /// �t�H�[�J�X���܂��B
    /// </summary>
    [Button("Focus")]
    public void Focus()
    {
        if (m_Locked) return;

        m_Focusing = true;
    }

    /// <summary>
    /// �t�H�[�J�X���������܂��B
    /// </summary>
    [Button("Unfocus")]
    public void Unfocus()
    {
        if (m_Locked) return;

        m_Focusing = false;
    }

    /// <summary>
    /// �Œ�J�������[�h�ɐ؂�ւ��܂��B
    /// </summary>
    [Button("Fix Mode")]
    public void FixMode()
    {
        if (m_Locked) return;

        m_Fixed = true;
        m_Focusing = true;
    }

    /// <summary>
    /// �Œ�J�������[�h���������܂��B
    /// ������́A�J���������̈ʒu�ɖ߂�O�Ƀt�H�[�J�X���I�t�ɂ���Ɛ���Ȉʒu�܂Ŗ߂�Ȃ��̂Œ��ӂ��ĉ������B
    /// </summary>
    [Button("Unfix Mode")]
    public void UnfixMode()
    {
        if (m_Locked) return;

        m_Fixed = false;
        m_Focusing = true;
    }

    /// <summary>
    /// �J���������b�N���܂��B
    /// </summary>
    [Button("Lock")]
    public void Lock()
    {
        m_Locked = true;
    }

    /// <summary>
    /// �J�����̃��b�N���������܂��B
    /// </summary>
    [Button("Unlock")]
    public void Unlock()
    {
        m_Locked = false;
    }

    #endregion
}
