using UnityEngine;

public class Spark : MonoBehaviour
{
    [SerializeField] private float duration = 10f;

    // Si colisiona con un enemigo
    void OnTriggerEnter2D(Collider2D other) // Cambiado a OnTriggerEnter2D
    {
        IParalyzable paralyzable = other.GetComponent<IParalyzable>(); // Llamar a la interfaz
        if (paralyzable != null) // Solo si es un enemigo paralizable
        {
            paralyzable.Paralyze();  // Paralizarlo
            Destroy(gameObject);    // Destruir la bala
        }
    }

    // Si no colisiona
    void Start()
    {
        Destroy(gameObject, duration); // Destruye después de la duración especificada
    }
}
