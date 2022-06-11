using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTuto : MonoBehaviour
{
    public GameManager manager;
    public int where;
    public AudioSource enter;
    public AudioSource login;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")//게임 시작
        {
            if (where == 0)
            {
                enter.Play();
                manager.GoTuto();
            }
            else if (where == 1)
            {
                enter.Play();
                manager.BackLobby();
            }
            else if (where == 2)
            {
                login.Play();
                manager.GoQuiz();
            }
            else if (where == 3)
                Application.Quit();
        }
    }
}
