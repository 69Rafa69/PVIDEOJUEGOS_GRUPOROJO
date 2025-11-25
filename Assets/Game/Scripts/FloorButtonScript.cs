using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private PortalScript[] portals;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Sprite spriteIdle;
    [SerializeField] private Sprite spritePressed;

    [Header("Audio")] // <--- NUEVO: Para organizar el inspector
    [SerializeField] private AudioSource audioSource; // <--- NUEVO: El "parlante"
    [SerializeField] private AudioClip pressSound;    // <--- NUEVO: El archivo de sonido

    private int pressCount = 0;

    private void Start()
    {
        // Desactiva los portales al inicio
        foreach (var portal in portals)
            portal.SetActive(false);

        // Configura el sprite inicial
        if (buttonSprite != null && spriteIdle != null)
            buttonSprite.sprite = spriteIdle;

        // Verificación de seguridad opcional
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo cuenta Player o EnemyGrab
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount++;

            // <--- NUEVO: Solo reproducir sonido si es el PRIMER objeto (el botón estaba libre)
            if (pressCount == 1 && audioSource != null && pressSound != null)
            {
                // Usamos PlayOneShot para que sonidos cortos no se corten entre sí
                audioSource.PlayOneShot(pressSound);
            }

            UpdateButtonState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reduce contador cuando sale un objeto válido
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount = Mathf.Max(pressCount - 1, 0);

            // Solo desactiva si nadie más está encima
            if (pressCount == 0)
                UpdateButtonState(false);
        }
    }

    private void UpdateButtonState(bool pressed)
    {
        // Activa o desactiva todos los portales
        foreach (var portal in portals)
            portal.SetActive(pressed);

        // Cambia el sprite según el estado
        if (buttonSprite != null)
            buttonSprite.sprite = pressed ? spritePressed : spriteIdle;
    }
}