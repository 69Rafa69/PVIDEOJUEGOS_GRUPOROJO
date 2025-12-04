using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Estabilidad de salto")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float groundBuffer = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float stepRate = 0.35f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite; // Ya no lo usamos para girar, pero lo mantengo por si acaso
    private Animator animator;
    private AudioSource audioSource;

    private InputAction moveAction;
    private InputAction jumpAction;

    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float lastGroundedTime;

    // Estado de dirección (Asumimos que el dibujo original mira a la derecha)
    private bool facingRight = true;

    // Variables para control de audio
    private float nextStepTime;
    private float lastLandTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;

        bool groundCheckNow = CheckGround();

        if (groundCheckNow)
            lastGroundedTime = Time.time;

        isGrounded = groundCheckNow || (Time.time - lastGroundedTime <= coyoteTime);

        // --- SONIDO ATERRIZAJE ---
        if (!wasGrounded && groundCheckNow && rb.linearVelocity.y < -0.5f && Time.time > lastLandTime + 0.2f)
        {
            PlaySound(landSound, 0.7f);
            lastLandTime = Time.time;
        }

        // Lógica de pasos (Caminar)
        if (isGrounded && Mathf.Abs(moveInput) > 0.05f && Time.time >= nextStepTime)
        {
            PlaySound(footstepSound, Random.Range(0.8f, 1f));
            nextStepTime = Time.time + stepRate;
        }

        animator.SetBool("isGrounded", groundCheckNow);
        animator.SetBool("isWalking", Mathf.Abs(moveInput) > 0.05f);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (jumpAction.WasPressedThisFrame() && isGrounded)
            Jump();

        wasGrounded = groundCheckNow;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // --- CORRECCIÓN DE GIRO ---
        // En lugar de flipX, llamamos a la función que voltea el Transform completo
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    // Esta función voltea al padre y a TODOS sus hijos (DropPoint, GrabZone, etc.)
    private void Flip()
    {
        facingRight = !facingRight;

        // Multiplicamos la escala X por -1
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger("Jump");
    }

    private bool CheckGround()
    {
        Vector2 origin = groundCheck.position;
        float offset = 0.2f;

        RaycastHit2D leftRay = Physics2D.Raycast(origin + Vector2.left * offset, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D rightRay = Physics2D.Raycast(origin + Vector2.right * offset, Vector2.down, groundCheckDistance, groundLayer);

        return leftRay.collider != null || rightRay.collider != null;
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}