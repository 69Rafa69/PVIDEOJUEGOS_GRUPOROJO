using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] // <--- NUEVO: Asegura que tenga AudioSource
public class Sentry_Movement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speedX;
    [SerializeField] private float limitRight;
    [SerializeField] private float limitLeft;

    private Vector2 limits;
    private int direction;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector3 originalPosition;

    [Header("Disparo")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject sentryAttackPrefab;
    [SerializeField] private float shootCooldown = 1.5f;

    [Header("Audio")] // <--- NUEVO: Sección de Audio
    [SerializeField] private AudioClip shootSound;

    private Transform player;
    private float shootTimer;
    private AudioSource audioSource; // <--- NUEVO: Referencia al componente

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // <--- NUEVO: Capturamos el componente

        Vector3 pos = transform.localPosition;
        originalPosition = pos;
        limits = new Vector2(pos.x - limitLeft, pos.x + limitRight);

        sprite = GetComponent<SpriteRenderer>();
        direction = 1; // Hacia la derecha
    }

    private void Start()
    {
        // buscar al jugador por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        DetectarYDisparar();
        shootTimer -= Time.deltaTime;

        // Lógica de movimiento
        if (direction != 0)
        {
            sprite.flipX = direction < 0 ? true : false;
        }

        Vector3 pos = transform.localPosition;
        if (pos.x <= limits.x)
        {
            direction = 1;
        }
        if (pos.x >= limits.y)
        {
            direction = -1;
        }
        body.linearVelocityX = direction * speedX;
    }

    private void DetectarYDisparar()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);
        if (distancia <= detectionRange)
        {
            // mirar al jugador
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (shootTimer <= 0f)
            {
                Disparar(player.position);
                shootTimer = shootCooldown;
            }
        }
    }

    private void Disparar(Vector3 targetPosition)
    {
        if (sentryAttackPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Prefab o firePoint no asignado en " + name);
            return;
        }

        // 1. Instanciar la bala
        Vector3 dir = (targetPosition - firePoint.position).normalized;
        GameObject bullet = Instantiate(sentryAttackPrefab, firePoint.position, Quaternion.identity);
        Projectile_Sentry proj = bullet.GetComponent<Projectile_Sentry>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }

        // 2. Reproducir sonido <--- NUEVO
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = originalPosition != Vector3.zero ? originalPosition : transform.localPosition;
        Vector3 posLeft = new Vector3(pos.x - limitLeft, pos.y, pos.z);
        Vector3 posRight = new Vector3(pos.x + limitRight, pos.y, pos.z);
        Gizmos.DrawSphere(posLeft, 0.5f);
        Gizmos.DrawSphere(posRight, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}