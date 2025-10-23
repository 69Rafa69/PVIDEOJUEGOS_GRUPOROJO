using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PortalScript[] portals;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;

    private bool isPressed = false;

    private void Start()
    {
        // Asegurar que todos los portales comiencen desactivados
        foreach (var portal in portals)
        {
            portal.SetActive(false);
        }

        if (buttonSprite != null)
            buttonSprite.color = inactiveColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPressed && collision.CompareTag("Player"))
        {
            isPressed = true;
            foreach (var portal in portals)
            {
                portal.SetActive(true);
            }

            if (buttonSprite != null)
                buttonSprite.color = activeColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
