using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerShoot : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private float sparkSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Audio")] // <--- NUEVO
    [SerializeField] private AudioClip shootSound;

    private float nextFireTime = 0f;
    private InputAction shootAction;
    private SpriteRenderer sprite;
    private bool canParalyze = false;
    private AudioSource audioSource; // <--- NUEVO

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); // <--- NUEVO

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
    }

    private void Update()
    {
        if (!canParalyze) return;

        if (shootAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 shootDirection = GetShootDirection();
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);

        // <--- NUEVO: Reproducir sonido de disparo con pequeña variación de tono
        if (audioSource != null && shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Variación para que no suene robótico
            audioSource.PlayOneShot(shootSound);
            audioSource.pitch = 1f; // Resetear pitch
        }

        GameObject spark = Instantiate(sparkPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();

        if (sparkRb != null)
            sparkRb.linearVelocity = shootDirection * sparkSpeed;

        var sparkScript = spark.GetComponent<Spark>();
        if (sparkScript != null)
            sparkScript.SetDirection(shootDirection.x > 0);

        nextFireTime = Time.time + fireRate;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
    }

    private Vector2 GetShootDirection()
    {
        return sprite.flipX ? Vector2.left : Vector2.right;
    }

    public void UnlockParalyze()
    {
        canParalyze = true;
        Debug.Log("Parálisis desbloqueada.");
    }

    public bool HasParalyzePower => canParalyze;
}