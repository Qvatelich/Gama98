using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickUIBlocker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string _joystickTag = "Joystick";
    [SerializeField] private CameraController _сameraController;

    void Start()
    {
        gameObject.tag = _joystickTag;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Джойстик активирован
        _сameraController.IsJostick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Джойстик деактивирован
        _сameraController.IsJostick = false;
    }
}