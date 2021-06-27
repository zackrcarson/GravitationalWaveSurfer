using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShredder : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player" && other.tag != "Black hole")
        {
            Destroy(other.gameObject);
        }
    }
}
