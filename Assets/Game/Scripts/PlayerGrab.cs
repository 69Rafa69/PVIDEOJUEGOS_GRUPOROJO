using UnityEngine;

using UnityEngine.InputSystem;

using System.Collections;


[RequireComponent(typeof(AudioSource))]

public class PlayerGrab : MonoBehaviour

{

    [Header("Referencias")]

    [SerializeField] private Transform grabZone;

    [Tooltip("Coloca este objeto hijo en los pies del jugador o donde la animación deja al enemigo.")]

    [SerializeField] private Transform dropPoint;


    [Header("Sincronización de Animación")]

    [Tooltip("Tiempo en segundos que tarda la animación en llegar al punto de soltar.")]

    [SerializeField] private float dropAnimationDelay = 0.4f; // <--- AJUSTA ESTO


    [Header("Configuración de Lanzamiento")]

    [SerializeField] private float throwForce = 10f;

    [SerializeField] private float throwArc = -2.0f;


    [Header("Audio")]

    [SerializeField] private AudioClip grabSound;

    [SerializeField] private AudioClip dropSound;


    // Inputs y Estado

    private InputAction interactAction;

    private InputAction moveAction;

    private EnemyGrabable currentEnemy;

    private EnemyGrabable grabbedEnemy;

    private bool isGrabbing = false;


    // Componentes

    private Animator animator;

    private AudioSource audioSource;


    private void Awake()

    {

        audioSource = GetComponent<AudioSource>();


        if (dropPoint == null)

        {

            GameObject autoDrop = new GameObject("AutoDropPoint");

            autoDrop.transform.SetParent(transform);

            // Lo ponemos abajo y adelante (ajusta esto en el editor)

            autoDrop.transform.localPosition = new Vector3(0.5f, -0.5f, 0);

            dropPoint = autoDrop.transform;

            Debug.LogWarning("PlayerGrab: Se creó un DropPoint automático. Ajústalo en el editor para que coincida con el suelo.");

        }

    }


    private void Start()

    {

        interactAction = InputSystem.actions.FindAction("Interact");

        moveAction = InputSystem.actions.FindAction("Move");

        animator = GetComponent<Animator>();

    }


    private void Update()

    {

        HandleAnimations();

        HandleInput();

    }


    private void HandleAnimations()

    {

        if (isGrabbing)

        {

            float moveX = 0f;

            if (moveAction != null) moveX = Mathf.Abs(moveAction.ReadValue<Vector2>().x);


            animator.SetBool("isWalking", moveX > 0.1f);

            animator.SetBool("isHolding", true);

        }

        else

        {

            animator.SetBool("isHolding", false);

        }

    }


    private void HandleInput()

    {

        if (interactAction.WasPressedThisFrame())

        {

            if (!isGrabbing && currentEnemy != null)

            {

                Grab();

            }

            else if (isGrabbing && grabbedEnemy != null)

            {

                Throw();

            }

        }

    }


    private void Grab()

    {

        grabbedEnemy = currentEnemy;

        grabbedEnemy.ChangeEnemyStatus(grabZone);


        isGrabbing = true;

        animator.SetTrigger("Pickup");

        PlaySound(grabSound);

    }


    private void Throw()

    {

        // 1. Iniciamos la animación y el sonido

        animator.SetTrigger("Drop");

        PlaySound(dropSound);


        // 2. Iniciamos la espera para sincronizar con la animación

        StartCoroutine(ThrowRoutine());

    }


    private IEnumerator ThrowRoutine()

    {

        // Esperamos a que la animación llegue al punto donde el enemigo toca el suelo

        yield return new WaitForSeconds(dropAnimationDelay);


        if (grabbedEnemy != null)

        {

            // 3. Calcular dirección (basado en hacia dónde mira el jugador)

            float facingDir = transform.localScale.x > 0 ? 1f : -1f;

            Vector2 direction = new Vector2(facingDir, throwArc).normalized;


            // 4. CORRECCIÓN DE POSICIÓN:

            // Movemos el objeto enemigo a la posición del dropPoint (el suelo) 

            // ANTES de reactivar sus físicas.

            grabbedEnemy.transform.position = dropPoint.position;


            // 5. Soltamos y reactivamos físicas

            grabbedEnemy.ThrowEnemy(direction, throwForce);

        }


        // Limpiar estado

        isGrabbing = false;

        grabbedEnemy = null;

    }


    private void PlaySound(AudioClip clip)

    {

        if (audioSource != null && clip != null)

            audioSource.PlayOneShot(clip);

    }


    private void OnTriggerEnter2D(Collider2D collision)

    {

        if (!isGrabbing && collision.CompareTag("EnemyGrab"))

        {

            if (collision.TryGetComponent<EnemyGrabable>(out var enemy))

            {

                currentEnemy = enemy;

            }

        }

    }


    private void OnTriggerExit2D(Collider2D collision)

    {

        if (!isGrabbing && collision.CompareTag("EnemyGrab"))

        {

            if (collision.TryGetComponent<EnemyGrabable>(out var enemy) && enemy == currentEnemy)

            {

                currentEnemy = null;

            }

        }

    }

}