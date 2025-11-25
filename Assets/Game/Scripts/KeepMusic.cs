using UnityEngine;

public class KeepMusic : MonoBehaviour
{
    // Una variable estática para recordar si ya existe un DJ en el juego
    private static KeepMusic instance;

    private void Awake()
    {
        // 1. ¿Ya existe alguien poniendo música?
        if (instance != null)
        {
            // ¡Sí! Entonces yo soy una copia impostora. Me autodestruyo.
            Destroy(gameObject);
            return;
        }

        // 2. ¿No existe nadie?
        // Entonces yo soy el DJ oficial. Guárdame en la variable 'instance'.
        instance = this;

        // 3. LA ORDEN CLAVE: "No me destruyas al cambiar de escena"
        DontDestroyOnLoad(gameObject);
    }
}