using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    private InputAction mousePositionAction;

    void Start()
    {
        Cursor.visible = true;
        mousePositionAction = new InputAction("MousePosition", InputActionType.Value, "<Mouse>/position");
        mousePositionAction.Enable();
    }

    void Update()
    {
        Vector2 mouseCursorPos = Camera.main.ScreenToWorldPoint(mousePositionAction.ReadValue<Vector2>());
        transform.position = mouseCursorPos;
    }
}
