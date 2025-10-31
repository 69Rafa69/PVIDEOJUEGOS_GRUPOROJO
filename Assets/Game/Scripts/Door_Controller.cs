using UnityEngine;

public class Door_Controller : MonoBehaviour
{
    private bool isOpen = false;
    private SpriteRenderer sprite;

    private void Start()
    {
        // Inicializa la referencia al SpriteRenderer
        sprite = GetComponent<SpriteRenderer>();

        // (Opcional) Lanza advertencia si no hay sprite en el objeto
        if (sprite == null)
            Debug.LogWarning("⚠️ No se encontró SpriteRenderer en la puerta: " + gameObject.name);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            Debug.Log("Puerta abierta!");

            // Desactiva el sprite (lo oculta visualmente)
            if (sprite != null)
                sprite.enabled = false;

            // Desactiva el collider (permite pasar)
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }
}
