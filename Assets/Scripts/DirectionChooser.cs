using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DirectionChooser : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Camera mainCamera;

    public GameObject button;
    
    private bool isLocked = false;
    
    public static DirectionChooser Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        if (target == null)
            target = transform;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Ball ball = BallManager.Instance.GetMyBall();
        target = ball != null ? ball.arrow : null;
        isLocked = false;
    }

    private void Update()
    {
        if (isLocked || target == null || Mouse.current == null || mainCamera == null)
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
        if (isLocked || target == null)
            return;

        isLocked = true;

        if (NetworkManager.Instance.isHost)
        {
            GameManager.Instance.LockDirection(GetDirection(), 0);
        }
        else
        {
            ClientSend.LockDirection(GetDirection());
        }
        
        button.SetActive(false);
    }

    public void Unlock()
    {
        isLocked = false;
        button.SetActive(true);
    }

    public Vector2 GetDirection()
    {
        return target != null ? (Vector2)target.right.normalized : Vector2.zero;
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}
