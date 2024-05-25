using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class Buttons : MonoBehaviour
    {
        void Start () { Cursor.lockState = CursorLockMode.Confined;  }
        [SerializeField] Animator animator;
        public void LevelOne()
        {
            SceneManager.LoadScene("LevelOne");
        }
        public void Lose() 
        {
            if (Itemcollerctor.level)
            {
                SceneManager.LoadScene("LevelTwo");
            }
            else { SceneManager.LoadScene("LevelOne"); }
        }
        public void QuitGame()
        {
            Application.Quit();
        }
        public void Tutorial ()
        {
            SceneManager.LoadScene("Tutorial");
        }
        public void FadeIn()
        {
            animator.SetBool("Fade", false);
        }
        public void FadeOut() 
        {
            animator.SetBool("Fade", true);
        }
    }
}
