using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [Header("Configuración de escena")]
    [SerializeField] private string nextSceneName; // Nombre exacto de la escena a cargar
    [SerializeField] private float delayBeforeLoad = 0.5f; // Pequeño retraso opcional

    private bool isTriggered = false; // Evita múltiples activaciones

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo reacciona si el jugador toca la meta
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Meta alcanzada, cambiando de escena...");
            StartCoroutine(LoadNextScene());
        }
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        // Espera opcional (para mostrar animación o sonido)
        yield return new WaitForSeconds(delayBeforeLoad);

        // Verifica que el nombre esté configurado
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No se ha asignado el nombre de la siguiente escena en GoalTrigger.");
        }
    }
}
