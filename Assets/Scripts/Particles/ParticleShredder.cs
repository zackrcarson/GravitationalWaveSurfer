using UnityEngine;

public class ParticleShredder : MonoBehaviour
{
    // Constants
    const string BLACK_HOLE_NAME = "Black Hole";
    const string PLAYER_NAME = "Player";
    const string PARTICLE_CLUMP_NAME = "Particle Clump";

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != PLAYER_NAME && other.tag != BLACK_HOLE_NAME)
        {
            if (other.transform.parent.tag == PARTICLE_CLUMP_NAME) 
            { 
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "Player")
        {
            if (!FindObjectOfType<GameOver>().isGameOver)
            {
                FindObjectOfType<Player>().KillPlayer(BLACK_HOLE_NAME);
            }
        }
    }
}
