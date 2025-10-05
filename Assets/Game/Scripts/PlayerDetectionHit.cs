using UnityEngine;

public class PlayerDetectionHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el enemigo est� paralizado antes de hacer respawn
        if (collision.gameObject.CompareTag("DeadZone") ||
            (collision.gameObject.CompareTag("Enemy") && !IsEnemyParalyzed(collision.gameObject)))
        {
            SpawnPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            SpawnPlayer();
            // Desaparecemos la bala
            Destroy(collision.gameObject);
        }
    }

    // M�todo para verificar si un enemigo est� paralizado
    private bool IsEnemyParalyzed(GameObject enemy)
    {
        // Buscar el componente IParalyzable en el enemigo
        IParalyzable paralyzableEnemy = enemy.GetComponent<IParalyzable>();

        // Si el enemigo tiene el componente IParalyzable, verificar su estado
        if (paralyzableEnemy != null)
        {
            // Asume que tienes una propiedad o m�todo isParalyzed en tu interfaz
            // Si no la tienes, necesitar�s modificar tu interfaz IParalyzable
            return paralyzableEnemy.isParalyzed;
        }

        // Si no tiene el componente, asumir que no est� paralizado
        return false;
    }

    private void SpawnPlayer()
    {
        // Ubicamos el SpawnPoint, eso significa que el spawnpoint debe tener su etiqueta (tag)
        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        // Mandamos al player a esa posici�n.
        transform.localPosition = spawn.transform.localPosition;
    }
}
