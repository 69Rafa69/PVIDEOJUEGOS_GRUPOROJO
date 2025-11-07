using UnityEngine;

public class EnemyGrabable : MonoBehaviour
{
    private bool isGrabbed;
    private Transform enemyContainer;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D body;

    private void Awake()
    {
        // Inicializa referencias y estado inicial
        isGrabbed = false;
        enemyContainer = transform.parent;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    public void ChangeEnemyStatus(Transform grabZone)
    {
        if (!isGrabbed)
        {
            // Asocia el enemigo a la zona de agarre
            transform.SetParent(grabZone);
            transform.localPosition = Vector3.zero;
            isGrabbed = true;

            // Desactiva colisión y física mientras está agarrado
            capsuleCollider.enabled = false;
            body.simulated = false;
        }
        else
        {
            // Restaura el enemigo a su contenedor original
            transform.SetParent(enemyContainer);
            isGrabbed = false;

            // Reactiva colisión y física
            capsuleCollider.enabled = true;
            body.simulated = true;
        }
    }
}
