using System.Collections; // <--- NECESARIO para que funcione el tiempo de espera
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PortalScript : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [Header("Configuración del portal")]
    [SerializeField] private Transform destination;
    [SerializeField] private bool isActive = false;

    [Header("Sprites visuales")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("Efectos de Audio")]
    [SerializeField] private AudioClip openSound;     // Suena cuando el botón lo activa
    [SerializeField] private AudioClip teleportSound; // Suena cuando el jugador viaja
    [SerializeField, Range(0f, 1f)] private float soundDelay = 0.2f; // <--- NUEVO: Tiempo de espera (0.2s recomendado)

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        UpdatePortalSprite();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;
        if (!collision.CompareTag("Player")) return;
        if (portalObjects.Contains(collision.gameObject)) return;

        if (destination != null && destination.TryGetComponent(out PortalScript destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        // Mueve al jugador
        collision.transform.position = destination.position;

        // Sonido al teletransportarse ("Whoosh") - Este suena INMEDIATAMENTE
        PlaySound(teleportSound);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }

    public void SetActive(bool active)
    {
        // Solo actualiza si el estado cambia de verdad
        if (isActive == active) return;

        isActive = active;
        UpdatePortalSprite();

        // Lógica del sonido de activación con RETRASO
        if (isActive)
        {
            // Iniciamos la cuenta atrás para que no se pise con el clic del botón
            StartCoroutine(PlayOpenSoundDelayed());
        }
    }

    // <--- NUEVO: La rutina que espera antes de sonar
    private IEnumerator PlayOpenSoundDelayed()
    {
        // 1. Espera el tiempo configurado
        yield return new WaitForSeconds(soundDelay);

        // 2. Verifica si sigue activo y reproduce
        if (isActive)
        {
            PlaySound(openSound);
        }
    }

    private void UpdatePortalSprite()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
    }

    public bool IsActive()
    {
        return isActive;
    }

    // Método auxiliar para reproducir sonidos con seguridad
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}