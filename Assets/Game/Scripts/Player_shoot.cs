using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerShoot : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto vacío 'FirePoint' que creaste como hijo del jugador.")]
    [SerializeField] private Transform firePoint; // <--- LA CLAVE

    [Header("Disparo")]
    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private float sparkSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;

    private float nextFireTime = 0f;
    private InputAction shootAction;
    private bool canParalyze = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Validación de seguridad
        if (firePoint == null)
        {
            Debug.LogError("PlayerShoot: ¡Falta asignar el FirePoint en el inspector! Creando uno temporal...");
            GameObject tempPoint = new GameObject("TempFirePoint");
            tempPoint.transform.SetParent(transform);
            tempPoint.transform.localPosition = new Vector3(0.5f, 0.5f, 0);
            firePoint = tempPoint.transform;
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
        // 1. Calcular dirección BASADO EN LA ESCALA (Igual que el movimiento)
        // Si la escala es positiva (1), miramos derecha. Si es negativa (-1), izquierda.
        float directionSign = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 shootDirection = new Vector2(directionSign, 0);

        // 2. Posición de Spawn: Usamos el FirePoint
        // Al ser hijo del jugador, si el jugador se voltea, el FirePoint también.
        Vector3 spawnPos = firePoint.position;

        // Audio
        if (audioSource != null && shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound);
            audioSource.pitch = 1f;
        }

        // Instanciar bala
        GameObject spark = Instantiate(sparkPrefab, spawnPos, Quaternion.identity);

        // Físicas de la bala
        Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();
        if (sparkRb != null)
        {
            // Unity 6: linearVelocity | Unity 2022: velocity
            sparkRb.linearVelocity = shootDirection * sparkSpeed;
        }

        // Configurar dirección visual de la bala
        var sparkScript = spark.GetComponent<Spark>();
        if (sparkScript != null)
        {
            // true si mira a la derecha, false si mira a la izquierda
            sparkScript.SetDirection(directionSign > 0);
        }

        nextFireTime = Time.time + fireRate;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
    }

    public void UnlockParalyze()
    {
        canParalyze = true;
        Debug.Log("Parálisis desbloqueada.");
    }

    public bool HasParalyzePower => canParalyze;
}