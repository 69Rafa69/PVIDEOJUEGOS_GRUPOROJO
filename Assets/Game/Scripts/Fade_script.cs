using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Fade_transition : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Duración del fundido en segundos.")]
    [SerializeField] private float fadeDuration = 1f;

    [Tooltip("Si es true, empieza la escena con la pantalla negra y aclara.")]
    [SerializeField] private bool fadeInOnStart = true;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (fadeInOnStart)
        {
            // Iniciamos totalmente negros (Alpha 1)
            canvasGroup.alpha = 1f;
            StartCoroutine(Fade(0f)); // Vamos hacia transparente (0)
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false; // Permitir clics
        }
    }

    /// <summary>
    /// Llama a este método desde tus botones o triggers.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // 1. Bloqueamos los clics para que el usuario no toque nada más
        canvasGroup.blocksRaycasts = true;

        // 2. Fundido a Negro (Alpha va a 1)
        yield return StartCoroutine(Fade(1f));

        // 3. Cargamos la escena
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Corrutina genérica para cambiar el Alpha suavemente.
    /// </summary>
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            // Mathf.Lerp nos da el valor intermedio entre A y B basado en el tiempo
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos que el valor final sea exacto
        canvasGroup.alpha = targetAlpha;

        // Si terminamos de aclarar (Alpha 0), desactivamos el bloqueo de Raycasts
        if (targetAlpha == 0f)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}