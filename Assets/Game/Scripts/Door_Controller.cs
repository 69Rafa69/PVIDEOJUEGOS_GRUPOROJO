using System.Collections; // <--- NECESARIO para usar Corutinas (IEnumerator)
using UnityEngine;

[RequireComponent(typeof(AudioSource))] // <--- Asegura que tenga AudioSource
public class Door_Controller : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float openDelay = 0.2f; // Tiempo de espera tras el interruptor

    [Header("Audio")]
    [SerializeField] private AudioClip openSound; // Sonido de puerta abriéndose (piedra/metal)

    private bool isOpen = false;
    private SpriteRenderer sprite;
    private Collider2D col; // <--- Guardamos referencia al collider
    private AudioSource audioSource;

    private void Start()
    {
        // Inicializa las referencias
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>(); // <--- Capturamos el collider al inicio
        audioSource = GetComponent<AudioSource>();

        if (sprite == null)
            Debug.LogWarning("⚠️ No se encontró SpriteRenderer en la puerta: " + gameObject.name);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            // En lugar de abrirla ya, iniciamos la secuencia con retraso
            StartCoroutine(OpenDoorSequence());
        }
    }

    private IEnumerator OpenDoorSequence()
    {
        // 1. Esperamos el delay (para no pisar el sonido del interruptor)
        yield return new WaitForSeconds(openDelay);

        Debug.Log("Puerta abierta!");

        // 2. Reproducir sonido
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        // 3. Desactivar visuales y física (Sincronizado con el sonido)
        if (sprite != null) sprite.enabled = false;
        if (col != null) col.enabled = false;
    }
}