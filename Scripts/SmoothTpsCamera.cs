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
    /// もしも目標がその場に留まった場合、その地点に到達するまでに何秒かかるか？
    /// </summary>
    [OdinSerialize, InfoBox("もしも目標がその場に留まった場合、その地点に到達するまでに何秒かかるか？")] float m_ChaseDuration;

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
    /// 目標地点との位置の差を縮めます。
    /// </summary>
    /// <param name="closer">接近させる対象を指定します。</param>
    /// <param name="destination">目標地点を指定します。</param>
    /// <param name="gap">目標地点との位置の差を何割に変更するか、0から1の間で指定します。</param>
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
    /// 強制的にフォーカスします。
    /// </summary>
    [Button("Focus Immediately")]
    public void FocusImmediately()
    {
        if (m_Locked) return;

        transform.LookAt(m_Focus);
    }

    /// <summary>
    /// フォーカスします。
    /// </summary>
    [Button("Focus")]
    public void Focus()
    {
        if (m_Locked) return;

        m_Focusing = true;
    }

    /// <summary>
    /// フォーカスを解除します。
    /// </summary>
    [Button("Unfocus")]
    public void Unfocus()
    {
        if (m_Locked) return;

        m_Focusing = false;
    }

    /// <summary>
    /// 固定カメラモードに切り替えます。
    /// </summary>
    [Button("Fix Mode")]
    public void FixMode()
    {
        if (m_Locked) return;

        m_Fixed = true;
        m_Focusing = true;
    }

    /// <summary>
    /// 固定カメラモードを解除します。
    /// 解除後は、カメラが元の位置に戻る前にフォーカスをオフにすると正常な位置まで戻らないので注意して下さい。
    /// </summary>
    [Button("Unfix Mode")]
    public void UnfixMode()
    {
        if (m_Locked) return;

        m_Fixed = false;
        m_Focusing = true;
    }

    /// <summary>
    /// カメラをロックします。
    /// </summary>
    [Button("Lock")]
    public void Lock()
    {
        m_Locked = true;
    }

    /// <summary>
    /// カメラのロックを解除します。
    /// </summary>
    [Button("Unlock")]
    public void Unlock()
    {
        m_Locked = false;
    }

    #endregion
}
