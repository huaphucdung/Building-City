using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public static class InputManager
{
    public static InputController inputActions;
    public static InputController.PlayerActions playerActions;
    public static InputController.UIActions uiActions;

    private static bool isInit;

    public static Action<Vector2> MouseMoveAction;
    public static Action<Vector2> MouseDragMoveAction;
    
    public static Action MouseLeftTapAction;

    public static Action MouseLeftStartedAction;
    public static Action MouseLeftCanceledAction;

    public static Action RotationAction;

    public static void Initialize()
    {
        if (isInit) return;
        isInit = true;

        inputActions = new InputController();
        playerActions = inputActions.Player;
        uiActions = inputActions.UI;
        inputActions.Enable();

        
    }

    public static void AddInputAction()
    {
        playerActions.MousePosition.performed += OnMouseMove;

        playerActions.MouseLeftTap.performed += OnMouseLeftTap;
        playerActions.MouseLeft.started += OnMouseLeftClkied;
        playerActions.MouseLeft.canceled += OnMouseLeftClkied;

        playerActions.Rotation.performed += OnRotaton;

        playerActions.MouseDragMove.performed += OnMouseDragMove;
    }

    public static void RemoveInputAction()
    {
        playerActions.MousePosition.performed -= OnMouseMove;

        playerActions.MouseLeftTap.performed -= OnMouseLeftTap;
        playerActions.MouseLeft.started -= OnMouseLeftClkied;
        playerActions.MouseLeft.canceled -= OnMouseLeftClkied;

        playerActions.Rotation.performed -= OnRotaton;

        playerActions.MouseDragMove.performed -= OnMouseDragMove;
    }

    private static void OnMouseMove(InputAction.CallbackContext ctx)
    {
        MouseMoveAction?.Invoke(ctx.ReadValue<Vector2>());
    }

    private static void OnMouseLeftTap(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            MouseLeftTapAction?.Invoke();
        }
    }

    private static void OnMouseLeftClkied(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                MouseLeftStartedAction?.Invoke();
                break;
            case InputActionPhase.Canceled:
                MouseLeftCanceledAction?.Invoke();
                break;
        }  
    }

    private static void OnRotaton(InputAction.CallbackContext ctx)
    {
        RotationAction?.Invoke();
    }

    private static void OnMouseDragMove(InputAction.CallbackContext ctx)
    {
        MouseDragMoveAction?.Invoke(ctx.ReadValue<Vector2>());
    }
}
