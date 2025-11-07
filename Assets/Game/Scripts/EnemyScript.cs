using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private float limitRight;
    [SerializeField] private float limitLeft;

    private Vector2 limits;
    private int direction;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector3 originalPosition;

    private void Awake()
    {
        // Guarda la posición inicial y define los límites de movimiento
        Vector3 pos = transform.localPosition;
        originalPosition = pos;
        limits = new Vector2(pos.x - limitLeft, pos.x + limitRight);

        // Obtiene las referencias necesarias
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // Comienza moviéndose hacia la derecha
        direction = 1;
    }

    private void Update()
    {
        // Actualiza la orientación del sprite según la dirección
        if (direction != 0)
            sprite.flipX = direction < 0;

        // Comprueba los límites y cambia la dirección si es necesario
        Vector3 pos = transform.localPosition;
        if (pos.x <= limits.x)
            direction = 1;
        if (pos.x >= limits.y)
            direction = -1;

        // Aplica la velocidad en el eje X
        body.linearVelocityX = direction * speedX;
    }

    private void OnDrawGizmos()
    {
        // Dibuja los límites de patrullaje en el editor
        Vector3 pos = originalPosition != Vector3.zero ? originalPosition : transform.localPosition;
        Vector3 posLeft = new Vector3(pos.x - limitLeft, pos.y, pos.z);
        Vector3 posRight = new Vector3(pos.x + limitRight, pos.y, pos.z);
        Gizmos.DrawSphere(posLeft, 0.5f);
        Gizmos.DrawSphere(posRight, 0.5f);
    }
}
