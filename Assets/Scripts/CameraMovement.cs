using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 1.5f;

    private Vector3 ResetPosition = Vector3.zero;
    private bool isDragging;

    #region Initialize Methods
    public void Initialize()
    {
        AddInputAction();
    }

    public void UnInitalize()
    {
        RemoveInputAction();
    }

    private void AddInputAction()
    {
        InputManager.MouseLeftStartedAction += OnMouseLeftStarted;
        InputManager.MouseLeftCanceledAction += OnMouseLeftCanceld;
        InputManager.MouseDragMoveAction += OnMouseDragMove;
    }

    private void RemoveInputAction()
    {
        InputManager.MouseLeftStartedAction -= OnMouseLeftStarted;
        InputManager.MouseLeftCanceledAction -= OnMouseLeftCanceld;

        InputManager.MouseDragMoveAction -= OnMouseDragMove;
    }
    #endregion

    #region Main Methods
    public void ResetToCenter()
    {
        transform.position = ResetPosition;
    }
    
    private void DragMap(Vector2 moveDirection)
    {
        float directionAngle = GetDirectionAngle(moveDirection);
        directionAngle = AddCameraRotationToAngle(directionAngle);

        Vector3 targetDirection = GetTargetRotationDirection(directionAngle);
        
        transform.position -= targetDirection * dragSpeed;
    }

    private float GetDirectionAngle(Vector2 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        if (directionAngle < 0f) directionAngle += 360f;
        return directionAngle;
    }


    private float AddCameraRotationToAngle(float angle)
    {
        angle += Camera.main.transform.eulerAngles.y;

        if (angle > 360f) angle -= 360f;
        return angle;
    }

    private Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
    }
    #endregion

    #region Callback Methods
    private void OnMouseLeftStarted()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = InputManager.playerActions.MousePosition.ReadValue<Vector2>();

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        // Check each result for UI elements on the specified layer
        foreach (var result in results)
        {
            if (result.gameObject.layer == 5)
            {
                return;
            }
        }
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        isDragging = true;
    }

    private void OnMouseLeftCanceld()
    {
        isDragging = false;
    }

    private void OnMouseDragMove(Vector2 moveDirection)
    {
        if (!isDragging) return;
        DragMap(moveDirection);
    }
    #endregion
}
