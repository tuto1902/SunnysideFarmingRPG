using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputManager", menuName = "Scriptable Objects/Input Manager")]
public class InputManager : ScriptableObject, InputActions.IPlayerActions
{
    public event UnityAction<Vector2> moveEvent;
    private InputActions inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
            inputActions.Player.SetCallbacks(this);
            inputActions.Enable();
        }
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (moveEvent != null)
        {
            moveEvent.Invoke(context.ReadValue<Vector2>());
        }
    }
}
