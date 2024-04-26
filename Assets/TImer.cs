using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class TImer : MonoBehaviour
    {
        public int Seconds = 180;
        [SerializeField] TextMeshProUGUI Timer;
        void Start()
        {
            StartCoroutine(Countdown());
        }

        
        IEnumerator Countdown()
        {
            yield return new WaitForSeconds(1);
            Seconds--;
            Timer.text = ""+ Seconds;
            if (Seconds == 0)
            {
                SceneManager.LoadScene("Lose");
            }
            StartCoroutine(Countdown()); 
        }
    }
}
