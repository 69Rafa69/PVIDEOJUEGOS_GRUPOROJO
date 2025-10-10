using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour, IParalyzable
{
    [Header("Disparo")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float timeToShoot = 2f;

    [Header("Parálisis")]
    [SerializeField] private float paralysisDuration = 5f;
    [SerializeField] private float paralysisCooldown = 5f; // ⏳ tiempo de inmunidad tras recuperar el control

    private Transform bulletContainerTransform;
    private float timer;
    private SpriteRenderer sprite;

    private bool isImmune = false; // evita spam de parálisis
    public bool isParalyzed { get; private set; } = false;

    private void Awake()
    {
        timer = 0f;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject bulletContainer = GameObject.FindGameObjectWithTag("BulletContainer");
        if (bulletContainer != null)
            bulletContainerTransform = bulletContainer.transform;
    }

    private void Update()
    {
        if (isParalyzed) return; // paralizado → no dispara

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

        GameObject bulletObject = Instantiate(bullet, bulletContainerTransform);
        bulletObject.transform.localPosition = transform.localPosition;

        BulletScript bulletScript = bulletObject.GetComponent<BulletScript>();
        bulletScript.StartBullet();
    }

    // --- Parálisis con cooldown e indicadores visuales ---
    public void Paralyze()
    {
        if (isParalyzed || isImmune) return; // ⛔ no se puede stunear si ya lo está o es inmune

        Debug.Log($"EnemyShooter paralizado durante {paralysisDuration} segundos");
        StartCoroutine(ParalysisRoutine());
    }

    private IEnumerator ParalysisRoutine()
    {
        isParalyzed = true;

        if (sprite != null)
            sprite.color = Color.gray;

        yield return new WaitForSeconds(paralysisDuration);

        // Termina la parálisis
        isParalyzed = false;

        // Comienza inmunidad
        StartCoroutine(ParalysisCooldownRoutine());
    }

    private IEnumerator ParalysisCooldownRoutine()
    {
        isImmune = true;

        // Color amarillo → indica inmunidad
        if (sprite != null)
            sprite.color = Color.yellow;

        Debug.Log($"EnemyShooter inmune a la parálisis durante {paralysisCooldown} segundos");

        yield return new WaitForSeconds(paralysisCooldown);

        isImmune = false;

        // Regresa al color normal
        if (sprite != null)
            sprite.color = Color.white;
    }
}
