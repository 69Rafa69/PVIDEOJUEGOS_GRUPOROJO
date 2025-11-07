using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;
    [SerializeField] private float maxDistance;

    private Rigidbody2D body;
    private Vector2 initialPosition;
    private bool moveRight = true; // Controla la dirección de disparo

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = new Vector2(speedX, speedY);
        Debug.Log("La bala se ha inicializado");
    }

    public void StartBullet()
    {
        initialPosition = transform.localPosition;
    }

    // Cambia la dirección de disparo
    public void SetDirection(bool right)
    {
        moveRight = right;
        float dir = moveRight ? 1f : -1f;

        if (body != null)
        {
            // Ajusta la velocidad según la dirección
            body.linearVelocity = new Vector2(Mathf.Abs(speedX) * dir, speedY);
        }

        // Voltea el sprite si dispara a la izquierda
        transform.localScale = new Vector3(moveRight ? 1 : -1, 1, 1);
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.localPosition, initialPosition);
        if (distance >= maxDistance)
        {
            Destroy(gameObject);
            Debug.Log("Bala destruida");
        }
    }
}
