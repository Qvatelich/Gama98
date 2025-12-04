using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public bool IsJostick;
    [Header("Цель слежения")]
    [SerializeField] private Transform _target;

    [Header("Настройки камеры")]
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _minDistance = 2f;
    [SerializeField] private float _maxDistance = 15f;
    [SerializeField] private float _zoomSpeed = 0.5f;

    [Header("Настройки вращения")]
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private float _minVerticalAngle = -30f;
    [SerializeField] private float _maxVerticalAngle = 80f;

    [Header("Настройки плавности")]
    [SerializeField] private float _smoothTime = 0.2f;
    [SerializeField] private bool _invertY = false;

    [Header("Защита от UI")]
    [SerializeField] private bool _ignoreUI = true; // Игнорировать касания по UI

    // Текущие углы вращения
    private float _currentX = 0f;
    private float _currentY = 20f;

    // Для сглаживания движения
    private Vector3 _smoothVelocity = Vector3.zero;

    // Для обработки мультитача
    private float _initialTouchDistance;
    private float _initialCameraDistance;
    private Vector2 _previousSingleTouchPosition;
    private bool _isRotating = false;
    private bool _isZooming = false;

    // Для защиты от джойстика
    [Header("Защита от джойстика")]
    [SerializeField] private string[] _ignoredUITags = { "Joystick", "Button", "UI" };
    [SerializeField] private LayerMask _uiLayerMask = 1 << 5; // Слой UI (5)
    [Header("Зоны исключения")]
    [SerializeField] private RectTransform _joystickZone;
    [SerializeField] private float _joystickZonePadding = 50f;

   
    void Start()
    {
        if (_target == null)
        {
            Debug.LogError("Цель не назначена для камеры!");
            enabled = false;
            return;
        }

        // Инициализация начальной позиции камеры
        Vector3 angles = transform.eulerAngles;
        _currentX = angles.y;
        _currentY = Mathf.Clamp(angles.x, _minVerticalAngle, _maxVerticalAngle);

        UpdateCameraPosition(true);
    }

    void Update()
    {
        if (_target == null) return;

        HandleMobileInput();
        UpdateCameraPosition();
    }

    void HandleMobileInput()
    {
        // Обработка зума двумя пальцами
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
        // Обработка вращения одним пальцем
        else if (Input.touchCount == 1 && !_isZooming)
        {
            HandleSingleTouchRotation();
        }

        // Сброс флагов, если пальцы убраны
        if (Input.touchCount == 0)
        {
            _isRotating = false;
            _isZooming = false;
        }
    }

    void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        // Проверяем, не касаемся ли мы UI элементов
        if (_ignoreUI && (IsTouchOnUI(touch1) || IsTouchOnUI(touch2)))
        {
            _isZooming = false;
            return;
        }

        _isZooming = true;
        _isRotating = false;

        // В начале жеста зума
        if (touch2.phase == TouchPhase.Began)
        {
            _initialTouchDistance = Vector2.Distance(touch1.position, touch2.position);
            _initialCameraDistance = _distance;
        }
        // Во время жеста зума
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
            float deltaDistance = currentTouchDistance - _initialTouchDistance;

            // Изменяем дистанцию камеры
            _distance = _initialCameraDistance - deltaDistance * _zoomSpeed * 0.01f;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }
    }

    void HandleSingleTouchRotation()
    {
        Touch touch = Input.GetTouch(0);

        // Проверяем, не касаемся ли мы UI элементов
        if (_ignoreUI && IsTouchOnUI(touch))
        {
            // Дополнительная проверка для джойстика
            if (IsJostick/*IsTouchOnJoystick(touch.position)*/)
            {
                _isRotating = false;
                return;
            }
        }

        if (touch.phase == TouchPhase.Began)
        {
            // Проверяем, не начинаем ли касание на джойстике
            if (IsJostick/*IsTouchOnJoystick(touch.position)*/)
            {
                _isRotating = false;
                return;
            }

            _previousSingleTouchPosition = touch.position;
            _isRotating = true;
            _isZooming = false;
        }
        else if (touch.phase == TouchPhase.Moved && _isRotating)
        {
            Vector2 delta = touch.position - _previousSingleTouchPosition;

            // Вращение по горизонтали
            _currentX += delta.x * _rotationSpeed * 0.01f;

            // Вращение по вертикали
            float yDelta = delta.y * _rotationSpeed * 0.01f;
            _currentY += _invertY ? yDelta : -yDelta;
            _currentY = Mathf.Clamp(_currentY, _minVerticalAngle, _maxVerticalAngle);

            _previousSingleTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isRotating = false;
        }
    }

    bool IsTouchOnUI(Touch touch)
    {
#if UNITY_EDITOR
        // Для редактора проверяем через EventSystem
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return true;
        }
#else
        // Для мобильных устройств
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return true;
        }
#endif

        return false;
    }

    /*bool IsTouchOnJoystick(Vector2 touchPosition)
    {
        // Создаем луч из позиции касания
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Проверяем Raycast по UI слою
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _uiLayerMask))
        {
            // Проверяем тег объекта
            foreach (string tag in _ignoredUITags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    return true;
                }
            }
        }

        // Альтернативная проверка через 2D Raycast (для Canvas)
        RaycastHit2D hit2D = Physics2D.Raycast(touchPosition, Vector2.zero, 0f, _uiLayerMask);
        if (hit2D.collider != null)
        {
            foreach (string tag in _ignoredUITags)
            {
                if (hit2D.collider.CompareTag(tag))
                {
                    return true;
                }
            }
        }

        return false;
    }*/

    void UpdateCameraPosition(bool immediate = false)
    {
        if (_target == null) return;

        // Создаем вращение из текущих углов
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);

        // Вычисляем желаемую позицию камеры
        Vector3 targetPosition = _target.position + Vector3.up * 1f; // Немного выше цели
        Vector3 desiredPosition = targetPosition - rotation * Vector3.forward * _distance;

        // Проверяем коллизии с окружением
        RaycastHit hit;
        Vector3 direction = desiredPosition - targetPosition;
        if (Physics.SphereCast(targetPosition, 0.3f, direction.normalized, out hit, _distance))
        {
            // Если есть препятствие, приближаем камеру
            desiredPosition = hit.point - direction.normalized * 0.3f;
        }

        // Плавно перемещаем камеру
        if (immediate)
        {
            transform.position = desiredPosition;
            transform.LookAt(targetPosition);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref _smoothVelocity,
                _smoothTime
            );

            // Плавно поворачиваем камеру к цели
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }

    // Метод для принудительной установки позиции камеры
    public void ResetCameraToDefault()
    {
        _currentX = _target.eulerAngles.y;
        _currentY = 30f; // Угол по умолчанию
        _distance = (_minDistance + _maxDistance) * 0.5f;
        UpdateCameraPosition(true);
    }

    // Методы для настройки через другие скрипты
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
        if (_target != null)
        {
            ResetCameraToDefault();
        }
    }

    public void SetDistance(float newDistance)
    {
        _distance = Mathf.Clamp(newDistance, _minDistance, _maxDistance);
    }

    // Для отладки в редакторе
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_target.position, transform.position);
            Gizmos.DrawWireSphere(_target.position + Vector3.up * 1f, 0.5f);
        }
    }
#endif
}
