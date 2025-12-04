using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // <--- NECESARIO para que funcione la tecla R

public class RestartLevel : MonoBehaviour
{
    // Función pública para llamar desde el Botón (UI)
    public void Restart()
    {
        // 1. Obtenemos el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 2. Buscamos tu sistema de transición
        Fade_transition fader = FindObjectOfType<Fade_transition>();

        if (fader != null)
        {
            // Reinicio elegante
            fader.LoadScene(currentSceneName);
        }
        else
        {
            // Reinicio de emergencia
            SceneManager.LoadScene(currentSceneName);
        }
    }

    // Lógica para detectar la tecla 'R' con el Nuevo Input System
    private void Update()
    {
        // Verificamos si el teclado existe y si se pulsó la tecla R en este frame
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }
}