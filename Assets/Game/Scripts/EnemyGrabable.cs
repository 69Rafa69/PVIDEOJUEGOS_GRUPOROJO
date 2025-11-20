using UnityEngine;

public class EnemyGrabable : MonoBehaviour
{
    private bool isGrabbed;
    private Transform originalParent;

    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private CapsuleCollider2D col;
    private Goomba ai; // tu script de movimiento del enemigo

    private void Awake()
    {
        originalParent = transform.parent;
        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        ai = GetComponent<Goomba>();
    }

    public void ChangeEnemyStatus(Transform grabZone)
    {
        isGrabbed = true;

        ai.enabled = false;
        body.simulated = false;
        col.enabled = false;

        transform.SetParent(grabZone);
        transform.localPosition = Vector3.zero;

        sprite.enabled = false; // invisible
    }

    public void HideTemporarily(float delay)
    {
        StartCoroutine(Reappear(delay));
    }

    private System.Collections.IEnumerator Reappear(float delay)
    {
        sprite.enabled = false;
        ai.enabled = false;
        body.simulated = false;
        col.enabled = false;

        yield return new WaitForSeconds(delay);

        // reaparecer y reiniciar
        sprite.enabled = true;
        ai.enabled = true;
        body.simulated = true;
        col.enabled = true;

        isGrabbed = false;
    }
}
