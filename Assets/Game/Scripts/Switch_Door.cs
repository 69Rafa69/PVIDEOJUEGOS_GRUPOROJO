using UnityEngine;

public class Switch_Door : MonoBehaviour
{
    [SerializeField] private GameObject door;  // La puerta a abrir
    [SerializeField] private bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log("Switch activado por bala!");

            // Ejemplo: abre la puerta
            if (door != null)
            {
                Door_Controller doorScript = door.GetComponent<Door_Controller>();
                if (doorScript != null)
                {
                    doorScript.OpenDoor();
                }
            }
        }
    }
}
