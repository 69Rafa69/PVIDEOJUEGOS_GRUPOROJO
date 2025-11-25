using UnityEngine;

[RequireComponent(typeof(AudioSource))] // <--- NUEVO: Asegura que tenga AudioSource
public class Switch_Door : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject door;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites del switch")]
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private Sprite spriteOn;

    [Header("Audio")] // <--- NUEVO
    [SerializeField] private AudioClip switchSound; // El sonido "Clack" al activarse

    [Header("Estado inicial")]
    [SerializeField] private bool isActive = false;

    private AudioSource audioSource; // <--- NUEVO

    private void Start()
    {
        // Si no se asignó SpriteRenderer, intenta tomarlo del mismo objeto
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // <--- NUEVO: Obtenemos el componente de audio
        audioSource = GetComponent<AudioSource>();

        // Establece el sprite inicial
        UpdateSprite();
    }

    public void Activate()
    {
        // Solo se activa si no estaba activo antes
        if (!isActive)
        {
            isActive = true;
            Debug.Log("Switch activado por bala!");

            UpdateSprite(); // Cambia el sprite a 'activado'

            // <--- NUEVO: Reproducir sonido
            if (audioSource != null && switchSound != null)
            {
                audioSource.PlayOneShot(switchSound);
            }

            // Si hay una puerta asignada, intenta abrirla
            if (door != null)
            {
                Door_Controller doorScript = door.GetComponent<Door_Controller>();
                if (doorScript != null)
                    doorScript.OpenDoor();
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