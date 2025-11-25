using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Transform grabZone;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropRespawnDelay = 0.8f;

    [Header("Audio")] // <--- NUEVO
    [SerializeField] private AudioClip grabSound; // Sonido al levantar
    [SerializeField] private AudioClip dropSound; // Sonido al soltar

    private InputAction interactAction;
    private EnemyGrabable currentEnemy;
    private EnemyGrabable grabbedEnemy;
    private bool isGrabbing = false;

    private Animator animator;
    private InputAction moveAction;
    private AudioSource audioSource; // <--- NUEVO

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // <--- NUEVO

        if (dropPoint == null)
        {
            GameObject autoDrop = new GameObject("AutoDropPoint");
            autoDrop.transform.SetParent(transform);
            autoDrop.transform.localPosition = new Vector3(0.5f, -0.2f, 0);
            dropPoint = autoDrop.transform;
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
        if (isGrabbing)
        {
            float moveX = Mathf.Abs(moveAction.ReadValue<Vector2>().x);
            animator.SetBool("isWalking", moveX > 0.1f);
            animator.SetBool("isHolding", true);
        }
        else
        {
            animator.SetBool("isHolding", false);
        }

        if (interactAction.WasPressedThisFrame())
        {
            if (!isGrabbing && currentEnemy != null)
            {
                // --- GRAB ---
                grabbedEnemy = currentEnemy;
                grabbedEnemy.ChangeEnemyStatus(grabZone);

                isGrabbing = true;
                animator.SetTrigger("Pickup");

                // <--- NUEVO: Sonido Agarrar
                PlaySound(grabSound);
            }
            else if (isGrabbing && grabbedEnemy != null)
            {
                // --- DROP ---
                StartCoroutine(HandleDrop());
            }
        }
    }

    private System.Collections.IEnumerator HandleDrop()
    {
        animator.SetTrigger("Drop");

        // <--- NUEVO: Sonido Soltar
        PlaySound(dropSound);

        grabbedEnemy.transform.SetParent(null);
        grabbedEnemy.transform.position = dropPoint.position;

        grabbedEnemy.HideTemporarily(dropRespawnDelay);

        isGrabbing = false;
        grabbedEnemy = null;

        yield return null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
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