using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class Itemcollerctor : MonoBehaviour
    {
        int itemCount = 0;
        [SerializeField] int colectableAmount; 
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] string scene;
        public static bool level = false;

        void Start()
        {
            text.text = "You have collected " + itemCount + "/"+ colectableAmount + "parts so far keep going";
        }
        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Yellow"))
            {
                
                Destroy(col.gameObject);
                itemCount++;
                text.text = "You have collected " + itemCount + "/"+ colectableAmount + " gems so far keep going";
            }
            if (itemCount >= colectableAmount)
            {
                text.text = "You have collected the gem in time well done";
                SceneManager.LoadScene(scene);
                level = true;
            }
               
        }
    }
}
