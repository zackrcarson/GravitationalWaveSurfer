using GWS.HydrogenCollection.Runtime;
using TMPro;
using UnityEngine;

namespace GWS.HydrogenCollectionUI.Runtime
{
    /// <summary>
    /// Displays how much hydrogen the player holds.
    /// </summary>
    public class HydrogenText : MonoBehaviour
    {
        [SerializeField] 
        private ParticleInventory particleInventory;
        
        [SerializeField]
        private TMP_Text text;

        private float time;

        private void Start()
        {
            UpdateText(0);
        }

        private void FixedUpdate()
        {
            WiggleText(0.05f);
        }

        private void Update()
        {
            UpdateText(particleInventory.HydrogenCount);
        }

        /// <summary>
        /// Set's the text value.
        /// </summary>
        /// <param name="hydrogen">Hydrogen amount to be displayed.</param>
        private void UpdateText(int hydrogen)
        {
            text.text = $"{hydrogen}/{HydrogenTracker.HYDROGEN_CAPACITY}";
            WiggleText(0.05f);
        }


        private void WiggleText(float intensity)
        {
            text.ForceMeshUpdate();
            time += intensity;

            TMP_TextInfo textInfo = text.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible) continue;

                Vector3[] vertices = textInfo.meshInfo[characterInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; ++j)
                {
                    Vector3 original = vertices[characterInfo.vertexIndex + j];
                    vertices[characterInfo.vertexIndex + j] += original + new Vector3(0, Mathf.Sin(time * 2f + original.x * 0.01f) * 9f, 0);

                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {

                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                text.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}
