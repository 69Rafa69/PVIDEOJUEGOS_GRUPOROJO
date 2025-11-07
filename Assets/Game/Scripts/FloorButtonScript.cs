using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    [SerializeField] private PortalScript[] portals;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Sprite spriteIdle;     // Sprite cuando está sin presionar
    [SerializeField] private Sprite spritePressed;  // Sprite cuando está presionado

    private int pressCount = 0; // Cuántos objetos están presionando el botón

    private void Start()
    {
        // Desactiva los portales al inicio
        foreach (var portal in portals)
            portal.SetActive(false);

        // Configura el sprite inicial
        if (buttonSprite != null && spriteIdle != null)
            buttonSprite.sprite = spriteIdle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo cuenta Player o EnemyGrab
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount++;
            UpdateButtonState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reduce contador cuando sale un objeto válido
        if (collision.CompareTag("Player") || collision.CompareTag("EnemyGrab"))
        {
            pressCount = Mathf.Max(pressCount - 1, 0);

            // Solo desactiva si nadie más está encima
            if (pressCount == 0)
                UpdateButtonState(false);
        }
    }

    private void UpdateButtonState(bool pressed)
    {
        // Activa o desactiva todos los portales
        foreach (var portal in portals)
            portal.SetActive(pressed);

        // Cambia el sprite según el estado
        if (buttonSprite != null)
            buttonSprite.sprite = pressed ? spritePressed : spriteIdle;
    }
}
