using UnityEngine;

/// <summary>
/// 相机控制器，负责跟随玩家移动
/// </summary>
public class CameraController : MonoBehaviour
{
    public Transform player;

    private Vector3 offset;

    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.position + offset;
    }
}
