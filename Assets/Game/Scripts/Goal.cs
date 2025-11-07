using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Goal : MonoBehaviour
{
    [Header("Configuración de escena")]
    [SerializeField] private string nextSceneName;      // Nombre de la escena a cargar
    [SerializeField] private float delayBeforeLoad = 0.5f; // Retraso antes de cambiar de escena

    [Header("Audio")]
    [SerializeField] private AudioClip portalSound;     // Sonido al entrar al portal
    [SerializeField] private bool waitForSound = true;  // Si debe esperar al sonido antes de cambiar

    private AudioSource audioSource;
    private bool isTriggered = false;

    private void Awake()
    {
        // Obtiene o asegura el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo se activa al tocar el jugador
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Meta alcanzada, cambiando de escena...");

            // Reproduce el sonido si está configurado
            if (portalSound != null)
                audioSource.PlayOneShot(portalSound);

            // Determina el tiempo de espera antes de cambiar de escena
            float delay = waitForSound && portalSound != null ? portalSound.length : delayBeforeLoad;
            StartCoroutine(LoadNextScene(delay));
        }
    }

    private IEnumerator LoadNextScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Carga la siguiente escena si el nombre está configurado
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("No se ha asignado el nombre de la siguiente escena en Goal.");
    }
}
