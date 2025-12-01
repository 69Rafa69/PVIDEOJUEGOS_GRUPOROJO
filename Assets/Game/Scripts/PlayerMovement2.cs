using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de personaje 2D "Refinado".
/// Implementa físicas avanzadas como Coyote Time, suavizado de movimiento y eventos de aterrizaje.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player_Movement_Refined : MonoBehaviour
{
    [Header("Física y Movimiento")]
    [Tooltip("Velocidad máxima horizontal.")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Tooltip("Tiempo que tarda en alcanzar la velocidad máxima. 0 = instantáneo, >0 = sensación de inercia/hielo.")]
    [Range(0f, 0.3f)]
    [SerializeField] private float movementSmoothing = 0.05f;

    [Tooltip("Permite mover al personaje mientras está en el aire.")]
    [SerializeField] private bool airControl = true;

    [Header("Sensores de Entorno")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Assist (Game Feel)")]
    [Tooltip("Tiempo extra para saltar después de caer de una plataforma (mejora la jugabilidad).")]
    [SerializeField] private float coyoteTime = 0.1f;

    [Tooltip("Umbral de velocidad vertical para considerar que se ha aterrizado establemente.")]
    [SerializeField] private float groundBuffer = 0.05f;

    [Header("Eventos Externos")]
    [Tooltip("Invoca efectos (sonido, partículas) al tocar el suelo.")]
    public UnityEvent OnLandEvent;

    // Referencias internas
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Renombrado para claridad
    private Animator animator;

    // Referencias de Input
    private InputAction moveAction;
    private InputAction jumpAction;

    // Estado del Controlador
    private float horizontalInput;
    private bool isGrounded;
    private bool wasGrounded;         // Para detectar el flanco de bajada (Landing)
    private float lastGroundedTime;   // Timestamp para calcular Coyote Time
    private bool facingRight = true;

    // Variable auxiliar para el cálculo de SmoothDamp
    private Vector3 velocitySmoothing = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Inicialización defensiva del evento para evitar NullReferenceException
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void OnEnable()
    {
        // TODO: Mantenimiento - Buscar acciones por string ("Move") es frágil. 
        // Si cambias el nombre en el Input Action Asset, esto dejará de funcionar silenciosamente.
        // Se recomienda usar referencias directas (InputActionReference) en el inspector.
        var actionMap = InputSystem.actions;
        if (actionMap != null)
        {
            moveAction = actionMap.FindAction("Move");
            jumpAction = actionMap.FindAction("Jump");
        }
    }

    private void Update()
    {
        // 1. Lectura de Inputs (Siempre en Update para respuesta instantánea)
        if (moveAction != null)
            horizontalInput = moveAction.ReadValue<Vector2>().x;

        // 2. Lógica de Sensores
        bool groundNow = CheckGround();

        if (groundNow)
            lastGroundedTime = Time.time;

        // Lógica de Coyote Time:
        // Es "Grounded" si estamos tocando el suelo AHORA o si lo tocamos hace muy poco tiempo.
        isGrounded = groundNow || (Time.time - lastGroundedTime <= coyoteTime);

        // 3. Gestión de Animaciones
        animator.SetBool("isWalking", Mathf.Abs(horizontalInput) > 0.05f);
        animator.SetBool("isGrounded", groundNow);

        // 4. Lógica de Salto
        // Usamos isGrounded (que incluye CoyoteTime) para permitir el salto
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }

        // 5. Detección de Aterrizaje
        // Si antes NO estaba en el suelo, y AHORA sí, y no estoy saliendo disparado hacia arriba...
        if (!wasGrounded && groundNow && Mathf.Abs(rb.linearVelocity.y) < groundBuffer)
        {
            OnLandEvent.Invoke();
            animator.SetTrigger("Land");
        }

        wasGrounded = groundNow;
    }

    private void FixedUpdate()
    {
        // La física siempre va en FixedUpdate para ser determinista
        ApplyMovement(horizontalInput);
        HandleFlipping();
    }

    private void ApplyMovement(float move)
    {
        // Solo movemos si estamos en el suelo O si se permite control aéreo
        if (isGrounded || airControl)
        {
            // Calculamos la velocidad deseada
            Vector3 targetVelocity = new Vector2(move * speed, rb.linearVelocity.y);

            // Vector3.SmoothDamp:
            // Interpola suavemente desde la velocidad actual hacia la deseada.
            // Esto elimina el movimiento "robótico" instantáneo y añade peso al personaje.
            // NOTA: 'linearVelocity' es exclusivo de Unity 6. Usar 'velocity' en versiones anteriores.
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocitySmoothing, movementSmoothing);
        }
    }

    private void Jump()
    {
        // Resetear la velocidad Y es CRÍTICO para saltos consistentes.
        // Si no lo haces, saltar mientras caes anula parte de la fuerza del salto.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // ForceMode2D.Impulse es ideal para fuerzas instantáneas (explosiones, saltos)
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Feedback visual inmediato
        animator.ResetTrigger("Land"); // Evita bugs visuales si saltas justo al aterrizar
        animator.SetTrigger("Jump");

        // Hack: Reseteamos lastGroundedTime para evitar saltos infinitos dentro del tiempo de Coyote
        lastGroundedTime = -10f;
        isGrounded = false;
    }

    private void HandleFlipping()
    {
        // Invertimos la escala solo si la dirección del input es opuesta a la dirección actual
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            facingRight = !facingRight;

            // Invertimos el LocalScale en X.
            // Nota: Esto invierte también a los hijos (puntos de disparo, detectores).
            // Si esto causa problemas, usar spriteRenderer.flipX en su lugar.
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private bool CheckGround()
    {
        // OverlapCircle es más eficiente y permisivo que Raycast para plataformas 2D
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}