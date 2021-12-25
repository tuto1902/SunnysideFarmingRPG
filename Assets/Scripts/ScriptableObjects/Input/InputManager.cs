using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputManager", menuName = "Scriptable Objects/Input Manager")]
public class InputManager : ScriptableObject, InputActions.IPlayerActions
{
    public event UnityAction<Vector2> moveEvent;
    public event UnityAction toggleRunEvent;
    public event UnityAction itemSelectLeft;
    public event UnityAction itemSelectRight;
    public event UnityAction playerClick;
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

    public void OnRun(InputAction.CallbackContext context)
    {
        if (toggleRunEvent != null)
        {
            toggleRunEvent.Invoke();
        }
    }

    public void OnItemSelectLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled && itemSelectLeft != null)
        {
            itemSelectLeft.Invoke();
        }
    }

    public void OnItemSelectRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled && itemSelectRight != null)
        {
            itemSelectRight.Invoke();
        }
    }

    public void OnTestInputAdvanceTime(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
    }

    public void OnTestInputAdvanceDay(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
    }

    public void OnTestInputReloadScene(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), Player.Instance.gameObject.transform.position);
        }
    }

    public void OnPlayerClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled && playerClick != null)
        {
            playerClick.Invoke();
        }
    }
}
