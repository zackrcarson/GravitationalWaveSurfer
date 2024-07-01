using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.Gameplay
{
    /// <summary>
    /// Very temporary implementation of hydrogen spawning. This should be deleted eventually.
    /// </summary>
    public class HydrogenSpawner: MonoBehaviour
    {
        [SerializeField]
        private GameObject hydrogen;

        [SerializeField]
        private float spawnRate;

        private void Start()
        {
            InvokeRepeating("SpawnHydrogen", 0f, spawnRate);
        }

        private void SpawnHydrogen()
        {
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );

            Instantiate(hydrogen, randomPosition, Quaternion.identity);
        }
    }
}