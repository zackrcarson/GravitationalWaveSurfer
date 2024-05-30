# UI Toolkit Menu Framework 
A framework for expediting the creation of menus using UI toolkit. 

## Installation 

**Install via git URL**

from the Add package from git URL option, enter:

```bash
https://github.com/Mushakushi/UIToolkitMenuFramework.git?path=Assets/Mushakushi.MenuFramework
```

If you are specifying a version, append #{VERSION} to the end of the git URL. 

```bash
https://github.com/Mushakushi/UIToolkitMenuFramework.git?path=Assets/Mushakushi.MenuFramework#{VERSION}
```

When using the git URL, install the following upm dependencies:

https://openupm.com/packages/com.mackysoft.serializereference-extensions/

https://openupm.com/packages/com.rotaryheart.serializabledictionarylite/

## Usage

### Setup the Menu Controller 
The menu controller provides a simple way to process input (using the Unity Input System) to navigate to 
and process the data within menus. It also contains a global set of extensions 
which will be applied to each menu.

1. Create a `UI Document` and assign it a source asset.
2. Create a `Menu Event Channel`, which is responsible for communicating menu events, and a `PlayerInput` component, which will be used by certain extensions to understand in what context the current menu was opened in. 
3. Attach  the `MenuController` script to some GameObject. Assign it the previous UI Document and select the `RootContainerName` (if left empty it will use the entire UI Document as the root container) and the `InitialFocusedElementClassName` (if left empty nothing will be focused on any menu, otherwise the first Visual Element with this class name will be focused on every menu when it first populates).

A basic setup would look something like this: 
![image](https://github.com/Mushakushi/UIToolkitMenuFramework/assets/60948236/556c93bb-57bf-412f-8929-a83aa9880842)

### Basic Usage
Just use the `Menu Event Channel` to subscribe and invoke menu events. Example usage can be found in the [Example folder](https://github.com/Mushakushi/UIToolkitMenuFramework/tree/main/Assets/Example).

**About Extensions**

An extension is some piece of code that is called when an UXML menu is attached
to the Menu Controller's root document. For example, a `MenuConnectionButtonExtension`
will trigger a menu to be populated on the screen based on a query, to which multiple 
of these connections can exist.  

## Create a Menu
A menu is a Scriptable Object containing a UXML menu and any extensions applied to it.
A `MenuConnectionButtonExtension` is added by default. 

## Attributes

### NameClassSelectorAttribute
Add this attribute to a string or string collection, in order to get a list of 
name or classes from a `VisualTreeAsset`, `UIDocument` or a string collection. 
This is used throughout the project to avoid having to give hard-coded strings. 

```csharp
public UIDocument document; 

[NameClassSelector(nameof(document), SelectorMode.Class)]
public string className; 
```

### UQueryBuilderSerializable
In combination with the `NameClassSelectorAttribute`, this class creates a psuedo-`UQueryBuilder`
that allows you to use much of the same selectors within the editor and then build into 
the actual class. 
