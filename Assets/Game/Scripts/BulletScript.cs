using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;
    [SerializeField] private float maxDistance;

    private Rigidbody2D body;
    private Vector2 initialPosition;
    private bool moveRight = true; // ? agregado para controlar direcci�n

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = new Vector2(speedX, speedY);
        Debug.Log("La bala se despierta...");
    }

    public void StartBullet()
    {
        initialPosition = transform.localPosition;
    }

    // ? NUEVO M�TODO: permite cambiar la direcci�n de disparo (derecha o izquierda)
    public void SetDirection(bool right)
    {
        moveRight = right;

        float dir = moveRight ? 1f : -1f;

        if (body != null)
        {
            // Invierte la direcci�n en X seg�n el flip del enemigo
            body.linearVelocity = new Vector2(Mathf.Abs(speedX) * dir, speedY);
        }

        // Invierte el sprite visualmente si va a la izquierda
        transform.localScale = new Vector3(moveRight ? 1 : -1, 1, 1);
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.localPosition, initialPosition);
        if (distance >= maxDistance)
        {
            Destroy(gameObject);
            Debug.Log("Destruida la bala...");
        }
    }
}
