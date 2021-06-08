using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // Config parameters
    [SerializeField] float nebulaScrollSpeed = 0.02f;
    [SerializeField] float smallStarsScrollSpeed = 0.004f;
    [SerializeField] float bigStarsScrollSpeed = 0.008f;

    [SerializeField] GameObject[] nebulae = null;
    [SerializeField] GameObject smallStars = null;
    [SerializeField] GameObject bigStars = null;

    // Cached references
    Vector2 randomDirection;

    Material nebulaMaterial;
    Vector2 nebulaMaterialOffset;

    Material smallStarsMaterial;
    Vector2 smallStarsMaterialOffset;

    Material bigStarsMaterial;
    Vector2 bigStarsMaterialOffset;

    // Use this for initialization
    void Start ()
    {
        int randomNebula = Random.Range(0, nebulae.Length);
        GameObject nebula = nebulae[randomNebula];

        randomDirection = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;

        nebulaMaterial = nebula.GetComponent<Renderer>().material;
        nebulaMaterialOffset = randomDirection * nebulaScrollSpeed;

        smallStarsMaterial = smallStars.GetComponent<Renderer>().material;
        smallStarsMaterialOffset = randomDirection * smallStarsScrollSpeed;

        bigStarsMaterial = bigStars.GetComponent<Renderer>().material;
        bigStarsMaterialOffset = randomDirection * bigStarsScrollSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        nebulaMaterial.mainTextureOffset += nebulaMaterialOffset * Time.deltaTime;
        smallStarsMaterial.mainTextureOffset += smallStarsMaterialOffset * Time.deltaTime;
        bigStarsMaterial.mainTextureOffset += bigStarsMaterialOffset * Time.deltaTime;
    }
}
