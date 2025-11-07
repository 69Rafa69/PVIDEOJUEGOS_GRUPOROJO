using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject playerReference;
    private Vector2 playerPos;

    private void Start()
    {
        // Busca al jugador por etiqueta
        playerReference = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(playerReference.transform.position);

        // Guarda la posición inicial del jugador
        Vector2 player = playerReference.transform.localPosition;
        playerPos = new Vector2(player.x, player.y);
    }

    private void Update()
    {
        // Calcula el movimiento del jugador desde el último frame
        Vector2 player = playerReference.transform.localPosition;
        float deltaX = player.x - playerPos.x;
        float deltaY = player.y - playerPos.y;

        // Actualiza la posición registrada
        playerPos = new Vector2(player.x, player.y);

        // Desplaza la cámara en la misma dirección y magnitud
        Vector3 cameraPos = transform.localPosition;
        transform.localPosition = new Vector3(cameraPos.x + deltaX, cameraPos.y + deltaY, cameraPos.z);
    }
}
