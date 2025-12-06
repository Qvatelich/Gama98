using UnityEngine;

public sealed class EnemyPotrooling : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private bool _potrool;
    private Transform _player;
    private Vector3 _startPos;

    private void Start() =>
        _startPos = transform.position;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            _potrool = true;
        Debug.Log("Hyeta");
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            _potrool = false;
    }
    private void FixedUpdate()
    {
        if (_potrool)
        {
            Vector3 targetPos = _player.transform.position;
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position,targetPos, _speed);
        }
        if (transform.position != _startPos)
            Vector3.Lerp(transform.position, _startPos, _speed);
    }
}
