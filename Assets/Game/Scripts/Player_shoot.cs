using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private float sparkSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private InputAction shootAction;
    private SpriteRenderer sprite;

    // 🔒 control del poder de parálisis
    private bool canParalyze = false;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
    }

    private void Update()
    {
        if (!canParalyze) return; // ❌ no puede disparar si no tiene el poder

        if (shootAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 shootDirection = GetShootDirection();

        // 💡 Ajusta aquí la altura de salida (por ejemplo 0.5 unidades arriba del sapo)
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);

        GameObject spark = Instantiate(sparkPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();

        if (sparkRb != null)
        {
            sparkRb.linearVelocity = shootDirection * sparkSpeed;
        }

        // ✅ NUEVO BLOQUE: enviar dirección al Spark si tiene SetDirection()
        var sparkScript = spark.GetComponent<Spark>();
        if (sparkScript != null)
        {
            // le decimos si el disparo va hacia la derecha o izquierda
            sparkScript.SetDirection(shootDirection.x > 0);
        }

        nextFireTime = Time.time + fireRate;
    }

    private Vector2 GetShootDirection()
    {
        return sprite.flipX ? Vector2.left : Vector2.right;
    }

    // 🔓 Llamado desde el power-up
    public void UnlockParalyze()
    {
        canParalyze = true;
        Debug.Log("⚡ Parálisis desbloqueada!");
    }

    // (Opcional) puedes añadir esto para depurar desde el editor
    public bool HasParalyzePower => canParalyze;
}
