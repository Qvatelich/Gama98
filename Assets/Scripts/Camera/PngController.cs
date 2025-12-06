using UnityEngine;

public sealed class PngController : MonoBehaviour
{
    private void Update()
    {
        Vector3 camraDirection = Camera.main.transform.forward;
        float rotateAngle = Mathf.Atan2(camraDirection.x, camraDirection.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, rotateAngle, 0);
        if (camraDirection.x != 0 || camraDirection.y != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 40f * Time.deltaTime);
    }
}
