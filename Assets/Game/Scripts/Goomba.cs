using UnityEngine;
using System.Collections;

public class Goomba : MonoBehaviour, IParalyzable
{
    [Header("Movimiento")]
    [SerializeField] private float speedX = 3f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distanciaSuelo = 1.0f;
    [SerializeField] private bool moviendoDerecha = true;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Parálisis")]
    [SerializeField] private float paralysisDuration = 5f;
    [SerializeField] private float paralysisCooldown = 5f;

    private Rigidbody2D body;
    private Animator animator;

    public bool isParalyzed { get; private set; } = false;
    private bool isImmune = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isParalyzed)
        {
            body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            return;
        }

        float dir = moviendoDerecha ? 1 : -1;
        body.linearVelocity = new Vector2(dir * speedX, body.linearVelocity.y);

        RaycastHit2D infoSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
        if (infoSuelo.collider == null)
        {
            Girar();
        }
    }

    private void Girar()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.eulerAngles = new Vector3(0, moviendoDerecha ? 0 : 180, 0);
    }

    public void Paralyze()
    {
        if (isParalyzed || isImmune) return;
        StartCoroutine(ParalysisRoutine());
    }

    private IEnumerator ParalysisRoutine()
    {
        isParalyzed = true;

        // ⚡ Activar animación de parálisis
        animator.SetBool("isParalyzed", true);

        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 4f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        yield return new WaitForSeconds(paralysisDuration);

        // Esperar a que toque suelo
        yield return new WaitUntil(() => body.IsTouchingLayers(capaSuelo));

        // 🧠 Desactivar parálisis
        isParalyzed = false;
        animator.SetBool("isParalyzed", false);

        // ✨ Activar trigger para relive
        animator.SetTrigger("revive");

        // Detener movimiento durante relive
        body.linearVelocity = Vector2.zero;

        // Esperar la duración de la animación (ajústala según tu clip)
        yield return new WaitForSeconds(1.5f);

        // Restaurar movimiento normal
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

        StartCoroutine(ParalysisCooldownRoutine());
    }

    private IEnumerator ParalysisCooldownRoutine()
    {
        isImmune = true;
        yield return new WaitForSeconds(paralysisCooldown);
        isImmune = false;
    }

    private void OnDrawGizmos()
    {
        if (controladorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(controladorSuelo.position, controladorSuelo.position + Vector3.down * distanciaSuelo);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isParalyzed)
        {
            PlayerDetectionHit playerHit = collision.gameObject.GetComponent<PlayerDetectionHit>();
            if (playerHit != null)
                playerHit.SendMessage("SpawnPlayer");
        }
    }
}
