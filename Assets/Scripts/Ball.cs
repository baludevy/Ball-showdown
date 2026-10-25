using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [NonSerialized] public Player owner;
    [NonSerialized] public BallType type;

    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!NetworkManager.Instance.isHost)
            rb.simulated = false;
    }

    private void Start()
    {
        if (!NetworkManager.Instance.isHost)
            return;

        direction = UnityEngine.Random.insideUnitCircle.normalized;
        rb.linearVelocity = direction * speed;
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.Instance.isHost)
            return;
        
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!NetworkManager.Instance.isHost)
            return;

        if (!collision.gameObject.CompareTag("Wall") &&
            collision.gameObject.GetComponent<Ball>() == null)
            return;

        Vector2 normal = collision.GetContact(0).normal;
        
        direction = Vector2.Reflect(direction, normal).normalized;
        
        if (Vector2.Dot(direction, normal) <= 0.01f)
        {
            direction = (normal + direction * 0.2f).normalized;
        }

        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!NetworkManager.Instance.isHost)
            return;

        if (!collision.gameObject.CompareTag("Wall") && collision.gameObject.GetComponent<Ball>() == null)
            return;

        Vector2 normal = collision.GetContact(0).normal;
        
        if (Vector2.Dot(direction, normal) <= 0.01f)
        {
            direction = normal.normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    public void ApplySnapshot(BallSnapshot from, BallSnapshot to, float t)
    {
        
    }
}