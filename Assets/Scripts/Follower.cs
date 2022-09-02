using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Transform player = null;

    // Variables
    Vector3 PlayerCenterOffset = new Vector3(0, 0, 0);
    float playerSize = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + PlayerCenterOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, player.rotation.z * -1f);
    }

    public void UpdatePlayerOffset(Vector3 playerOffset, float playerRadius)
    {
        PlayerCenterOffset = playerOffset;
        playerSize = playerRadius;

        Debug.Log((playerOffset, PlayerCenterOffset, playerSize));
        Time.timeScale = 0;
    }
}
