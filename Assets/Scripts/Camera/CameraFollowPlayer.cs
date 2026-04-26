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

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        transform.position = player.position - player.forward * followDistance + Vector3.up * heightOffset;
        transform.rotation = Quaternion.Euler(tiltDown, player.eulerAngles.y, 0f);
    }
}