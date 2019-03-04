using UnityEngine;

public class TpsCamera : MonoBehaviour
{
    #region

    [System.Serializable]
    class ZoomSetting
    {
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
    public class RotationSetting
    {
        [SerializeField] float m_RotationSpeed = 1.0f;
        public float RotationSpeed
        {
            get { return m_RotationSpeed; }
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

    #endregion

    [Header("Required")]
    [SerializeField] Transform m_Focus;

    [Header("Settings")]
    [SerializeField] ZoomSetting m_ZoomSetting = new ZoomSetting();
    [SerializeField] RotationSetting m_RotationSetting = new RotationSetting();

    Vector3 m_InitialPosition;
    Vector3 m_LatestFocusPosition;

    public void Initialize()
    {
        transform.position = m_InitialPosition;
        transform.LookAt(m_Focus.position);
        m_LatestFocusPosition = m_Focus.position;
    }

    void RatateAround(Transform target)
    {
        var horizontalDelta = Input.GetAxis("Mouse X");
        var verticalDelta = Input.GetAxis("Mouse Y");

        transform.RotateAround(target.position, Vector3.up, horizontalDelta * m_RotationSetting.RotationSpeed);
        transform.RotateAround(target.position, -transform.right, verticalDelta * m_RotationSetting.RotationSpeed);

        if (m_RotationSetting.RotationMinX <= transform.eulerAngles.x && transform.eulerAngles.x <= m_RotationSetting.RotationMaxX) return;

        transform.RotateAround(target.position, -transform.right, -verticalDelta * m_RotationSetting.RotationSpeed);
    }

    void Zoom(Vector3 target)
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        var distance = Vector3.Distance(transform.position, target);

        transform.position += transform.forward * scroll * m_ZoomSetting.ZoomSpeed;

        if (m_ZoomSetting.ViewRangeMin <= distance && distance <= m_ZoomSetting.ViewRangeMax) return;

        transform.position -= transform.forward * scroll * m_ZoomSetting.ZoomSpeed;
    }

    void ChaseTarget()
    {
        transform.position += m_Focus.position - m_LatestFocusPosition;
    }

    void Awake()
    {
        m_InitialPosition = transform.position;
        Initialize();
    }

    void Update()
    {
        ChaseTarget();

        if (Input.GetMouseButton(1)) RatateAround(m_Focus);
        Zoom(m_Focus.position);

        m_LatestFocusPosition = m_Focus.position;
    }
}