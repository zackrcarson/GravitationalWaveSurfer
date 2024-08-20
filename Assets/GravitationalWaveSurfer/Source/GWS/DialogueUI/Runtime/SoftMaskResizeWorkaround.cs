using System.Linq;
using System.Reflection;
using Coffee.UISoftMask;
using UnityEngine;

namespace GWS.DialogueUI.Runtime
{
    [System.Obsolete("This doesn't fix the issue, and is being actively worked on by developer: https://github.com/mob-sakai/SoftMaskForUGUI/issues/184.")]
    [ExecuteInEditMode, RequireComponent(typeof(SoftMask))]
    public class SoftMaskDisappearanceWorkaround : MonoBehaviour
    {
        private SoftMask softMask;      
        private MaterialPropertyBlock materialPropertyBlock; 
        private MethodInfo renderSoftMaskBuffer; 
        private FieldInfo hasResolutionChanged;
        private const BindingFlags NonPublicInstanceBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly int MainTextureID = Shader.PropertyToID("_MainTex");

        private void OnRectTransformDimensionsChange()
        {
            if (softMask == null && (softMask = GetComponent<SoftMask>()) == null) return;
            if (!gameObject.activeInHierarchy || !softMask.enabled) return;
        
            materialPropertyBlock ??= softMask.GetType()
                .GetField("_mpb", NonPublicInstanceBindingFlags)
                ?.GetValue(softMask) as MaterialPropertyBlock;
            renderSoftMaskBuffer ??= softMask.GetType()
                .GetMethods(NonPublicInstanceBindingFlags)
                .FirstOrDefault(x => x.Name == "RenderSoftMaskBuffer" && x.GetParameters().Length == 0);
            hasResolutionChanged ??= softMask.GetType()
                .GetField("_hasResolutionChanged", NonPublicInstanceBindingFlags);
        
            materialPropertyBlock?.SetTexture(MainTextureID, softMask.softMaskBuffer);
            hasResolutionChanged?.SetValue(softMask, true);
            renderSoftMaskBuffer?.Invoke(softMask, null);
        }
    }
}