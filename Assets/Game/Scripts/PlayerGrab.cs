using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Transform grabZone;

    private InputAction interactAction;
    private EnemyGrabable currentEnemy; // enemigo actual dentro del trigger
    private EnemyGrabable grabbedEnemy; // enemigo actualmente agarrado
    private bool isGrabbing = false;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        // Si presiona la tecla de interacción
        if (interactAction.WasPressedThisFrame())
        {
            if (!isGrabbing && currentEnemy != null)
            {
                // Agarra al enemigo si no hay ninguno agarrado
                grabbedEnemy = currentEnemy;
                grabbedEnemy.ChangeEnemyStatus(grabZone);
                isGrabbing = true;
            }
            else if (isGrabbing && grabbedEnemy != null)
            {
                // Suelta al enemigo actual
                grabbedEnemy.ChangeEnemyStatus(null);
                grabbedEnemy = null;
                isGrabbing = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyGrab") && !isGrabbing)
        {
            currentEnemy = collision.GetComponent<EnemyGrabable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el enemigo sale del rango y no lo estás agarrando, lo olvidamos
        if (collision.CompareTag("EnemyGrab") && !isGrabbing)
        {
            if (collision.GetComponent<EnemyGrabable>() == currentEnemy)
            {
                currentEnemy = null;
            }
        }
    }
}