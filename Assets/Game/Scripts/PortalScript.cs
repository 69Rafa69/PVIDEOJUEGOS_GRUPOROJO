using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    // Conjunto para evitar que un mismo objeto se teletransporte repetidamente
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [SerializeField] private Transform destination; // portal de destino
    [SerializeField] private bool isActive = false; // indica si el portal está activo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // si el portal está desactivado, no hace nada
        if (!isActive) return;

        // solo el jugador puede usar el portal
        if (!collision.CompareTag("Player")) return;

        // evita teletransportes múltiples del mismo objeto
        if (portalObjects.Contains(collision.gameObject)) return;

        // registra el objeto en el portal destino para evitar loops
        if (destination.TryGetComponent(out PortalScript destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        // mueve al jugador a la posición del portal destino
        collision.transform.position = destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // cuando el objeto sale del portal, lo eliminamos del registro
        portalObjects.Remove(collision.gameObject);
    }

    // activa o desactiva el portal desde otro script
    public void SetActive(bool active)
    {
        isActive = active;
    }

    // devuelve si el portal está activo
    public bool IsActive()
    {
        return isActive;
    }
}
