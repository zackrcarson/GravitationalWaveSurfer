using Codice.Client.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Time = UnityEngine.Time;

public class LockIconWiggle : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private float intensity;

    private void Update()
    {
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible) continue;

            Vector3[] vertices = textInfo.meshInfo[characterInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j)
            {
                Vector3 original = vertices[characterInfo.vertexIndex + j];
                vertices[characterInfo.vertexIndex + j] += original + new Vector3(0, Mathf.Sin(Time.time * 2f + original.x * 0.01f) * 9f, 0);

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
