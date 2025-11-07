using UnityEngine;

public class Switch_Door : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject door;           // La puerta a abrir
    [SerializeField] private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer del switch

    [Header("Sprites del switch")]
    [SerializeField] private Sprite spriteOff;          // Sprite cuando está desactivado
    [SerializeField] private Sprite spriteOn;           // Sprite cuando está activado

    [Header("Estado inicial")]
    [SerializeField] private bool isActive = false;     // Estado del switch

    private void Start()
    {
        // Si no se asignó SpriteRenderer, intenta tomarlo del mismo objeto
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Establece el sprite inicial
        UpdateSprite();
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log("Switch activado por bala!");

            UpdateSprite(); // Cambia el sprite a 'activado'

            // Si hay una puerta asignada, intenta abrirla
            if (door != null)
            {
                Door_Controller doorScript = door.GetComponent<Door_Controller>();
                if (doorScript != null)
                    doorScript.OpenDoor();
            }
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? spriteOn : spriteOff;
        }
    }
}
