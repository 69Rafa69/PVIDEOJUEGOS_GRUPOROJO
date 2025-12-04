using UnityEngine;

/// <summary>
/// Proyectil que paraliza a los enemigos (EnemyGrabable) y se destruye con el entorno.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Spark : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float duration = 3f;

    [Header("Física y Movimiento")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool moveRight = true;

    [Header("Colisiones")]
    [Tooltip("Capas con las que la bala choca y se destruye (Suelo, Paredes, etc).")]
    [SerializeField] private LayerMask whatIsObstacle; // <--- NUEVO: Para paredes

    [Header("Visuales")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ApplyMovement();

        if (animator != null)
        {
            animator.Play("Spark_Anim", 0, 0f);
        }

        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 0. Ignorar al propio jugador
        if (other.CompareTag("Player")) return;

        // 1. Detección de Enemigo Paralizable
        // Buscamos el script específico EnemyGrabable
        if (other.TryGetComponent<EnemyGrabable>(out var enemy))
        {
            enemy.Paralyze(); // <--- LLAMADA CLAVE
            Destroy(gameObject); // La bala desaparece al impactar
            return;
        }

        // 2. Colisión con Paredes/Suelo
        // Usamos operaciones de bits para ver si la capa del objeto está en la máscara
        if (((1 << other.gameObject.layer) & whatIsObstacle) != 0)
        {
            Destroy(gameObject); // Chocó con pared
        }
    }

    public void SetDirection(bool isFacingRight)
    {
        moveRight = isFacingRight;
        if (rb != null) ApplyMovement();
    }

    private void ApplyMovement()
    {
        float directionMultiplier = moveRight ? 1f : -1f;

        // Unity 6: linearVelocity | Unity 2022: velocity
        rb.linearVelocity = new Vector2(speed * directionMultiplier, 0f);

        Vector3 newScale = transform.localScale;
        newScale.x = moveRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }
}