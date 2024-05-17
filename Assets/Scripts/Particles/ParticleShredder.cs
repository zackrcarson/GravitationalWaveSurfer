using UnityEngine;

public class ParticleShredder : MonoBehaviour
{
    // Constants
    const string BLACK_HOLE_NAME = "Black Hole";
    const string PLAYER_NAME = "Player";
    const string PARTICLE_CLUMP_NAME = "Particle Clump";

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other) return;
        if (!other.CompareTag(PLAYER_NAME) && !other.CompareTag(BLACK_HOLE_NAME))
        {
            var parent = other.transform.parent;
            Destroy(parent && parent.CompareTag(PARTICLE_CLUMP_NAME)
                ? other.transform.parent.gameObject
                : other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            if (!FindObjectOfType<GameOver>().isGameOver)
            {
                FindObjectOfType<Player>().KillPlayer(BLACK_HOLE_NAME);
            }
        }
    }
}
