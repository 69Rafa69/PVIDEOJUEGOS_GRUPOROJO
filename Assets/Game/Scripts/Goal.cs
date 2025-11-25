using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para IEnumerator

[RequireComponent(typeof(AudioSource))] // Asegura que haya un AudioSource
public class Goal : MonoBehaviour
{
    [Header("Configuración de escena")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float delayBeforeLoad = 2.3f; // Aumentado a 2s para oír el audio

    [Header("Configuración de Audio")] // <--- NUEVO
    [SerializeField] private AudioClip winSound;

    private AudioSource audioSource;
    private bool isTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo reacciona si el jugador toca la meta
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;

            // 1. Reproducir sonido de victoria INMEDIATAMENTE
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            Debug.Log("Meta alcanzada, esperando sonido...");

            // 2. Iniciar la espera para cambiar de escena
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        // Espera el tiempo configurado (asegúrate que sea suficiente para oír el audio)
        yield return new WaitForSeconds(delayBeforeLoad);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No se ha asignado el nombre de la siguiente escena en Goal.");
        }
    }
}