using Mushakushi.MenuFramework.Runtime.ExtensionFramework;
using UnityEngine;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Opens a menu on Start. 
    /// </summary>
    public class OpenMenuOnStart: MonoBehaviour
    {
        public MenuEventChannel menuEventChannel;
        public Menu menu;

        public void Start()
        {
            menuEventChannel.RaiseOnOpenRequested(menu);
        }
    }
}