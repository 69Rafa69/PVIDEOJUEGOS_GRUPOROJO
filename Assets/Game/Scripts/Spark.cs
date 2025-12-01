using UnityEngine;

/// <summary>
/// Controla el comportamiento del proyectil "Spark".
/// Maneja movimiento lineal, ciclo de vida (tiempo) e interacciones con IParalyzable.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // Asegura que el componente físico exista para evitar NullReference.
public class Spark : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Tiempo en segundos antes de que el proyectil se autodestruya.")]
    [SerializeField] private float duration = 10f;

    [Header("Física y Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool moveRight = true;

    [Header("Visuales")]
    [Tooltip("Opcional. Si se asigna, se fuerza el estado 'Spark_Anim' al iniciar.")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private void Awake()
    {
        // Inicializamos referencias en Awake en lugar de Start.
        // Esto previene errores si SetDirection() es llamado inmediatamente después de instanciar.
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Configuración inicial de movimiento
        ApplyMovement();

        // Feedback visual inicial
        if (animator != null)
        {
            animator.Play("Spark_Anim", 0, 0f);
        }

        // Gestión automática del ciclo de vida (Memory Management)
        Destroy(gameObject, duration);
    }

    /// <summary>
    /// Detecta colisiones tipo Trigger.
    /// Filtra objetos que implementen la interfaz IParalyzable.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Optimización: TryGetComponent es más eficiente y limpio que GetComponent + check de null.
        if (other.TryGetComponent<IParalyzable>(out var paralyzable))
        {
            paralyzable.Paralyze();
            Destroy(gameObject); // El proyectil se consume al impactar
        }
    }

    /// <summary>
    /// Configura la dirección del proyectil desde una clase externa (ej. PlayerShoot).
    /// </summary>
    /// <param name="isFacingRight">True para derecha, False para izquierda.</param>
    public void SetDirection(bool isFacingRight)
    {
        moveRight = isFacingRight;

        // Aplicamos el movimiento inmediatamente si el objeto ya está inicializado
        if (rb != null)
        {
            ApplyMovement();
        }
    }

    /// <summary>
    /// Aplica la velocidad y rotación visual basada en el estado actual.
    /// Centraliza la lógica para evitar código duplicado en Start y SetDirection.
    /// </summary>
    private void ApplyMovement()
    {
        // Cálculo de dirección
        float directionMultiplier = moveRight ? 1f : -1f;

        // NOTA: 'linearVelocity' es exclusivo de Unity 6. Usar 'velocity' en versiones anteriores.
        rb.linearVelocity = new Vector2(speed * directionMultiplier, 0f);

        // Flip visual
        // TODO: Considerar usar SpriteRenderer.flipX si el objeto es simple, es más performante que modificar localScale.
        Vector3 newScale = transform.localScale;
        newScale.x = moveRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }
}