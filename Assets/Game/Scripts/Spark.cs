using UnityEngine;

public class Spark : MonoBehaviour
{
    [Header("Duraci�n")]
    [SerializeField] private float duration = 10f;

    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool moveRight = true;

    [Header("Animaci�n")]
    [SerializeField] private Animator animator; // ? nuevo campo opcional para reproducir animaci�n

    private Rigidbody2D rb;

    // Si colisiona con un enemigo
    void OnTriggerEnter2D(Collider2D other) // Cambiado a OnTriggerEnter2D
    {
        IParalyzable paralyzable = other.GetComponent<IParalyzable>(); // Llamar a la interfaz
        if (paralyzable != null) // Solo si es un enemigo paralizable
        {
            paralyzable.Paralyze();  // Paralizarlo
            Destroy(gameObject);    // Destruir la bala
        }
    }

    // Si no colisiona
    void Start()
    {
        // ?? Configuraci�n de Rigidbody y movimiento inicial
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = moveRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(speed * direction, 0f);

            // ?? Ajuste visual seg�n direcci�n
            if (!moveRight)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // ?? Si hay animador, iniciar animaci�n del Spark
        if (animator != null)
            animator.Play("Spark_Anim", 0, 0f);

        // ?? Destruye despu�s de la duraci�n especificada
        Destroy(gameObject, duration);
    }

    // ? Nuevo m�todo p�blico para definir direcci�n desde PlayerShoot
    public void SetDirection(bool right)
    {
        moveRight = right;
        if (rb != null)
        {
            float direction = moveRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(speed * direction, 0f);
            if (!moveRight)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
