using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Transform grabZone;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropRespawnDelay = 0.8f; // ⏳ tiempo antes de mostrar al enemigo

    private InputAction interactAction;
    private EnemyGrabable currentEnemy;
    private EnemyGrabable grabbedEnemy;
    private bool isGrabbing = false;

    private Animator animator;
    private InputAction moveAction;

    private void Awake()
    {
        // Crear dropPoint si no está asignado
        if (dropPoint == null)
        {
            GameObject autoDrop = new GameObject("AutoDropPoint");
            autoDrop.transform.SetParent(transform);
            autoDrop.transform.localPosition = new Vector3(0.5f, -0.2f, 0);
            dropPoint = autoDrop.transform;

            Debug.LogWarning("⚠ DropPoint no asignado. Creado automáticamente.");
        }
    }

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        moveAction = InputSystem.actions.FindAction("Move");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // --- ANIMACIONES SOLO SI ESTÁ SOSTENIENDO ---
        if (isGrabbing)
        {
            float moveX = Mathf.Abs(moveAction.ReadValue<Vector2>().x);
            animator.SetBool("isWalking", moveX > 0.1f);
            animator.SetBool("isHolding", true);
        }
        else
        {
            animator.SetBool("isHolding", false);
            // ❌ YA NO PISAMOS isWalking = false (lo maneja el script principal)
        }

        // --- INPUT ---
        if (interactAction.WasPressedThisFrame())
        {
            if (!isGrabbing && currentEnemy != null)
            {
                // GRAB
                grabbedEnemy = currentEnemy;
                grabbedEnemy.ChangeEnemyStatus(grabZone);

                isGrabbing = true;
                animator.SetTrigger("Pickup");
            }
            else if (isGrabbing && grabbedEnemy != null)
            {
                // DROP
                StartCoroutine(HandleDrop());
            }
        }
    }

    private System.Collections.IEnumerator HandleDrop()
    {
        animator.SetTrigger("Drop");

        grabbedEnemy.transform.SetParent(null);
        grabbedEnemy.transform.position = dropPoint.position;

        // Ocultar y desactivar por un tiempo
        grabbedEnemy.HideTemporarily(dropRespawnDelay);

        // liberar estado
        isGrabbing = false;
        grabbedEnemy = null;

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isGrabbing && collision.CompareTag("EnemyGrab"))
        {
            currentEnemy = collision.GetComponent<EnemyGrabable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isGrabbing && collision.CompareTag("EnemyGrab") &&
            collision.GetComponent<EnemyGrabable>() == currentEnemy)
        {
            currentEnemy = null;
        }
    }
}
