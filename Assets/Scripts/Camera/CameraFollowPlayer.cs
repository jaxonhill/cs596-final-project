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

        transform.position = player.position - player.forward * followDistance + Vector3.up * heightOffset;
        transform.rotation = Quaternion.Euler(tiltDown, player.eulerAngles.y, 0f);
    }
}
