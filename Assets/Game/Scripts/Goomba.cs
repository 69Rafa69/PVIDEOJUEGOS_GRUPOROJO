using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Goomba : MonoBehaviour, IParalyzable
{
    [Header("Movimiento")]
    [SerializeField] private float speedX = 3f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distanciaSuelo = 1.0f;
    [SerializeField] private bool moviendoDerecha = true;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Parálisis")]
    [SerializeField] private float paralysisDuration = 20f;
    // (Borrado) paralysisCooldown ya no es necesario

    [Header("Audio")]
    [SerializeField] private AudioClip paralyzeSound;
    [SerializeField] private AudioClip reviveSound;
    [SerializeField, Range(0f, 0.5f)] private float hitSoundDelay = 0.15f;

    private Rigidbody2D body;
    private Animator animator;
    private AudioSource audioSource;

    public bool isParalyzed { get; private set; } = false;
    // (Borrado) isImmune ya no es necesario

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        // CAMBIO: Ya solo revisamos si está paralizado actualmente.
        // Si ya está en el suelo, ignoramos el disparo para no reiniciar la animación glitchy.
        if (isParalyzed) return;

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

        // --- AUDIO (Con delay) ---
        yield return new WaitForSeconds(hitSoundDelay);

        if (audioSource != null && paralyzeSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(paralyzeSound);
        }
        // -------------------------

        // Restamos el delay a la duración total
        yield return new WaitForSeconds(paralysisDuration - hitSoundDelay);

        // Esperar a que toque suelo (por si cayó de una plataforma)
        yield return new WaitUntil(() => body.IsTouchingLayers(capaSuelo));

        // 🧠 Desactivar parálisis
        isParalyzed = false;
        animator.SetBool("isParalyzed", false);

        // ✨ Activar trigger para relive
        animator.SetTrigger("revive");

        if (audioSource != null && reviveSound != null)
            audioSource.PlayOneShot(reviveSound);

        // Detener movimiento durante relive
        body.linearVelocity = Vector2.zero;

        // Esperar la duración de la animación de revivir
        yield return new WaitForSeconds(1.5f);

        // Restaurar movimiento normal
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

        if (audioSource != null) audioSource.pitch = 1f;

        // (Borrado) Ya no iniciamos la rutina de Cooldown aquí
    }

    // (Borrado) El método IEnumerator ParalysisCooldownRoutine ha sido eliminado

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
        // Solo hace daño si NO está paralizado
        if (collision.gameObject.CompareTag("Player") && !isParalyzed)
        {
            PlayerDetectionHit playerHit = collision.gameObject.GetComponent<PlayerDetectionHit>();
            if (playerHit != null)
                playerHit.SendMessage("SpawnPlayer");
        }
    }
}