using UnityEngine;

public class ParticleText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Player";
        this.gameObject.GetComponent<MeshRenderer>().sortingOrder = 1;
    }
}

