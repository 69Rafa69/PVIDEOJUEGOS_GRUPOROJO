using UnityEngine;

public class ParalyzePowerup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float blinkSpeed = 4f;          // Velocidad del parpadeo
    [SerializeField] private Color blinkColor = Color.yellow; // Color del parpadeo

    private SpriteRenderer sprite;
    private Color originalColor;

    private void Awake()
    {
        // Obtiene el SpriteRenderer y guarda el color original
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            originalColor = sprite.color;
    }

    private void Update()
    {
        // Genera parpadeo mediante una interpolación senoidal
        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
        if (sprite != null)
            sprite.color = Color.Lerp(originalColor, blinkColor, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo reacciona al jugador
        if (!collision.CompareTag("Player")) return;

        // Busca el componente PlayerShoot (directo o en el padre)
        PlayerShoot playerShoot = collision.GetComponent<PlayerShoot>() ?? collision.GetComponentInParent<PlayerShoot>();

        if (playerShoot != null)
            playerShoot.UnlockParalyze();

        // Reproduce sonido de recogida
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Elimina el objeto tras recogerlo
        Destroy(gameObject);
    }
}
