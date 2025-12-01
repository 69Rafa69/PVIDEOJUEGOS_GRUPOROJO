using UnityEngine;

/// <summary>
/// Controla la lógica de un interruptor interactivo. 
/// Gestiona el estado visual, feedback de audio y la comunicación con una puerta objetivo.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Switch_Door : MonoBehaviour
{
    [Header("Referencias")]
    // TODO: Optimización - Cambiar 'GameObject' por 'Door_Controller' directamente para evitar GetComponent en runtime.
    [Tooltip("Referencia al objeto puerta. Debe contener el script Door_Controller.")]
    [SerializeField] private GameObject door;

    [Tooltip("Renderizador del switch. Se obtendrá automáticamente si se deja vacío.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Configuración Visual")]
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite spriteOn;

    [Header("Configuración de Audio")]
    [Tooltip("Clip de sonido al activar el switch.")]
    [SerializeField] private AudioClip switchSound;

    [Header("Estado")]
    [Tooltip("Indica si el switch ya ha sido accionado. Evita múltiples activaciones.")]
    [SerializeField] private bool isActive = false;

    private AudioSource audioSource;

    private void Start()
    {
        // Validación de dependencias visuales
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) Debug.LogWarning($"{name}: Falta SpriteRenderer.");
        }

        audioSource = GetComponent<AudioSource>();

        // Inicialización de estado visual
        UpdateSprite();
    }

    /// <summary>
    /// Intenta activar el switch. Contiene lógica de "One-Shot" (solo funciona una vez).
    /// </summary>
    public void Activate()
    {
        // Guard clause: Si ya está activo, no hacemos nada.
        if (isActive) return;

        isActive = true;
        Debug.Log($"Switch activado: {name}");

        // Feedback visual y auditivo
        UpdateSprite();
        PlayActivationSound();

        // Lógica de interacción externa
        TriggerDoorAction();
    }

    private void PlayActivationSound()
    {
        if (switchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(switchSound);
        }
    }

    private void TriggerDoorAction()
    {
        // TODO: Mantenimiento - Considerar usar UnityEvents para desacoplar este script de Door_Controller.
        if (door != null)
        {
            if (door.TryGetComponent<Door_Controller>(out var doorScript))
            {
                doorScript.OpenDoor();
            }
            else
            {
                Debug.LogError($"{name}: El objeto asignado en 'door' no tiene el componente Door_Controller.");
            }
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? spriteOn : spriteOff;
        }
    }
}