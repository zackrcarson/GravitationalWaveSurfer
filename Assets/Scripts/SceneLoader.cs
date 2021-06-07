using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void ReloadCurrentScene()
    {
        Destroy(GameManager.instance.gameObject);

        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
}
