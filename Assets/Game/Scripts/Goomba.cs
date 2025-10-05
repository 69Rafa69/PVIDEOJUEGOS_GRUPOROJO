using UnityEngine;
using System.Collections;

public class Goomba : MonoBehaviour, IParalyzable
{
    [SerializeField] private float speedX; 
    [SerializeField] private float limitRight;
    [SerializeField] private float limitLeft;
    [SerializeField] private float paralysisDuration = 10f;

    private Vector2 limits;
    private int direction;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector3 originalPosition;
    public bool isParalyzed { get; private set; } = false;  //atributo de la interfaz

    public void Paralyze()
    {
        if (!isParalyzed)   //Esta paralizado?
        {
            Debug.Log($"Goomba paralizado durante {paralysisDuration} segundos");
            StartCoroutine(ParalysisRoutine()); //Iniciar corrutina (se necesita para pausar al enemigo sin pausar con wait)
        }
    }
    //Hilo de paralisis
    private IEnumerator ParalysisRoutine()
    {
        isParalyzed = true;
        body.linearVelocityX = 0f; // Detener el movimiento

        // Cambiar el color 
        sprite.color = Color.gray;

        //Esperar la duración de la paralisis
        yield return new WaitForSeconds(paralysisDuration); //Esperar

        // Restaurar estado normal
        isParalyzed = false;    //Cambiar estado de paralisis
        sprite.color = Color.white; //Color normal
        direction = 1; // Reiniciar dirección por defecto
    }

    private void Awake()
    {
        Vector3 pos = transform.localPosition;
        originalPosition = pos;
        limits = new Vector2(pos.x - limitLeft, pos.x + limitRight);

        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        direction = 1; // Hacia la derecha
    }

    private void Update()
    {
        // Solo permitir movimiento si no está paralizado
        if (!isParalyzed)
        {
            if (direction != 0)
            {
                sprite.flipX = direction < 0;
            }

            Vector3 pos = transform.localPosition;
            if (pos.x <= limits.x)
            {
                direction = 1;
            }
            if (pos.x >= limits.y)
            {
                direction = -1;
            }

            body.linearVelocityX = direction * speedX;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = originalPosition != Vector3.zero ? originalPosition : transform.localPosition;
        Vector3 posLeft = new Vector3(pos.x - limitLeft, pos.y, pos.z);
        Vector3 posRight = new Vector3(pos.x + limitRight, pos.y, pos.z);
        Gizmos.DrawSphere(posLeft, 0.5f);
        Gizmos.DrawSphere(posRight, 0.5f);
    }
}
