using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GamepadCursor : SingletonMonoBehaviour<GamepadCursor>
{
    private Mouse virtualMouse;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Image cursorImage;
    [SerializeField] private float cursorSpeed = 900f;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private float padding = 35f;
    [SerializeField] private ItemFaderSettings faderSettings;

    private bool previousMouseState;
    private Vector2 deltaValue;
    private float fadeOutTimer;
    private bool isVisible = true;

    private bool _canFadeOut = true;
    private string _currentControlScheme;

    public bool CanFadeOut
    {
        get => _canFadeOut;
        set => _canFadeOut = value;
    }

    public string CurrentControlScheme
    {
        get => _currentControlScheme;
        private set => _currentControlScheme = value;
    }

    private void OnEnable()
    {
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

        CurrentControlScheme = playerInput.currentControlScheme;

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput playerInput)
    {
        CurrentControlScheme = playerInput.currentControlScheme;
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    public Vector2 GetVirtualMousePosition()
    {
        return virtualMouse.position.ReadValue();
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        deltaValue = Gamepad.current.rightStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        if (deltaValue != Vector2.zero && isVisible == false)
        {
            FadeIn();
        }

        if (deltaValue == Vector2.zero)
        {
            if (fadeOutTimer > 0 && CanFadeOut)
            {
                fadeOutTimer -= Time.deltaTime;
            }
            else
            {
                if (CanFadeOut && isVisible == true)
                {
                    FadeOut();
                }
            }
        }
        else
        {
            fadeOutTimer = 1f;
        }

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool buttonSouthPressed = Gamepad.current.rightShoulder.IsPressed();
        if (previousMouseState != buttonSouthPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, buttonSouthPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = buttonSouthPressed;
        }

        MoveCursor(newPosition);
        deltaValue = Vector2.zero;
    }

    private void MoveCursor(Vector2 newPosition)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPosition, null, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentAlpha = cursorImage.color.a;
        float distance = currentAlpha - faderSettings.targetAlpha;

        while (currentAlpha - faderSettings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / faderSettings.fadeOutSeconds * Time.deltaTime;
            cursorImage.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        cursorImage.color = new Color(1, 1, 1, faderSettings.targetAlpha);
        isVisible = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentAlpha = cursorImage.color.a;
        float distance = 1 - currentAlpha;

        while (1 - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / faderSettings.fadeInSeconds * Time.deltaTime;
            cursorImage.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        cursorImage.color = new Color(1, 1, 1, 1);
        isVisible = true;
    }
}
