using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using System;

public class GamepadCursor : MonoBehaviour
{
    private Mouse virtualMouse;
    private Mouse currentMouse;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private float padding = 35f;

    private bool previousMouseState;
    private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private void OnEnable()
    {
        currentMouse = Mouse.current;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (virtualMouse.added == false)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 cursorPosition = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, cursorPosition);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool buttonSouthPressed = Gamepad.current.buttonSouth.IsPressed();
        if (previousMouseState != buttonSouthPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, buttonSouthPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = buttonSouthPressed;
        }

        MoveCursor(newPosition);
    }

    private void MoveCursor(Vector2 newPosition)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPosition, null, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            Vector2 mousePosition = virtualMouse.position.ReadValue();
            currentMouse.WarpCursorPosition(mousePosition);
            previousControlScheme = mouseScheme;
        }
        else if (input.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            MoveCursor(currentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }

    private void OnDisable()
    {
        //playerInput.user.UnpairDevice(virtualMouse);
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void Update()
    {
        //if (playerInput.currentControlScheme != previousControlScheme)
        //{
        //    OnControlsChanged(playerInput);
        //}

        //previousControlScheme = playerInput.currentControlScheme;

    }
}
