using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private float sparkSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;

    // Cambiar a usar InputSystem.actions.FindAction
    private InputAction shootAction;

    private SpriteRenderer sprite;

    private void Awake()
    {
       
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Encontrar el input (Edit / Project Settings)
        shootAction = InputSystem.actions.FindAction("Shoot");
    }

    private void Update()
    {
        // Detectar la tecla
        if (shootAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Calcular la dirección del disparo
        Vector2 shootDirection = GetShootDirection();

        // Crear el spark en la posición del jugador
        GameObject spark = Instantiate(sparkPrefab, transform.position, Quaternion.identity);

        // Obtener el Rigidbody2D del spark
        Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();

        // Aplicar velocidad al spark
        if (sparkRb != null)
        {
            sparkRb.linearVelocity = shootDirection * sparkSpeed;
        }

        // Actualizar el tiempo para la próxima vez que se pueda disparar
        nextFireTime = Time.time + fireRate;
    }

    private Vector2 GetShootDirection()
    {
        // Determinar la dirección de disparo de acuerdo a donde se está mirando
        return sprite.flipX ? Vector2.left : Vector2.right;
    }
}
