using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de personaje para plataformas 2D.
/// Separa la lógica de Input (Update) de la simulación física (FixedUpdate) para un movimiento fluido.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovePlatform : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpImpulse = 10f;

    [Header("Detección de Suelo")]
    [Tooltip("Transform vacío ubicado en los pies del personaje.")]
    [SerializeField] private Transform detector;
    [SerializeField] private float sizeDetector = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Referencias a inputs
    private InputAction moveAction;
    private InputAction jumpAction;

    // Componentes
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Estado interno
    private Vector2 currentInput;
    private bool isGrounded;
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // TODO: Mantenimiento - Buscar acciones por string ("Move") es frágil. 
        // Si cambias el nombre en el Input Action Asset, esto fallará.
        // Considera usar la clase generada de C# o el componente PlayerInput.
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("PlayerMovePlatform: No se encontraron las acciones 'Move' o 'Jump'. Revisa el Input System.");
        }
    }

    /// <summary>
    /// Ciclo Variable: Se ejecuta cada frame visual.
    /// AQUÍ leemos los inputs para garantizar respuesta inmediata.
    /// </summary>
    private void Update()
    {
        // 1. Lectura de Input de movimiento
        currentInput = moveAction.ReadValue<Vector2>();

        // 2. Detección de salto (Input buffering simple)
        // Usamos WasPressedThisFrame en Update para no perder clics entre frames físicos.
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            jumpRequested = true;
        }

        // 3. Feedback Visual (Flip)
        // Es puramente visual, puede ir en Update
        if (currentInput.x != 0)
        {
            spriteRenderer.flipX = currentInput.x < 0;
        }
    }

    /// <summary>
    /// Ciclo Fijo: Se ejecuta cada 0.02s (por defecto).
    /// AQUÍ aplicamos fuerzas y velocidades al Rigidbody.
    /// </summary>
    private void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
        ApplyJump();
    }

    private void CheckGround()
    {
        // Physics2D.OverlapCircle es eficiente para este check.
        Collider2D colision = Physics2D.OverlapCircle(detector.position, sizeDetector, groundLayer);
        isGrounded = colision != null;
    }

    private void ApplyMovement()
    {
        // Lógica de "Movimiento Digital" (Snapping):
        // Ignoramos la magnitud del stick (0.1 a 1.0) y lo convertimos a -1, 0, 1.
        // Esto da control preciso tipo "Retro", evitando deslizamientos lentos.
        float directionX = 0f;
        if (currentInput.x != 0)
        {
            directionX = currentInput.x > 0 ? 1f : -1f;
        }

        // Aplicamos velocidad solo en X, manteniendo la velocidad Y actual (gravedad)
        // NOTA: 'linearVelocity' es de Unity 6. Usar 'velocity' en versiones anteriores.
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
    }

    private void ApplyJump()
    {
        if (jumpRequested)
        {
            // Resetear velocidad Y antes de saltar asegura altura consistente 
            // incluso si estamos cayendo ligeramente al tocar el suelo.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

            Debug.Log("Salto ejecutado.");
            jumpRequested = false; // Consumimos el input
        }
    }

    // Visualización en el Editor para ajustar el detector sin adivinar
    private void OnDrawGizmos()
    {
        if (detector != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(detector.position, sizeDetector);
        }
    }
}