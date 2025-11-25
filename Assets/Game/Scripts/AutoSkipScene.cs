using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSkipScene : MonoBehaviour
{
    [SerializeField] private float timeToWait = 5f; // Tiempo para leer
    [SerializeField] private string nextScene = "Sala_1";

    private void Start()
    {
        // Invoca el cambio de escena automáticamente
        Invoke("LoadLevel", timeToWait);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(nextScene);
    }
}