using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona la lógica de teletransportación bidireccional.
/// Controla el estado visual, el audio y previene bucles de teletransporte inmediatos.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PortalScript : MonoBehaviour
{
    // HashSet para rastrear objetos que acaban de llegar a este portal.
    // Esto evita que el portal te envíe de vuelta inmediatamente al tocar el collider de salida.
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [Header("Conexiones")]
    [Tooltip("El destino donde aparecerá el jugador. Si es otro portal, se vinculará la lógica de cooldown.")]
    [SerializeField] private Transform destination;

    [Header("Estado Inicial")]
    [SerializeField] private bool isActive = false;

    [Header("Configuración Visual")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip openSound;     // Sonido de activación (Switch)
    [SerializeField] private AudioClip teleportSound; // Sonido de viaje (Whoosh)

    [Tooltip("Retraso para el sonido de apertura. Útil para no solapar con el sonido del interruptor.")]
    [SerializeField, Range(0f, 1f)] private float soundDelay = 0.2f;

    // Componentes cacheados
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Coroutine openSoundCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Inicializa el estado visual sin reproducir sonido
        UpdatePortalSprite();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Guard Clauses: Validaciones rápidas para salir del método si no cumple requisitos
        if (!isActive) return;
        if (!collision.CompareTag("Player")) return;

        // Si el objeto está en la lista de bloqueo (acaba de llegar aquí desde otro portal), ignoramos.
        if (portalObjects.Contains(collision.gameObject)) return;

        // Lógica de Teletransporte
        if (destination != null)
        {
            // 1. Mover al jugador
            // NOTA: Si el jugador tiene Rigidbody2D y se mueve muy rápido, 
            // a veces es necesario resetear su velocidad aquí.
            collision.transform.position = destination.position;

            // 2. Bloquear retorno inmediato en el destino
            // Intentamos obtener el script del portal de destino para avisarle que el jugador acaba de llegar.
            if (destination.TryGetComponent(out PortalScript destinationPortal))
            {
                destinationPortal.RegisterArrivingObject(collision.gameObject);
            }

            // 3. Feedback Audio
            PlaySound(teleportSound);
        }
        else
        {
            Debug.LogWarning($"Portal {name}: No tiene destino asignado.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Al salir del collider, eliminamos al objeto de la lista de bloqueo.
        // Ahora es libre de volver a usar este portal si entra de nuevo.
        if (portalObjects.Contains(collision.gameObject))
        {
            portalObjects.Remove(collision.gameObject);
        }
    }

    /// <summary>
    /// Cambia el estado del portal (Activado/Desactivado).
    /// </summary>
    public void SetActive(bool active)
    {
        if (isActive == active) return; // Evita cálculos si el estado es el mismo

        isActive = active;
        UpdatePortalSprite();

        // Gestión de la Corrutina de Audio
        if (openSoundCoroutine != null) StopCoroutine(openSoundCoroutine);

        if (isActive)
        {
            openSoundCoroutine = StartCoroutine(PlayOpenSoundDelayed());
        }
    }

    /// <summary>
    /// Método público para que OTROS portales registren que un objeto ha llegado aquí.
    /// </summary>
    public void RegisterArrivingObject(GameObject obj)
    {
        if (!portalObjects.Contains(obj))
        {
            portalObjects.Add(obj);
        }
    }

    private IEnumerator PlayOpenSoundDelayed()
    {
        // Esperamos el tiempo definido para evitar saturación auditiva con el switch
        yield return new WaitForSeconds(soundDelay);

        if (isActive)
        {
            PlaySound(openSound);
        }
    }

    private void UpdatePortalSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Getter público para consultar estado desde otros scripts
    public bool IsActive => isActive;
}