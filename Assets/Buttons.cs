using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class Buttons : MonoBehaviour
    {
        void Start () { Cursor.lockState = CursorLockMode.None; }
        public void LevelOne()
        {
            SceneManager.LoadScene("LevelOne");
        }
    }
}
