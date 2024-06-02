using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LRS
{
    public class Voiceacting : MonoBehaviour
    {        
        [SerializeField] GameObject player;
        [SerializeField] GameObject Trigger;
        [SerializeField] GameObject UI;

        // Start is called before the first frame update
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                StartCoroutine(UIactive());
                this.GetComponent<PlayableDirector>().enabled = true;
                Trigger.SetActive(true);
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }

        }
        IEnumerator UIactive()
        {
            yield return new WaitForSeconds(15);
            UI.SetActive(true);
        }
    }
}
