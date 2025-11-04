using UnityEngine;

public class LaserBulletScript : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private bool moveRight = true;

    [Header("Animaci�n")]
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 0.6f; // m�s lento = efecto el�ctrico suave

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Movimiento inicial seg�n direcci�n
        float dir = moveRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(speed * dir, 0f);

        // Gira sprite si va hacia la izquierda
        if (!moveRight)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Start()
    {
        // Activa animaci�n continua
        if (animator != null)
        {
            animator.Play("Laser_Anim", 0, 0f);
            animator.speed = animationSpeed;
        }

        // Destruye despu�s de un tiempo
        Destroy(gameObject, duration);
    }

    public void SetDirection(bool right)
    {
        moveRight = right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si impacta algo que implementa IParalyzable o da�o
        IParalyzable paralyzable = other.GetComponent<IParalyzable>();
        if (paralyzable != null)
        {
            paralyzable.Paralyze();
            Destroy(gameObject);
        }
    }
}
