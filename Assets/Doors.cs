using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LRS
{
    public class Doors : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Animator anim;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                anim.SetBool("Door State", true);
            }
        }
        private void OnTriggerExit(Collider other) 
        {  
            if (other.gameObject == player) 
            {
                anim.SetBool("Door State", false);
            } 
        }
        public void Open() { anim.SetBool("Door State", true); }
        public void Close() { anim.SetBool("Door State", false); }
    }
}
