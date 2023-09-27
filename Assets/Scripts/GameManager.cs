//GameManager.cs
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace CatInMaze
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}