using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScroller : MonoBehaviour
{
    // Config parameters
    [SerializeField] float scrollSpeed = 0.005f;

    // Cached references
    Vector2 randomDirection;

    Material material;
    Vector2 materialOffset;

    // Use this for initialization
    void Start ()
    {
        GetComponent<Renderer>().sortingLayerName = "Background";

        material = GetComponent<Renderer>().material;

        randomDirection = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;

        materialOffset = randomDirection * scrollSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        material.mainTextureOffset += materialOffset * Time.deltaTime;
    }
}
