using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [NonSerialized] public Player owner;
    [NonSerialized] public BallType type;

    [SerializeField] protected float speed = 7f;
    [SerializeField] protected int contactDamage = 10;

    [SerializeField] private TMP_Text username;
    [SerializeField] private Slider healthBar;

    public int health = 100;

    protected Rigidbody2D rb;
    public Vector2 direction;

    public bool move;
    public Transform arrow;

    protected bool IsHost => NetworkManager.Instance.isHost;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsHost)
            rb.simulated = false;
    }

    public void Initialize()
    {
        username.text = owner.username;
        healthBar.value = health / 100f;
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsHost || health <= 0)
            return;

        health = Mathf.Max(0, health - damage);
        healthBar.value = health / 100f;

        ServerSend.BallHealth(owner.id, health);
    }

    public void SetHealth(int health)
    {
        this.health = health;
        healthBar.value = health / 100f;
    }

    protected virtual void Start()
    {
        if (owner.id != NetworkManager.Instance.myId)
            arrow.gameObject.SetActive(false);

        if (!IsHost)
            return;

        direction = UnityEngine.Random.insideUnitCircle.normalized;
        UpdateVelocity();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsHost)
            return;

        UpdateVelocity();
    }

    protected void UpdateVelocity()
    {
        if (move)
            rb.linearVelocity = direction * speed;
    }

    public virtual int GetContactDamage()
    {
        return contactDamage;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsHost)
            return;

        bool isWall = collision.gameObject.CompareTag("Wall");
        Ball otherBall = collision.gameObject.GetComponent<Ball>();

        if (!isWall && otherBall == null)
            return;

        if (isWall)
            OnCollisionWithWall();

        if (otherBall != null)
            OnCollisionWithBall(otherBall);

        Vector2 normal = collision.GetContact(0).normal;

        direction = Vector2.Reflect(direction, normal).normalized;

        if (Vector2.Dot(direction, normal) <= 0.01f)
            direction = (normal + direction * 0.2f).normalized;

        UpdateVelocity();
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsHost)
            return;

        if (!collision.gameObject.CompareTag("Wall") &&
            collision.gameObject.GetComponent<Ball>() == null)
            return;

        Vector2 normal = collision.GetContact(0).normal;

        if (Vector2.Dot(direction, normal) <= 0.01f)
        {
            direction = normal.normalized;
            UpdateVelocity();
        }
    }

    public virtual void OnCollisionWithWall()
    {
    }

    public virtual void OnCollisionWithBall(Ball otherBall)
    {
        if (health <= 0 || otherBall.health <= 0)
            return;

        otherBall.TakeDamage(GetContactDamage());
    }

    public void ApplySnapshot(BallSnapshot from, BallSnapshot to, float t)
    {
    }
}
