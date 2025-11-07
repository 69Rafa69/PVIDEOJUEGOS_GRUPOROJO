using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PortalScript : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    [Header("Configuración del portal")]
    [SerializeField] private Transform destination;     // Portal de destino
    [SerializeField] private bool isActive = false;     // Estado actual del portal

    [Header("Sprites del portal")]
    [SerializeField] private Sprite activeSprite;       // Sprite cuando está activo
    [SerializeField] private Sprite inactiveSprite;     // Sprite cuando está inactivo

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdatePortalSprite(); // Asegura que el sprite inicial sea el correcto
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el portal está desactivado, no hace nada
        if (!isActive) return;

        // Solo el jugador puede usar el portal
        if (!collision.CompareTag("Player")) return;

        // Evita teletransportes múltiples del mismo objeto
        if (portalObjects.Contains(collision.gameObject)) return;

        // Registra el objeto en el portal destino para evitar loops
        if (destination != null && destination.TryGetComponent(out PortalScript destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        // Mueve al jugador al portal destino
        collision.transform.position = destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Libera al objeto del registro cuando sale del portal
        portalObjects.Remove(collision.gameObject);
    }

    public void SetActive(bool active)
    {
        // Solo actualiza si cambia el estado
        if (isActive == active) return;

        isActive = active;
        UpdatePortalSprite();
    }

    private void UpdatePortalSprite()
    {
        if (spriteRenderer == null) return;

        // Cambia el sprite según el estado
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
    }

    public bool IsActive()
    {
        return isActive;
    }
}
