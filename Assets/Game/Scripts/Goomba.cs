using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Goomba : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speedX = 3f;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distanciaSuelo = 1.0f;
    [SerializeField] private bool moviendoDerecha = true;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Movimiento simple de patrulla
        float dir = moviendoDerecha ? 1 : -1;

        // Mantenemos la velocidad Y actual para la gravedad
        // (Unity 6 usa linearVelocity, Unity 2022 usa velocity)
        body.linearVelocity = new Vector2(dir * speedX, body.linearVelocity.y);

        // Raycast para detectar bordes
        RaycastHit2D infoSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);

        // Si no hay suelo adelante, giramos
        if (infoSuelo.collider == null)
        {
            Girar();
        }
    }

    private void Girar()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.eulerAngles = new Vector3(0, moviendoDerecha ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        if (controladorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(controladorSuelo.position, controladorSuelo.position + Vector3.down * distanciaSuelo);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Seguridad extra: Si este script está desactivado (por EnemyGrabable), 
        // no debería hacer daño.
        if (!this.enabled) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Asumiendo que tu jugador tiene un script para recibir daño o respawnear
            PlayerDetectionHit playerHit = collision.gameObject.GetComponent<PlayerDetectionHit>();
            if (playerHit != null)
                playerHit.SendMessage("SpawnPlayer", SendMessageOptions.DontRequireReceiver);
        }
    }
}