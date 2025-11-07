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
    [SerializeField] private Animator animator; // Referencia al Animator del jugador

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound; // Sonido del disparo
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.8f;

    private float nextFireTime = 0f;
    private InputAction shootAction;
    private SpriteRenderer sprite;
    private AudioSource audioSource;
    private bool canParalyze = false; // Controla si el jugador tiene habilitado el disparo

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Configuración del AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
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

        // 🔊 Reproduce el sonido de disparo
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
        else
        {
            Debug.LogWarning("No se asignó un AudioClip para el sonido de disparo.");
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
