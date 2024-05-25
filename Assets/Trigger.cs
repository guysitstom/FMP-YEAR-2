using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LRS
{
    public class Trigger : MonoBehaviour
    {
        
        [SerializeField] GameObject player;
        [SerializeField] GameObject Triggers;
        [SerializeField] GameObject P23;
        [SerializeField] GameObject Chair;
        [SerializeField] AudioSource audioplayer;
        [SerializeField] Animator P23sit;
        Vector3 Chairpos;

        // Start is called before the first frame update
        void Start()
        {
            Chairpos = Chair.transform.position;
            Chairpos.y = -0.25f;
        }
        void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject == player)
            {
                P23.transform.position = Chairpos;
                P23.transform.rotation = Chair.transform.rotation;
                P23sit.SetBool("Sitting",true);
                audioplayer.Play();
                Triggers.SetActive(true);
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }

        }
    }
}
