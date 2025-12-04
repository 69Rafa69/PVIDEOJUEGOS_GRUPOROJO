using UnityEngine;
using System.Collections;

public class EnemyGrabable : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Layer segura para convertir al enemigo en plataforma (ej. Ground).")]
    [SerializeField] private string groundLayerName = "Ground";

    [Header("Respawn")]
    [SerializeField] private float respawnTime = 2f;

    // Estado interno
    private bool isGrabbed;
    private bool isParalyzed;

    private Transform originalParent;
    private Vector3 startPosition;

    // Identidad original
    private int defaultLayer;
    // (Borrado defaultTag porque ya no vamos a cambiar el tag)
    private int groundLayerID;

    // Componentes
    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private CapsuleCollider2D col;
    private Goomba ai;
    private Animator animator; // <--- NUEVO: Necesitas esto para la animación

    private void Awake()
    {
        originalParent = transform.parent;
        startPosition = transform.position;

        defaultLayer = gameObject.layer;

        groundLayerID = LayerMask.NameToLayer(groundLayerName);

        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        ai = GetComponent<Goomba>();
        animator = GetComponent<Animator>(); // <--- NUEVO

        if (groundLayerID == -1) Debug.LogError("EnemyGrabable: ¡Falta la layer Ground!");
    }

    public void Paralyze()
    {
        if (isGrabbed || isParalyzed) return;

        isParalyzed = true;
        ConvertIntoBox();
    }

    private void ConvertIntoBox()
    {
        // 1. Apagar cerebro (Esto ya evita que haga daño gracias al check en Goomba)
        if (ai != null) ai.enabled = false;

        // 2. ANIMACIÓN DE PARÁLISIS <--- NUEVO
        if (animator != null)
        {
            animator.SetBool("isParalyzed", true);
        }

        // 3. Física de estatua
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeAll;

        // 4. Identidad
        gameObject.layer = groundLayerID; // Layer Ground (para saltar encima)

        // --- CORRECCIÓN IMPORTANTE ---
        // NO CAMBIES EL TAG A "UNTAGGED". 
        // Tu script PlayerGrab busca "EnemyGrab". Si lo quitas, no puedes agarrarlo.
        // gameObject.tag = "Untagged"; // <--- LÍNEA ELIMINADA

        // 5. Visual
        // (Opcional: Si tienes animación, quizás no quieras cambiar el color a Cyan)
        // sprite.color = Color.cyan; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CheckHazard(other.collider);

        // Si es caja voladora y choca con enemigo
        if (!isGrabbed && body.linearVelocity.magnitude > 1f && other.gameObject.GetComponent<Goomba>() != null)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckHazard(other);
    }

    private void CheckHazard(Collider2D other)
    {
        if (!isGrabbed && other.CompareTag("DeadZone"))
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    public void ChangeEnemyStatus(Transform grabZone)
    {
        isGrabbed = true;
        StopAllCoroutines();

        body.simulated = false;
        col.enabled = false;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.layer = defaultLayer;

        transform.SetParent(grabZone);
        transform.localPosition = Vector3.zero;

        // Ocultamos sprite (la animación ya no importa aquí porque es invisible)
        sprite.enabled = false;
    }

    public void ThrowEnemy(Vector2 direction, float force)
    {
        isGrabbed = false;
        transform.SetParent(originalParent);

        sprite.enabled = true;
        col.enabled = true;
        body.simulated = true;

        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.linearVelocity = Vector2.zero;
        body.AddForce(direction * force, ForceMode2D.Impulse);

        StartCoroutine(LockOnLand());
    }

    private IEnumerator LockOnLand()
    {
        yield return new WaitForSeconds(0.15f);

        while (Mathf.Abs(body.linearVelocity.y) > 0.1f)
        {
            yield return null;
        }

        if (isParalyzed)
        {
            ConvertIntoBox(); // Re-aplicamos estado
            Debug.Log("Caja aterrizó.");
        }
    }

    private IEnumerator RespawnRoutine()
    {
        sprite.enabled = false;
        body.simulated = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        transform.SetParent(originalParent);
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        sprite.enabled = true;
        col.enabled = true;
        body.simulated = true;
        isGrabbed = false;

        if (isParalyzed)
        {
            // Renace como caja
            ConvertIntoBox();
        }
        else
        {
            // Renace vivo
            body.linearVelocity = Vector2.zero;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            gameObject.layer = defaultLayer;

            if (animator != null) animator.SetBool("isParalyzed", false); // Reset anim
            if (ai != null) ai.enabled = true;
        }
    }
}