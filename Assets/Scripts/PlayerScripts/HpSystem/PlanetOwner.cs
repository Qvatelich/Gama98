using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetOwner : MonoBehaviour
{
  [SerializeField] private PlanetGravitySource _planet; // можно задать в инспекторе

  public Rigidbody Rigidbody { get; private set; }
  public PlanetGravitySource CurrentPlanet { get; set; }

  private void Awake()
  {
    Rigidbody = GetComponent<Rigidbody>();
    Rigidbody.useGravity = false;           // используем только «планетную» гравитацию
    Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
  }

  private void OnEnable()
  {
    if (_planet != null)
    {
      _planet.Register(this);
    }
  }

  private void OnDisable()
  {
    if (_planet != null)
    {
      _planet.Unregister(this);
    }

    if (CurrentPlanet != null && CurrentPlanet != _planet)
    {
      CurrentPlanet.Unregister(this);
    }
  }

  // Опционально — смена планеты «на лету»
  public void SetPlanet(PlanetGravitySource newPlanet)
  {
    if (CurrentPlanet == newPlanet) return;

    if (CurrentPlanet != null)
      CurrentPlanet.Unregister(this);

    _planet = newPlanet;

    if (_planet != null)
      _planet.Register(this);
  }
}