using System.Collections; // Necesario para Corrutinas
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la transición entre escenas y la salida de la aplicación.
/// Incluye lógica para esperar el feedback de audio antes de descargar la escena actual.
/// </summary>
public class Scene_Manager : MonoBehaviour
{
    [Header("Feedback de Audio")]
    [Tooltip("Fuente de audio para efectos de interfaz.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Sonido de confirmación al pulsar el botón.")]
    [SerializeField] private AudioClip clickSound;

    [Header("Configuración")]
    [Tooltip("Si es true, espera a que termine el audio antes de cambiar de escena.")]
    [SerializeField] private bool waitForSound = true;

    /// <summary>
    /// Método público para asignar en botones UI (On Click).
    /// Inicia la secuencia de cambio de escena.
    /// </summary>
    /// <param name="sceneName">Nombre exacto de la escena en Build Settings.</param>
    public void ChangeScene(string sceneName)
    {
        // Validación básica para evitar errores en consola
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene_Manager: Se intentó cargar una escena con nombre vacío.");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    /// <summary>
    /// Corrutina que maneja el tiempo de espera del audio y la carga asíncrona.
    /// </summary>
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1. Reproducir sonido
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);

            // Si está configurado, esperamos la duración del clip
            // Usamos WaitForSecondsRealtime por si el juego está en Pausa (Time.timeScale = 0)
            if (waitForSound)
            {
                yield return new WaitForSecondsRealtime(clickSound.length);
            }
        }

        // 2. Cargar la escena
        // TODO: Para escenas pesadas, considerar usar LoadSceneAsync y mostrar una barra de carga.
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Cierra la aplicación. Compatible con el Editor de Unity y Builds finales.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Cerrando aplicación...");

        // Preprocesador: Permite que el botón funcione mientras pruebas en el Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}