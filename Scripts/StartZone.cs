using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;
    public AudioSource start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")//���� ����
        {
            start.Play();
            manager.StageStart();
        }
    }

}
