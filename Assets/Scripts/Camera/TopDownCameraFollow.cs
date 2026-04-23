using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float targetHeightOffset = 1f;

    [Header("Orbit")]
    [SerializeField] private float yaw = 45f;
    [SerializeField] private float pitch = 60f;
    [SerializeField] private float distance = 15f;

    [Header("Follow")]
    [SerializeField] private float followLerpSpeed = 3.5f;

    [Header("Camera")]
    [SerializeField] private float fieldOfView = 50f;

    [Header("Debug")]
    [SerializeField] private bool verboseLogs = true;

    private Camera cachedCamera;
    private bool warnedAboutMissingTarget;

    private void Awake()
    {
        cachedCamera = GetComponent<Camera>();
        if (cachedCamera != null)
        {
            cachedCamera.orthographic = false;
            cachedCamera.fieldOfView = fieldOfView;
        }
    }

    private void Start()
    {
        SnapToTarget();

        if (!verboseLogs)
        {
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[TopDownCameraFollow] No target assigned. Camera follow is inactive until a target is set.", this);
            warnedAboutMissingTarget = true;
            return;
        }

        Debug.Log(
            $"[TopDownCameraFollow] Ready. target={target.name}, yaw={yaw:F1}, pitch={pitch:F1}, distance={distance:F1}, followLerp={followLerpSpeed:F2}, fov={fieldOfView:F1}.",
            this);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            if (!warnedAboutMissingTarget && verboseLogs)
            {
                Debug.LogWarning("[TopDownCameraFollow] Missing target reference during LateUpdate.", this);
                warnedAboutMissingTarget = true;
            }

            return;
        }

        warnedAboutMissingTarget = false;

        Vector3 anchor = target.position + Vector3.up * targetHeightOffset;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = anchor + orbitRotation * (Vector3.back * distance);

        float t = Mathf.Clamp01(followLerpSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
        transform.rotation = Quaternion.LookRotation(anchor - transform.position, Vector3.up);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        warnedAboutMissingTarget = false;

        if (verboseLogs)
        {
            Debug.Log(newTarget == null
                ? "[TopDownCameraFollow] Target cleared."
                : $"[TopDownCameraFollow] Target set to '{newTarget.name}'.", this);
        }
    }

    private void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 anchor = target.position + Vector3.up * targetHeightOffset;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = anchor + orbitRotation * (Vector3.back * distance);
        transform.rotation = Quaternion.LookRotation(anchor - transform.position, Vector3.up);
    }

    private void OnValidate()
    {
        targetHeightOffset = Mathf.Max(0f, targetHeightOffset);
        distance = Mathf.Max(0.01f, distance);
        followLerpSpeed = Mathf.Max(0f, followLerpSpeed);
        fieldOfView = Mathf.Clamp(fieldOfView, 1f, 179f);

        if (cachedCamera != null)
        {
            cachedCamera.orthographic = false;
            cachedCamera.fieldOfView = fieldOfView;
        }
    }
}
