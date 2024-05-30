using Mushakushi.MenuFramework.Runtime.ExtensionFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace Example
{
    public class Example : MonoBehaviour
    {
        public MenuEventChannel menuEventChannel;
        public Menu initialMenu;

        public void Start()
        {
            menuEventChannel.RaiseOnOpenRequested(initialMenu);
        }

        public void Print(Button element)
        {
            Debug.Log(element.name);
        }
    }
}