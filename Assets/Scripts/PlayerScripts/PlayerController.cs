using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerController : MonoBehaviour
{
    [Range(0, 50)][SerializeField] private float _moveSpeed;
    [Range(0, 50)][SerializeField] private float _jumpForce;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Animator _anim;
    [SerializeField] private float _rotateSpeed;

    private bool _jump = true;
    private float rotateAngle;

    private void Start() =>
        _rb = GetComponent<Rigidbody>();

    private void Update()
    {
        // float x = Input.GetAxisRaw("Horizontal");
        //  float y = Input.GetAxisRaw("Vertical");
        float x = _joystick.Horizontal;
        float y = _joystick.Vertical;
        /*if (x == 0 && y == 0)
            _anim.Play("Idle");
        else if (x == 0 && y == 0 && _jump)
            _anim.Play("Run");
        if (!_jump)
            _anim.Play("Jump");*/
        Vector3 moveVector = (x * Camera.main.transform.right + y * Camera.main.transform.forward).normalized * 4f;
        moveVector.y = _rb.velocity.y;
        _rb.velocity = moveVector;
        Vector3 rotateVector = _rb.velocity;
        float rotateAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, rotateAngle, 0);
        if (x != 0 || y != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
    }
    public void Jump()
    {
        if (_jump)
            _rb.AddForce(0, _jumpForce * 20f, 0);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            _jump = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            _jump = true;        
    }
}
