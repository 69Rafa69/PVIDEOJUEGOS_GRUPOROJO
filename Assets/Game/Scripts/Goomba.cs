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
    private SpriteRenderer sprite;
    public bool isParalyzed { get; private set; } = false; // 🔹 cumple con la interfaz
    private bool isImmune = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isParalyzed)
        {
            body.linearVelocity = new Vector2(0, body.linearVelocity.y);
            return;
        }

        // Movimiento horizontal
        float dir = moviendoDerecha ? 1 : -1;
        body.linearVelocity = new Vector2(dir * speedX, body.linearVelocity.y);

        // Detección de suelo con Raycast
        RaycastHit2D infoSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);

        // Si no hay suelo, gira
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

        if (sprite != null)
            sprite.color = Color.gray;

        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 4f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        yield return new WaitForSeconds(paralysisDuration);

        // Espera a tocar suelo
        yield return new WaitUntil(() => body.IsTouchingLayers(capaSuelo));

        // Restaurar movimiento
        isParalyzed = false;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

        StartCoroutine(ParalysisCooldownRoutine());
    }

    private IEnumerator ParalysisCooldownRoutine()
    {
        isImmune = true;

        if (sprite != null)
            sprite.color = Color.yellow;

        yield return new WaitForSeconds(paralysisCooldown);

        isImmune = false;

        if (sprite != null)
            sprite.color = Color.white;
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
