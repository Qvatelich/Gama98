using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlanetOwner))]
public class PlanetMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpForce = 8f;

    private Rigidbody _rb;
    private PlanetOwner _owner;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _owner = GetComponent<PlanetOwner>();
    }

    private void FixedUpdate()
    {
        if (_owner.CurrentPlanet == null)
            return;

        HandleMovement();
        HandleGroundCheck();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // теперь = влево/вправо
        float v = Input.GetAxis("Vertical");   // вперед/назад

        PlanetGravitySource planet = _owner.CurrentPlanet;
        Vector3 normal = planet.GetSurfaceNormal(transform.position);

        // Локальные оси вдоль поверхности
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(transform.right, normal).normalized;

        // Итоговое направление движения
        Vector3 moveDir = (forward * v + right * h).normalized;

        // Применяем движение
        Vector3 velocity = _rb.velocity;
        Vector3 verticalComponent   = Vector3.Project(velocity, normal);
        Vector3 horizontalComponent = moveDir * _moveSpeed;

        _rb.velocity = verticalComponent + horizontalComponent;

        // Поворот только при движении вперед
        if (v != 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(forward * Mathf.Sign(v), normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleGroundCheck()
    {
        PlanetGravitySource planet = _owner.CurrentPlanet;
        Vector3 surfaceNormal = planet.GetSurfaceNormal(transform.position);

        if (Physics.Raycast(transform.position, -surfaceNormal, out _, 1.1f))
            _isGrounded = true;
        else
            _isGrounded = false;
    }
}
