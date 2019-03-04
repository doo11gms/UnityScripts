using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TpsCamera : SerializedMonoBehaviour
{
    [Title("Settings")]
    [OdinSerialize] bool m_IsChasingPlayer = true;
    [OdinSerialize] FreeLookSettings m_FreeLookSettings = new FreeLookSettings();
    [OdinSerialize] ZoomSettings m_ZoomSettings = new ZoomSettings();

    [Title("Reference")]
    [OdinSerialize, Required] Transform m_CenterOfPlayer;

    Vector3 m_InitialPosition;
    Vector3 m_LatestCenterPositionOfPlayer;

    /// <summary>
    /// カメラの位置関係を初期化します。
    /// </summary>
    public void Initialize()
    {
        transform.position = m_InitialPosition;
        transform.LookAt(m_CenterOfPlayer.position);
        m_LatestCenterPositionOfPlayer = m_CenterOfPlayer.position;
    }

    void RatateAroundTarget()
    {
        if (!Input.GetMouseButton(1)) return;

        float horizontalDelta = Input.GetAxis("Mouse X");
        float verticalDelta = Input.GetAxis("Mouse Y");

        transform.RotateAround(m_CenterOfPlayer.position, Vector3.up, horizontalDelta * m_FreeLookSettings.FreeLookSpeed);
        transform.RotateAround(m_CenterOfPlayer.position, -transform.right, verticalDelta * m_FreeLookSettings.FreeLookSpeed);

        if (m_FreeLookSettings.RotationMinX <= transform.eulerAngles.x && transform.eulerAngles.x <= m_FreeLookSettings.RotationMaxX) return;

        transform.RotateAround(m_CenterOfPlayer.position, -transform.right, -verticalDelta * m_FreeLookSettings.FreeLookSpeed);
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        transform.position += transform.forward * scroll * m_ZoomSettings.ZoomSpeed;

        float distance = Vector3.Distance(transform.position, m_CenterOfPlayer.position);

        if (m_ZoomSettings.ViewRangeMin <= distance && distance <= m_ZoomSettings.ViewRangeMax) return;

        transform.position -= transform.forward * scroll * m_ZoomSettings.ZoomSpeed;
    }

    void FixPosition()
    {
        if (m_IsChasingPlayer) transform.position += m_CenterOfPlayer.position - m_LatestCenterPositionOfPlayer;
    }

    /// <summary>
    /// 固定カメラモードに変更します。
    /// </summary>
    public void EnableFixedCameraMode()
    {
        m_ZoomSettings.ZoomLock = true;
        m_FreeLookSettings.FreeLookLock = true;
        m_IsChasingPlayer = false;
    }

    /// <summary>
    /// 固定カメラモードを解除します。
    /// </summary>
    public void DisableFixedCameraMode()
    {
        m_ZoomSettings.ZoomLock = false;
        m_FreeLookSettings.FreeLookLock = false;
        m_IsChasingPlayer = true;
    }

    public void Awake()
    {
        m_InitialPosition = transform.position;
        Initialize();
    }

    void Update()
    {
        FixPosition();

        if (!m_FreeLookSettings.FreeLookLock) RatateAroundTarget();
        if (!m_ZoomSettings.ZoomLock) Zoom();

        m_LatestCenterPositionOfPlayer = m_CenterOfPlayer.position;
    }
}

[System.Serializable]
public class ZoomSettings
{
    [SerializeField] bool m_ZoomLock;
    public bool ZoomLock
    {
        get { return m_ZoomLock; }
        set { m_ZoomLock = value; }
    }

    [SerializeField] float m_ZoomSpeed = 10.0f;
    public float ZoomSpeed
    {
        get { return m_ZoomSpeed; }
    }

    [SerializeField] float m_ViewRangeMin = 2.5f;
    public float ViewRangeMin
    {
        get { return m_ViewRangeMin; }
    }

    [SerializeField] float m_ViewRangeMax = 15.0f;
    public float ViewRangeMax
    {
        get { return m_ViewRangeMax; }
    }
}

[System.Serializable]
public class FreeLookSettings
{
    [SerializeField] bool m_FreeLookLock;
    public bool FreeLookLock
    {
        get { return m_FreeLookLock; }
        set { m_FreeLookLock = value; }
    }

    [SerializeField] float m_FreeLookSpeed = 1.0f;
    public float FreeLookSpeed
    {
        get { return m_FreeLookSpeed; }
    }

    [SerializeField] float m_RotationMinX = 0.0f;
    public float RotationMinX
    {
        get { return m_RotationMinX; }
    }

    [SerializeField] float m_RotationMaxX = 80.0f;
    public float RotationMaxX
    {
        get { return m_RotationMaxX; }
    }
}