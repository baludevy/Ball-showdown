using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DirectionChooser : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Camera mainCamera;

    private bool isLocked = false;

    private void Awake()
    {
        if (target == null)
            target = transform;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        target = BallManager.Instance.GetMyBall().arrow;
    }

    private void Update()
    {
        if (isLocked || Mouse.current == null || mainCamera == null)
            return;
        
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (!Mouse.current.leftButton.isPressed)
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            new Vector3(
                mouseScreen.x,
                mouseScreen.y,
                Mathf.Abs(target.position.z - mainCamera.transform.position.z)
            )
        );

        Vector2 direction = mouseWorld - target.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void Lock()
    {
        isLocked = true;

        if (NetworkManager.Instance.isHost)
        {
            GameManager.Instance.LockDirection(GetDirection(), 0);
        }
        else
        {
            ClientSend.LockDirection(GetDirection());
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public Vector2 GetDirection()
    {
        return target.right.normalized;
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}
