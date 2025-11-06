using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player_Movement_Refined : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [Range(0f, 0.3f)][SerializeField] private float movementSmoothing = 0.05f;
    [SerializeField] private bool airControl = true;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Estabilidad de salto")]
    [SerializeField] private float coyoteTime = 0.1f;   // margen tras dejar el suelo
    [SerializeField] private float groundBuffer = 0.05f; // tolerancia al aterrizar

    [Header("Eventos")]
    public UnityEvent OnLandEvent; // Invocado al aterrizar

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    // Input System
    private InputAction moveAction;
    private InputAction jumpAction;

    // Estado interno
    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float lastGroundedTime;
    private bool facingRight = true;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void OnEnable()
    {
        // Vincular acciones de Input System
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        // Leer entrada de movimiento horizontal
        moveInput = moveAction.ReadValue<Vector2>().x;

        // Comprobar si está en el suelo
        bool groundNow = CheckGround();

        // Actualizar tiempo del último contacto con suelo
        if (groundNow)
            lastGroundedTime = Time.time;

        // Permitir un pequeño margen (coyote time)
        isGrounded = groundNow || (Time.time - lastGroundedTime <= coyoteTime);

        // Animaciones básicas
        animator.SetBool("isWalking", Mathf.Abs(moveInput) > 0.05f);
        animator.SetBool("isGrounded", groundNow);

        // Saltar
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            Jump();

        // Evento de aterrizaje (solo si recién tocó suelo estable)
        if (!wasGrounded && groundNow && Mathf.Abs(rb.linearVelocity.y) < groundBuffer)
        {
            OnLandEvent.Invoke();
            animator.SetTrigger("Land");
        }

        wasGrounded = groundNow;
    }

    private void FixedUpdate()
    {
        Move(moveInput);

        // Voltear el sprite según dirección
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    // ---- Lógica de movimiento principal (suavizado tipo CharacterController2D) ----
    private void Move(float move)
    {
        if (isGrounded || airControl)
        {
            // Calcula la velocidad objetivo
            Vector3 targetVelocity = new Vector2(move * speed, rb.linearVelocity.y);

            // Aplica suavizado (SmoothDamp)
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, movementSmoothing);
        }
    }

    // ---- Salto ----
    private void Jump()
    {
        // Reiniciar velocidad vertical y aplicar impulso
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        animator.ResetTrigger("Land");
        animator.SetTrigger("Jump");
    }

    // ---- Detección de suelo (OverlapCircle) ----
    private bool CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return hit != null;
    }

    // ---- Cambiar dirección ----
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ---- Debug visual del GroundCheck ----
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
