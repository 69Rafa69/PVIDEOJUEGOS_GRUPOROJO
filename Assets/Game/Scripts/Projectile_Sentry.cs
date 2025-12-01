using UnityEngine;

/// <summary>
/// Controla la balística y colisiones del proyectil de la torreta.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile_Sentry : MonoBehaviour
{
    [Header("Balística")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Configuración de Colisión")]
    [Tooltip("Define qué capas se consideran suelo/obstáculos para destruir la bala.")]
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Gestión automática de memoria: Destruye la bala si no golpea nada.
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        // Movimiento físico constante
        // NOTA: 'linearVelocity' es de Unity 6. Usar 'velocity' para versiones anteriores.
        rb.linearVelocity = moveDirection * speed;
    }

    /// <summary>
    /// Configura la trayectoria y rota el sprite para que mire hacia adelante.
    /// </summary>
    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;

        // Rotación matemática basada en el vector de dirección (Atan2 devuelve radianes)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Lógica de Jugador
        // TODO: Actualmente la bala ignora al jugador. 
        // Si esto es una trampa, aquí deberías llamar a 'PlayerHealth.TakeDamage()'.
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Golpeó al jugador (Sin daño implementado)");
            return;
        }

        // 2. Interacción con Switches (Mecánica de Puzzle)
        if (other.CompareTag("Switch"))
        {
            // Optimización: TryGetComponent evita excepciones y es más limpio
            if (other.TryGetComponent<Switch_Door>(out var switchDoor))
            {
                switchDoor.Activate();
            }

            Destroy(gameObject); // Consumir bala
            return;
        }

        // 3. Colisión con Entorno (Optimizado)
        // Usamos operaciones a nivel de bits para chequear la LayerMask. 
        // Es mucho más rápido que comparar strings con "NameToLayer".
        if (IsLayerInMask(other.gameObject.layer, obstacleLayers))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Helper para verificar si un layer específico está dentro de una LayerMask.
    /// </summary>
    private bool IsLayerInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}