using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
}
