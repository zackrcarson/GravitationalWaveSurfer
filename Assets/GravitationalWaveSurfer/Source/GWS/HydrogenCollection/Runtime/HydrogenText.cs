using TMPro;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Displays how much hydrogen the player holds.
    /// </summary>
    public class HydrogenText : MonoBehaviour
    {
        [SerializeField] 
        private ParticleInventoryEventChannel particleInventoryEventChannel;

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
        /// If hydrogen value below 1e5, show actual number <br/>
        /// if higher, show scientific notation of number
        /// </summary>
        /// <param name="hydrogen">Hydrogen amount to be displayed.</param>
        private void UpdateText(double hydrogen)
        {
            if (HydrogenManager.Instance.CurrentProgressBarText == null) return;
            if (hydrogen < 100000)
            {
                HydrogenManager.Instance.CurrentProgressBarText.text = $"{hydrogen}";
            }
            else
            {
                string scientificNotation = hydrogen.ToString($"E{4}");
                HydrogenManager.Instance.CurrentProgressBarText.text = $"{scientificNotation}";
            }
            WiggleText(0.05f);
        }


        private void WiggleText(float intensity)
        {
            HydrogenManager.Instance.CurrentProgressBarText.ForceMeshUpdate();
            time += intensity;

            TMP_TextInfo textInfo = HydrogenManager.Instance.CurrentProgressBarText.textInfo;
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
                HydrogenManager.Instance.CurrentProgressBarText.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}
