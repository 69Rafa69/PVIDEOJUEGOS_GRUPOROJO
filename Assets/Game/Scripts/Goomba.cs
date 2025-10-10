using UnityEngine;
using System.Collections;

public class Goomba : MonoBehaviour, IParalyzable
{
    [Header("Movimiento")]
    [SerializeField] private float speedX;
    [SerializeField] private float limitRight;
    [SerializeField] private float limitLeft;

    [Header("Parálisis")]
    [SerializeField] private float paralysisDuration = 15f;
    [SerializeField] private float paralysisCooldown = 5f; // ⏳ tiempo de inmunidad

    private Vector2 limits;
    private int direction;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector3 originalPosition;

    public bool isParalyzed { get; private set; } = false;
    private bool isImmune = false;

    private void Awake()
    {
        Vector3 pos = transform.localPosition;
        originalPosition = pos;
        limits = new Vector2(pos.x - limitLeft, pos.x + limitRight);

        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        direction = 1; // empieza moviéndose a la derecha
    }

    private void Update()
    {
        // solo se mueve si no está paralizado
        if (!isParalyzed)
        {
            if (direction != 0)
                sprite.flipX = direction < 0;

            Vector3 pos = transform.localPosition;
            if (pos.x <= limits.x) direction = 1;
            if (pos.x >= limits.y) direction = -1;

            body.linearVelocityX = direction * speedX;
        }
        else
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    public void Paralyze()
    {
        if (isParalyzed || isImmune) return; // ⛔ evita stun spam

        Debug.Log($"Goomba paralizado durante {paralysisDuration} segundos");
        StartCoroutine(ParalysisRoutine());
    }

    private IEnumerator ParalysisRoutine()
    {
        isParalyzed = true;

        // Cambiar color a gris
        if (sprite != null)
            sprite.color = Color.gray;

        // Aplicar físicas para que caiga si corresponde
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 2f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        yield return new WaitForSeconds(paralysisDuration);

        // Restaurar estado
        isParalyzed = false;

        // Quitar físicas y restaurar movimiento
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        direction = 1;

        // Iniciar cooldown de inmunidad
        StartCoroutine(ParalysisCooldownRoutine());
    }

    private IEnumerator ParalysisCooldownRoutine()
    {
        isImmune = true;

        // Color amarillo → inmunidad
        if (sprite != null)
            sprite.color = Color.yellow;

        Debug.Log($"Goomba inmune a la parálisis durante {paralysisCooldown} segundos");

        yield return new WaitForSeconds(paralysisCooldown);

        isImmune = false;

        // Regresar al color normal
        if (sprite != null)
            sprite.color = Color.white;
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = originalPosition != Vector3.zero ? originalPosition : transform.localPosition;
        Vector3 posLeft = new Vector3(pos.x - limitLeft, pos.y, pos.z);
        Vector3 posRight = new Vector3(pos.x + limitRight, pos.y, pos.z);
        Gizmos.DrawSphere(posLeft, 0.5f);
        Gizmos.DrawSphere(posRight, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isParalyzed)
        {
            PlayerDetectionHit playerHit = collision.gameObject.GetComponent<PlayerDetectionHit>();
            if (playerHit != null)
            {
                playerHit.SendMessage("SpawnPlayer");
            }
        }
    }
}
