using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile_Sentry : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 5f;
    Rigidbody2D rb;
    Vector2 direction = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        // Si golpea un switch
        if (other.CompareTag("Switch"))
        {
            Switch_Door button = other.GetComponent<Switch_Door>();
            if (button != null)
            {
                button.Activate(); // Activa el botón
            }

            // Destruye la bala después de activar el switch
            Destroy(gameObject);
            return;
        }

        // Si golpea el suelo u otra superficie sólida
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }

    }

}
