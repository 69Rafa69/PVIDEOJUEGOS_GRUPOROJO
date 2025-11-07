using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    // Genera un efecto de sacudida de cámara
    public IEnumerator Shake(float duration, float magnitude)
    {
        originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calcula un desplazamiento aleatorio en X e Y
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restaura la posición original
        transform.localPosition = originalPos;
    }
}
