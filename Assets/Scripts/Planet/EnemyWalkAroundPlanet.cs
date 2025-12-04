using UnityEngine;

public class EnemyWalkAroundPlanet : MonoBehaviour
{
  [SerializeField] private float _moveSpeed = 3f;

  private PlanetOwner _owner;
  private Rigidbody _rb;

  private void Awake()
  {
    _owner = GetComponent<PlanetOwner>();
    _rb = GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    if (_owner.CurrentPlanet == null) return;

    Vector3 normal = _owner.CurrentPlanet.GetSurfaceNormal(transform.position);

    // Пример: идем всегда "вперёд" вдоль поверхности
    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal).normalized;
    _rb.MovePosition(_rb.position + forward * _moveSpeed * Time.fixedDeltaTime);
  }
}