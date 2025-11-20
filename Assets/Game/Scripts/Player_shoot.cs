using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private float sparkSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Animación")]
    [SerializeField] private Animator animator; // Referencia al Animator del jugador

    private float nextFireTime = 0f;
    private InputAction shootAction;
    private SpriteRenderer sprite;
    private bool canParalyze = false; // Controla si el jugador tiene habilitado el disparo

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            // Si no se asignó manualmente en el inspector, intenta obtenerlo automáticamente
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // Busca la acción "Shoot" definida en el sistema de input
        shootAction = InputSystem.actions.FindAction("Shoot");
    }

    private void Update()
    {
        // Si no tiene el poder de parálisis, no puede disparar
        if (!canParalyze) return;

        // Comprueba si se presionó el botón de disparo y respeta la cadencia
        if (shootAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Determina la dirección del disparo según la orientación del sprite
        Vector2 shootDirection = GetShootDirection();

        // Posición de salida del proyectil, ligeramente por encima del jugador
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);

        // Instancia del proyectil
        GameObject spark = Instantiate(sparkPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();

        // Asigna velocidad al proyectil si tiene Rigidbody2D
        if (sparkRb != null)
            sparkRb.linearVelocity = shootDirection * sparkSpeed;

        // Informa la dirección al script del proyectil si implementa SetDirection()
        var sparkScript = spark.GetComponent<Spark>();
        if (sparkScript != null)
            sparkScript.SetDirection(shootDirection.x > 0);

        // Actualiza el tiempo del próximo disparo permitido
        nextFireTime = Time.time + fireRate;

        // Activa el trigger de animación "Shoot"
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            Debug.LogWarning("No se asignó un Animator para la animación de disparo.");
        }
    }

    private Vector2 GetShootDirection()
    {
        // Devuelve la dirección en función del flip del sprite
        return sprite.flipX ? Vector2.left : Vector2.right;
    }

    // Método público para habilitar la capacidad de disparar (por ejemplo, tras obtener un power-up)
    public void UnlockParalyze()
    {
        canParalyze = true;
        Debug.Log("Parálisis desbloqueada.");
    }

    // Propiedad de solo lectura para verificar si el poder está activo
    public bool HasParalyzePower => canParalyze;
}
