using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    [SerializeField] private PortalScript[] portals;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;

    private int pressCount = 0; // Cuántos objetos están presionando el botón

    private void Start()
    {
        foreach (var portal in portals)
            portal.SetActive(false);

        if (buttonSprite != null)
            buttonSprite.color = inactiveColor;
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
        foreach (var portal in portals)
            portal.SetActive(pressed);

        if (buttonSprite != null)
            buttonSprite.color = pressed ? activeColor : inactiveColor;
    }
}
