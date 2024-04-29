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
        [SerializeField] TextMeshProUGUI text;

        void Start()
        {
            text.text = "You have collected " + itemCount + "/5 gems so far keep going";
        }
        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Yellow"))
            {
                
                Destroy(col.gameObject);
                itemCount++;
                text.text = "You have collected " + itemCount +"/5 gems so far keep going";
            }
            if (itemCount > 4)
            {
                text.text = "You have collected the gem in time well done";
                SceneManager.LoadScene("Win");
            }
               
        }
    }
}
