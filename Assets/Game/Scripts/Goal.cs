using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Goal : MonoBehaviour
{
    [Header("Configuración de escena")]
    [Tooltip("Nombre exacto de la escena a cargar.")]
    [SerializeField] private string nextSceneName;

    [Tooltip("Tiempo de espera ANTES de que la pantalla empiece a oscurecerse.")]
    [SerializeField] private float delayBeforeFade = 2.0f;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip winSound;

    private AudioSource audioSource;
    private bool isTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitamos que se active dos veces
        if (isTriggered) return;

        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("¡Meta alcanzada!");

            // 1. Reproducir sonido de victoria
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            // 2. Iniciar la secuencia de salida
            StartCoroutine(SequenceEndLevel());
        }
    }

    private IEnumerator SequenceEndLevel()
    {
        // PASO 1: Esperar un poco para celebrar (oír el audio)
        // El jugador sigue viendo el juego mientras suena la música
        yield return new WaitForSeconds(delayBeforeFade);

        // PASO 2: Buscar el sistema de transición
        Fade_transition fader = FindObjectOfType<Fade_transition>();

        if (fader != null)
        {
            // Si existe, le pedimos que haga el fundido y cambie la escena
            Debug.Log("Iniciando transición suave...");
            fader.LoadScene(nextSceneName);
        }
        else
        {
            // FALLBACK: Si olvidaste poner el Canvas de transición, carga de golpe
            // para que el juego no se quede atascado.
            Debug.LogWarning("No se encontró Fade_transition. Cargando directo.");
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}