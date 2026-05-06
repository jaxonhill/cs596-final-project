using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Follow")]
    [SerializeField] private float followDistance = 8f;
    [SerializeField] private float heightOffset = 2f;

    [Header("Rotation")]
    [Range(0f, 60f)]
    [SerializeField] private float tiltDown = 30f;

    [Header("Collision")]
    [Tooltip("LayerMask for camera obstacle detection. In Unity Editor, set this to everything EXCEPT Player (6) and UI (5).")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float collisionBuffer = 0.2f;
    [SerializeField] private float minDistance = 1.2f;
    [SerializeField] private float focusHeight = 1.2f;

    private bool loggedMissingPlayer;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError($"CameraFollowPlayer requires player Transform assigned to '{nameof(player)}' on {name}.", this);
            loggedMissingPlayer = true;
            return;
        }

        Debug.Log($"CameraFollowPlayer found required player Transform reference on {name}.", this);
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            if (!loggedMissingPlayer)
            {
                Debug.LogError($"CameraFollowPlayer requires player Transform assigned to '{nameof(player)}' on {name}.", this);
                loggedMissingPlayer = true;
            }

            return;
        }

        Vector3 desiredPosition = player.position - player.forward * followDistance + Vector3.up * heightOffset;
        Vector3 focusPoint = player.position + Vector3.up * focusHeight;
        Vector3 direction = desiredPosition - focusPoint;
        float desiredDistance = direction.magnitude;

        if (collisionLayers != 0 && Physics.SphereCast(focusPoint, collisionRadius, direction.normalized, out RaycastHit hit, desiredDistance, collisionLayers))
        {
            float actualDistance = Mathf.Max(hit.distance - collisionBuffer, minDistance);
            transform.position = focusPoint + direction.normalized * actualDistance;
            Debug.Log($"[Camera] Obstacle: {hit.collider.name} at distance {hit.distance:F2}. Clamped from {desiredDistance:F2} to {actualDistance:F2}");
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.rotation = Quaternion.Euler(tiltDown, player.eulerAngles.y, 0f);
    }
}
