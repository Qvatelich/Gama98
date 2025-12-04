using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    public Transform player; // Трансформ игрока
    public Vector3 offset = new Vector3(0, 2, -5); // Смещение камеры относительно игрок

    void LateUpdate()
    {
        if (player == null)
            return;

        // Расчет позиции камеры
        Vector3 desiredPosition = player.position + player.rotation * offset;
        transform.position = desiredPosition;

        // Камера смотрит на игрока
        transform.LookAt(player);
    }
}
