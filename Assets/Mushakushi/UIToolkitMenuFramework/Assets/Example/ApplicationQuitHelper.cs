using Mushakushi.MenuFramework.Runtime.Extensions;
using UnityEngine;

namespace Example
{
    /// <summary>
    /// Attach this to the scene to able to quit the application with a certain key. 
    /// </summary>
    public class ApplicationQuitHelper: MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Event.KeyboardEvent"/> key  that will close the application. 
        /// </summary>
        [SerializeField] private string key = "escape"; 

        /// <summary>
        /// Quits the application if <see cref="key"/> is pressed. 
        /// </summary>
        private void OnGUI()
        {
            // works even with the old input system disabled
            if (Event.current.Equals(Event.KeyboardEvent(key)))
            {
                ApplicationQuitButtonExtension.QuitApplication();
            }
        }
    }
}