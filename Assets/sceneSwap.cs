using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneSwap : MonoBehaviour
{
    public void goToNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
