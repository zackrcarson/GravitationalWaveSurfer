using UnityEngine;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Hides the cursor when this script is enabled and vice versa.
    /// </summary>
    public class UIHideCursor: MonoBehaviour
    {   
        private void OnEnable()
        {
            Cursor.visible = false;
        }
        
        private void OnDisable()
        {
            Cursor.visible = true;
        }
    }
}