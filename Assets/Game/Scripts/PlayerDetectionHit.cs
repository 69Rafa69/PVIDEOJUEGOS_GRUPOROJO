using UnityEngine;

[RequireComponent(typeof(AudioSource))] // <--- Asegura que haya AudioSource
public class PlayerDetectionHit : MonoBehaviour
{
    [Header("Audio")] // <--- NUEVO
    [SerializeField] private AudioClip deathSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeadZone") ||
            (collision.gameObject.CompareTag("Enemy") && !IsEnemyParalyzed(collision.gameObject)))
        {
            SpawnPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            SpawnPlayer();
            Destroy(collision.gameObject);
        }
    }

    private bool IsEnemyParalyzed(GameObject enemy)
    {
        IParalyzable paralyzableEnemy = enemy.GetComponent<IParalyzable>();
        if (paralyzableEnemy != null)
        {
            return paralyzableEnemy.isParalyzed;
        }
        return false;
    }

    private void SpawnPlayer()
    {
        // <--- NUEVO: Reproducir sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawn != null)
            transform.localPosition = spawn.transform.localPosition;
    }
}