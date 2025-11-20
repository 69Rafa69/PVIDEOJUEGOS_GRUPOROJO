using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;   // punto donde reaparece el player

    private Animator animator;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private PlayerGrab grabScript;
    private Player_Movement moveScript;   // ← tu script real de movimiento

    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        grabScript = GetComponent<PlayerGrab>();
        moveScript = GetComponent<Player_Movement>();

        // Si no se asignó un respawnPoint, usamos la posición inicial del player
        if (respawnPoint == null)
        {
            GameObject autoRespawn = new GameObject("AutoRespawnPoint");
            autoRespawn.transform.position = transform.position;
            respawnPoint = autoRespawn.transform;
        }
    }

    /// <summary>
    /// Llamar a esta función cuando el player deba morir.
    /// </summary>
    public void KillPlayer()
    {
        if (isDead) return;   // evitar múltiples llamadas

        isDead = true;

        // Desactivar control de movimiento y grab
        if (moveScript != null) moveScript.enabled = false;
        if (grabScript != null) grabScript.enabled = false;

        // Detener físicas
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Desactivar colisión
        if (col != null) col.enabled = false;

        // Activar animación de muerte
        animator.SetBool("Dead", true);

        // Lanzar corrutina de respawn
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // 1) Esperar hasta que entremos a Player_Death
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Death"))
            yield return null;

        // 2) Esperar a que termine la animación (normalizedTime >= 1)
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        while (info.normalizedTime < 1f)
        {
            yield return null;
            info = animator.GetCurrentAnimatorStateInfo(0);
        }

        // 3) Respawn
        transform.position = respawnPoint.position;

        // 4) Reactivar físicas y colisión
        rb.simulated = true;
        if (col != null) col.enabled = true;

        // 5) Reactivar scripts de control
        if (moveScript != null) moveScript.enabled = true;
        if (grabScript != null) grabScript.enabled = true;

        // 6) Quitar estado Dead y forzar Idle
        animator.SetBool("Dead", false);
        animator.Play("Player_Idle", 0, 0f);

        isDead = false;
    }
}
