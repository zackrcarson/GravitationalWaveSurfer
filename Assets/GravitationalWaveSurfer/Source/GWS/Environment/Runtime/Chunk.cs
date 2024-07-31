using System.Collections.Generic;
using UnityEngine;

namespace GWS.WorldGen
{
    /// <summary>
    /// Chunk class, contains information about a chunk in the game <br/>
    /// - position in chunk coordinates <br/>
    /// - chunk GameObject (folder) <br/>
    /// - boolean for active or not <br/>
    /// - list of objects in the chunk 
    /// </summary>
    public class Chunk
    {
        public Vector3Int Position { get; private set; }
        /// <summary>
        /// GameObject of chunk: a Unity folder containing objects within the chunk
        /// </summary>
        public GameObject ChunkObject { get; private set; }
        public bool IsActive { get; private set; }
        public List<GameObject> Objects { get; private set; }

        public Chunk (Vector3Int position, GameObject chunkParent)
        {
            Position = position;
            ChunkObject = new GameObject($"Chunk_{position.x}_{position.y}_{position.z}");
            ChunkObject.transform.parent = chunkParent.transform;
            ChunkObject.layer = ChunkObject.transform.parent.gameObject.layer;
            IsActive = false;
            Objects = new List<GameObject>();
        }

        public void SetObjects(List<GameObject> objects)
        {
            Objects = objects;
        }

        public void SetActive(bool active)
        {
            IsActive = active;
            ChunkObject.SetActive(active);
        }
    }
}