using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Transform objectToFollow = null;

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.position;
    }
}
