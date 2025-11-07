using UnityEngine;

public class Door_Controller : MonoBehaviour
{
    private bool isOpen = false;
    private SpriteRenderer sprite;

    private void Start()
    {
        // Obtiene la referencia al SpriteRenderer
        sprite = GetComponent<SpriteRenderer>();

        // Muestra una advertencia si no hay SpriteRenderer asignado
        if (sprite == null)
            Debug.LogWarning("No se encontró SpriteRenderer en la puerta: " + gameObject.name);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            Debug.Log("Puerta abierta");

            // Oculta el sprite
            if (sprite != null)
                sprite.enabled = false;

            // Desactiva el collider para permitir el paso
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }
}
