using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private void Awake()
    {
        // Esto es un patrón "Singleton". 
        // Asegura que solo haya UNA música sonando a la vez.

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <--- Esto hace que la música sobreviva al cambio de escena
        }
        else
        {
            // Si ya existe otra música (porque volviste a cargar el menú), destrúyete
            Destroy(gameObject);
        }
    }
}