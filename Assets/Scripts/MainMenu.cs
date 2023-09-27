using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatInMaze
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
