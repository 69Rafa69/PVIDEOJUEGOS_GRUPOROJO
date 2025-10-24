using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Detecci�n de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Nuevos parámetros de estabilidad de salto
    [SerializeField] private float coyoteTime = 0.1f; // margen de tiempo tras dejar el suelo
    [SerializeField] private float groundBuffer = 0.05f; // amortiguador para evitar falsos aterrizajes

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private InputAction moveAction;
    private InputAction jumpAction;

    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float lastGroundedTime; // almacena el último momento en que tocó el suelo

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        // Leer entrada del jugador
        moveInput = moveAction.ReadValue<Vector2>().x;

        // Comprobar si est� en el suelo
        bool groundCheckNow = CheckGround();

        // Actualizar el tiempo del último contacto con el suelo
        if (groundCheckNow)
            lastGroundedTime = Time.time;

        // Considerar como en suelo si el raycast detecta o si ha pasado poco desde el último contacto
        isGrounded = groundCheckNow || (Time.time - lastGroundedTime <= coyoteTime);

        // Actualizar animaciones
        animator.SetBool("isWalking", Mathf.Abs(moveInput) > 0.05f);
        animator.SetBool("isGrounded", groundCheckNow);

        // Saltar
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            Jump();

        // Detectar aterrizaje (solo cuando realmente toca suelo estable)
        if (!wasGrounded && groundCheckNow && Mathf.Abs(rb.linearVelocity.y) < groundBuffer)
            animator.SetTrigger("Land");

        wasGrounded = groundCheckNow;
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Voltear sprite
        if (Mathf.Abs(moveInput) > 0.05f)
            sprite.flipX = moveInput < 0;
    }

    private void Jump()
    {
        // Reiniciar velocidad vertical y aplicar impulso
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Control de triggers de animación
        animator.ResetTrigger("Land");
        animator.SetTrigger("Jump");
    }

    private bool CheckGround()
    {
        // Raycast doble para detectar suelo
        Vector2 origin = groundCheck.position;
        float offset = 0.2f;

        RaycastHit2D leftRay = Physics2D.Raycast(origin + Vector2.left * offset, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D rightRay = Physics2D.Raycast(origin + Vector2.right * offset, Vector2.down, groundCheckDistance, groundLayer);

        Debug.DrawRay(origin + Vector2.left * offset, Vector2.down * groundCheckDistance, leftRay ? Color.green : Color.red);
        Debug.DrawRay(origin + Vector2.right * offset, Vector2.down * groundCheckDistance, rightRay ? Color.green : Color.red);

        return leftRay.collider != null || rightRay.collider != null;
    }
}
