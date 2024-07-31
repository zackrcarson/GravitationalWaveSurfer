using UnityEngine;

namespace GWS.WorldGen
{
    /// <summary>
    /// - Loads/unloads chunks based on player's position<br />
    /// - A threshold controls the frequency of such updates<br />
    /// (calls functions implemented in ChunkManager)
    /// </summary>
    public class PlayerChunkLoader : MonoBehaviour
    {
        private Vector3 lastUpdatedPosition;
        public float updateThreshold = 5f;

        private void Start()
        {
            lastUpdatedPosition = transform.position;
            ChunkManager.Instance?.UpdatePlayerPosition(transform.position);
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, lastUpdatedPosition) > updateThreshold)
            {
                lastUpdatedPosition = transform.position;
                ChunkManager.Instance?.UpdatePlayerPosition(transform.position);
            }    
        }
    }
}