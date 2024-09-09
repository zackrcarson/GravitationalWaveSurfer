using UnityEngine;

namespace GWS.WorldGen
{
    public class BlackHoleManager : MonoBehaviour
    {
        public static BlackHoleManager Instance { get; private set; }

        [Header("Black hole interaction parameters")]
        public float interactionRadius;
        public float gravitationalConstant = 100000f;

        [Space(6)]
        [Header("Relevant GameObjects")]
        public GameObject player;
        public GameObject blackHole;
        private Rigidbody playerRigidbody;

        private void Awake()
        {
            if (Instance = null) Instance = this;
        }

        private void Start()
        {
            interactionRadius = ChunkManager.Instance.chunkSize;

            if (player == null) Debug.LogWarning("BlackHoleManager: Player object not set!!!");
            playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody == null) Debug.LogWarning("BlackHoleManager: Player object doesn't have a rigidbody!!!");
        }

        private void Update()
        {
            CheckBlackHoleInVicinity();    
        }

        private void CheckBlackHoleInVicinity()
        {
            Chunk currentChunk = ChunkManager.Instance.GetCurrentChunk();
            if (currentChunk.HasBlackHole)
            {
                blackHole = currentChunk.ChunkObject.transform.GetChild(0).gameObject;

                float distance = Vector3.Distance(player.transform.position, blackHole.transform.position);
                Vector3 direction = blackHole.transform.position - player.transform.position;

                if (distance < interactionRadius)
                {
                    // Debug.Log("Black hole nearby");

                    float forceMagnitude = gravitationalConstant / (distance * distance);

                    Vector3 force = direction.normalized * forceMagnitude;

                    playerRigidbody.AddForce(force, ForceMode.Force);
                }
            }

        }
    }
}