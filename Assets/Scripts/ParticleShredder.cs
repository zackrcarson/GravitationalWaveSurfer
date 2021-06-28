using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShredder : MonoBehaviour
{
    // Constants
    const string BLACK_HOLE_NAME = "Black Hole";

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player" && other.tag != "Black Hole")
        {
            Destroy(other.gameObject);
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
