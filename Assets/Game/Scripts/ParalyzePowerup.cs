using UnityEngine;

public class ParalyzePowerup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float blinkSpeed = 4f; // velocidad del parpadeo
    [SerializeField] private Color blinkColor = Color.yellow; // color del parpadeo

    private SpriteRenderer sprite;
    private Color originalColor;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            originalColor = sprite.color;
    }

    private void Update()
    {
        // 🌟 Parpadeo usando una interpolación seno
        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
        if (sprite != null)
            sprite.color = Color.Lerp(originalColor, blinkColor, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerShoot playerShoot = collision.GetComponent<PlayerShoot>() ?? collision.GetComponentInParent<PlayerShoot>();

        if (playerShoot != null)
        {
            playerShoot.UnlockParalyze();
        }

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}
