using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private string sceneName;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
