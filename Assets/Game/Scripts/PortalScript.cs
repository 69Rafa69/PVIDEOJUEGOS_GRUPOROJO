using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [SerializeField] private Transform destination;
    [SerializeField] private bool isActive = false; // por defecto desactivado

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return; // si está apagado, no hace nada

        if (portalObjects.Contains(collision.gameObject))
            return;

        if (destination.TryGetComponent(out PortalScript destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        collision.transform.position = destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public bool IsActive()
    {
        return isActive;
    }


void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
