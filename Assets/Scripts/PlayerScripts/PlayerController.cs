using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerController : MonoBehaviour
{
    [Range(0, 50)][SerializeField] private float _moveSpeed;
    [Range(0, 50)][SerializeField] private float _jumpForce;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private float _rotateSpeed;
    float rotateAngle;

    private void Start() =>
        _rb = GetComponent<Rigidbody>();

    private void Update()
    {
       // float x = Input.GetAxisRaw("Horizontal");
      //  float y = Input.GetAxisRaw("Vertical");
        float x = _joystick.Horizontal;
        float y = _joystick.Vertical;

        Vector3 moveVector = (x * Camera.main.transform.right + y * Camera.main.transform.forward).normalized *2f;
        moveVector.y = _rb.velocity.y;
        _rb.velocity = moveVector;
        Vector3 rotateVector = _rb.velocity;
        float rotateAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, rotateAngle, 0);
        if (x != 0 || y != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
    }
}
