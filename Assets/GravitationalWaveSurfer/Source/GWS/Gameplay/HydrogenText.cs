using GWS.Gameplay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GWS.Gameplay
{
    /// <summary>
    /// Displays how much hydrogen the player holds.
    /// </summary>
    public class HydrogenText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        private float time;

        private void OnEnable()
        {
            HydrogenTracker.OnHydrogenChanged += UpdateText;
            UpdateText(0);
        }

        private void OnDisable()
        {
            HydrogenTracker.OnHydrogenChanged -= UpdateText;
        }

        private void UpdateText(int hydrogen)
        {
            text.text = $"{hydrogen}/{HydrogenTracker.HYDROGEN_CAPACITY}";
            text.ForceMeshUpdate();
            time += 0.1f;

            TMP_TextInfo textInfo = text.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible) continue;

                Vector3[] vertices = textInfo.meshInfo[characterInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; ++j)
                {
                    Vector3 original = vertices[characterInfo.vertexIndex + j];
                    vertices[characterInfo.vertexIndex + j] += original + new Vector3(0, Mathf.Sin(time * 2f + original.x * 0.01f) * 10f, 0);

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