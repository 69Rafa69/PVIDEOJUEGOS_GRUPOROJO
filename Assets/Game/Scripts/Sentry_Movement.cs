using UnityEngine;

/// <summary>
/// Controlador de IA para un enemigo tipo torreta móvil (Sentry).
/// Maneja patrullaje horizontal, detección de jugador y sistema de disparo con audio.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Sentry_Movement : MonoBehaviour
{
    [Header("Configuración de Patrullaje")]
    [Tooltip("Velocidad de movimiento horizontal.")]
    [SerializeField] private float speedX = 2f;

    [Tooltip("Distancia máxima hacia la derecha desde la posición inicial.")]
    [SerializeField] private float limitRight = 3f;

    [Tooltip("Distancia máxima hacia la izquierda desde la posición inicial.")]
    [SerializeField] private float limitLeft = 3f;

    [Header("Configuración de Combate")]
    [Tooltip("Radio de detección del jugador.")]
    [SerializeField] private float detectionRange = 6f;

    [Tooltip("Punto de origen del proyectil.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("Prefab del proyectil a instanciar.")]
    [SerializeField] private GameObject sentryAttackPrefab;

    [Tooltip("Tiempo en segundos entre disparos.")]
    [SerializeField] private float shootCooldown = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;

    // Referencias y Estado Interno
    private Vector2 moveLimits; // X = Mínimo (Izq), Y = Máximo (Der)
    private int moveDirection = 1; // 1 = Derecha, -1 = Izquierda
    private Vector3 initialLocalPosition;
    private float currentShootTimer;

    // Componentes cacheados
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Transform playerTransform;

    private void Awake()
    {
        // Cacheo de componentes para evitar GetComponent en Update
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Configuración de límites basada en la posición inicial LOCAL
        initialLocalPosition = transform.localPosition;
        moveLimits = new Vector2(initialLocalPosition.x - limitLeft, initialLocalPosition.x + limitRight);
    }

    private void Start()
    {
        // TODO: Optimización - 'FindGameObjectWithTag' es costoso. 
        // En un juego grande, considera usar un Singleton 'GameManager' o un ScriptableObject para referenciar al jugador.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"{name}: No se encontró objeto con tag 'Player'. La IA no atacará.");
        }
    }

    private void Update()
    {
        // Gestión del temporizador
        if (currentShootTimer > 0) currentShootTimer -= Time.deltaTime;

        // Prioridad de comportamiento:
        // 1. Intentar detectar y atacar
        bool isAttacking = TryDetectAndAttack();

        // 2. Si no está atacando (o si quieres que se mueva MIENTRAS ataca), ejecutar patrulla.
        // Aquí asumimos que siempre se mueve, incluso si dispara.
        PerformPatrol();
    }

    /// <summary>
    /// Gestiona el movimiento de vaivén entre los límites definidos.
    /// </summary>
    private void PerformPatrol()
    {
        Vector3 currentPos = transform.localPosition;

        // Verificación de límites
        if (currentPos.x <= moveLimits.x) moveDirection = 1;  // Toca límite izq -> ir derecha
        else if (currentPos.x >= moveLimits.y) moveDirection = -1; // Toca límite der -> ir izquierda

        // Aplicar velocidad física
        // NOTA: 'linearVelocityX' es específico de Unity 6. Usar 'velocity' en versiones 2022/2023.
        rb.linearVelocityX = moveDirection * speedX;

        // IMPORTANTE: Solo giramos el sprite por movimiento si NO estamos detectando al jugador.
        // Si el jugador está cerca, la prioridad de "mirar" la tiene el combate.
        if (playerTransform == null || Vector2.Distance(transform.position, playerTransform.position) > detectionRange)
        {
            FaceDirection(moveDirection);
        }
    }

    /// <summary>
    /// Detecta si el jugador está en rango, lo encara y dispara.
    /// Retorna true si el jugador está en rango.
    /// </summary>
    private bool TryDetectAndAttack()
    {
        if (playerTransform == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 1. Encarar al jugador (Sobrescribe la dirección del patrullaje visualmente)
            int directionToPlayer = playerTransform.position.x > transform.position.x ? 1 : -1;
            FaceDirection(directionToPlayer);

            // 2. Disparar si el cooldown terminó
            if (currentShootTimer <= 0f)
            {
                Shoot(playerTransform.position);
                currentShootTimer = shootCooldown;
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Unifica la lógica de rotación. Usa localScale para asegurar que el 'firePoint' hijo también gire.
    /// Evita usar SpriteRenderer.flipX para objetos compuestos.
    /// </summary>
    private void FaceDirection(int direction)
    {
        // Usamos Math.Abs para asegurar que partimos de una escala positiva antes de invertir
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * (direction > 0 ? 1 : -1);
        transform.localScale = newScale;
    }

    private void Shoot(Vector3 targetPosition)
    {
        if (sentryAttackPrefab == null || firePoint == null)
        {
            Debug.LogError($"{name}: Falta asignar 'AttackPrefab' o 'FirePoint'.");
            return;
        }

        // Calculamos dirección normalizada
        Vector3 shootDirection = (targetPosition - firePoint.position).normalized;

        // Instanciamos el proyectil
        // TODO: Optimización - Para muchos disparos, implementar patrón 'Object Pooling' en lugar de Instantiate/Destroy.
        GameObject projectileObj = Instantiate(sentryAttackPrefab, firePoint.position, Quaternion.identity);

        // Configuramos el proyectil
        if (projectileObj.TryGetComponent<Projectile_Sentry>(out var projectileScript))
        {
            projectileScript.SetDirection(shootDirection);
        }

        // Feedback de Audio
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualización de Debug en el editor
        if (!Application.isPlaying && transform.hasChanged)
        {
            // Estimación visual en modo edición (no exacta porque usa posición actual como centro)
            initialLocalPosition = transform.position;
        }

        // Dibujar rango de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dibujar límites de patrulla
        Gizmos.color = Color.red;
        // Nota: Esto es aproximado en el editor si el objeto se mueve, pero exacto en PlayMode
        Vector3 center = Application.isPlaying ? (Application.isPlaying ? transform.parent != null ? transform.parent.TransformPoint(initialLocalPosition) : initialLocalPosition : transform.position) : transform.position;

        // Simplificación para visualización rápida:
        Vector3 leftMark = new Vector3(initialLocalPosition.x - limitLeft, transform.position.y, transform.position.z);
        Vector3 rightMark = new Vector3(initialLocalPosition.x + limitRight, transform.position.y, transform.position.z);

        // Si estamos en modo Play, usamos las variables calculadas reales, si no, calculamos al vuelo
        if (Application.isPlaying)
        {
            // Ajuste para dibujar correctamente en espacio global
            // (Esta parte es compleja de visualizar en Gizmos simples si hay padres rotados, se mantiene simple)
        }

        Gizmos.DrawLine(leftMark, rightMark);
        Gizmos.DrawSphere(leftMark, 0.2f);
        Gizmos.DrawSphere(rightMark, 0.2f);
    }
}