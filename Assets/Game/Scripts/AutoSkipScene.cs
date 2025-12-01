using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // <--- NECESARIO para el nuevo sistema

public class AutoSkipScene : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float timeToWait = 5f;
    [SerializeField] private string nextScene = "Sala_1";

    private bool hasTriggered = false;

    private void Start()
    {
        Invoke("StartTransition", timeToWait);
    }

    private void Update()
    {
        if (hasTriggered) return;

        // VERIFICACIÓN CON EL NUEVO INPUT SYSTEM
        // 1. Chequeamos si el teclado existe y si se pulsó algo
        bool tecladoPulsado = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        // 2. Chequeamos si el mouse existe y se hizo clic izquierdo
        bool mouseClic = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        // Si cualquiera de los dos ocurrió...
        if (tecladoPulsado || mouseClic)
        {
            CancelInvoke("StartTransition");
            StartTransition();
        }
    }

    private void StartTransition()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Fade_transition transitioner = FindObjectOfType<Fade_transition>();

        if (transitioner != null)
        {
            transitioner.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("No se encontró SceneTransition. Cambiando escena sin efecto.");
            SceneManager.LoadScene(nextScene);
        }
    }
}