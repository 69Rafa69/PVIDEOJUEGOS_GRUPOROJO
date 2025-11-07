using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    [SerializeField] private PortalScript[] portals;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Sprite spriteIdle;     // Sprite cuando no está presionado
    [SerializeField] private Sprite spritePressed;  // Sprite cuando está presionado

    private int pressCount = 0; // Cantidad de objetos sobre el botón

    private void Start()
    {
        // Desactiva los portales al inicio
        foreach (var portal in portals)
            portal.SetActive(false);

        // Asigna el sprite inicial
        if (buttonSprite != null && spriteIdle != null)
            buttonSprite.sprite = spriteIdle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo reacciona ante el jugador o un enemigo agarrado
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount++;
            UpdateButtonState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Disminuye el contador cuando un objeto válido se retira
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount = Mathf.Max(pressCount - 1, 0);

            // Si no hay nada encima, cambia a estado inactivo
            if (pressCount == 0)
                UpdateButtonState(false);
        }
    }

    private void UpdateButtonState(bool pressed)
    {
        // Activa o desactiva todos los portales
        foreach (var portal in portals)
            portal.SetActive(pressed);

        // Cambia el sprite según el estado actual
        if (buttonSprite != null)
            buttonSprite.sprite = pressed ? spritePressed : spriteIdle;
    }
}
