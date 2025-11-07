using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour, IParalyzable
{
    [Header("Disparo")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float timeToShoot = 2f;

    [Header("Parálisis")]
    [SerializeField] private float paralysisDuration = 5f;
    [SerializeField] private float paralysisCooldown = 5f; // Tiempo de inmunidad tras recuperarse

    private Transform bulletContainerTransform;
    private float timer;
    private SpriteRenderer sprite;

    private bool isImmune = false; // Evita múltiples parálisis seguidas
    public bool isParalyzed { get; private set; } = false;

    private void Awake()
    {
        timer = 0f;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Obtiene la referencia al contenedor de balas
        GameObject bulletContainer = GameObject.FindGameObjectWithTag("BulletContainer");
        if (bulletContainer != null)
            bulletContainerTransform = bulletContainer.transform;
    }

    private void Update()
    {
        // No dispara si está paralizado
        if (isParalyzed) return;

        timer += Time.deltaTime;
        if (timer >= timeToShoot)
        {
            Shoot();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (bullet == null || bulletContainerTransform == null) return;

        // Crea una nueva bala dentro del contenedor
        GameObject bulletObject = Instantiate(bullet, bulletContainerTransform);
        bulletObject.transform.localPosition = transform.localPosition;

        // Configura la bala
        BulletScript bulletScript = bulletObject.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.StartBullet();

            // Define la dirección según la orientación del enemigo
            bool mirandoDerecha = !sprite.flipX;
            bulletScript.SetDirection(mirandoDerecha);
        }
    }

    public void Paralyze()
    {
        // Evita aplicar parálisis si ya está paralizado o es inmune
        if (isParalyzed || isImmune) return;

        Debug.Log($"EnemyShooter paralizado durante {paralysisDuration} segundos");
        StartCoroutine(ParalysisRoutine());
    }

    private IEnumerator ParalysisRoutine()
    {
        isParalyzed = true;

        // Cambia color para indicar parálisis
        if (sprite != null)
            sprite.color = Color.gray;

        yield return new WaitForSeconds(paralysisDuration);

        // Recupera movimiento
        isParalyzed = false;

        // Inicia el periodo de inmunidad
        StartCoroutine(ParalysisCooldownRoutine());
    }

    private IEnumerator ParalysisCooldownRoutine()
    {
        isImmune = true;

        // Color amarillo = inmune
        if (sprite != null)
            sprite.color = Color.yellow;

        Debug.Log($"EnemyShooter inmune durante {paralysisCooldown} segundos");

        yield return new WaitForSeconds(paralysisCooldown);

        isImmune = false;

        // Vuelve al color normal
        if (sprite != null)
            sprite.color = Color.white;
    }
}
