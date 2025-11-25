using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar escenas

public class Scene_Manager : MonoBehaviour
{
    [Header("Sonido (Opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    // Esta función la llamará el botón
    public void ChangeScene(string sceneName)
    {
        // Reproducir sonido si existe (opcional)
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Cargar la escena
        SceneManager.LoadScene(sceneName);
    }

    // Opción extra: Salir del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}