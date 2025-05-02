using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;              // 플레이어의 Transform
    public float smoothSpeed = 0.125f;    // 카메라 따라가는 속도
    public Vector3 offset;                // 카메라와 플레이어 사이의 거리(오프셋)

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        // 목표 위치: 플레이어 위치 + 오프셋
        Vector3 targetPosition = player.position + offset;

        // 카메라 이동 
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }
}
