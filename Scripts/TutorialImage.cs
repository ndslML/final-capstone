using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImage : MonoBehaviour
{
    public enum Type { trigger, inter, story, Quiz};
    public Type type;
    public string TutoText;
    public Text Tutoimage;
    public GameObject TutoGroup;

    private void OnTriggerEnter(Collider other)
    {
        if (type == Type.trigger)
        {
            TutoGroup.SetActive(true);
            Tutoimage.text = TutoText;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if(type == Type.inter && Input.GetButton("Interation"))
        {
            TutoGroup.SetActive(true);
            Tutoimage.text = TutoText;
        }
        else if (type == Type.story)
        {
            TutoGroup.SetActive(true);
            Tutoimage.text = TutoText;
            Destroy(gameObject);
        }
    }



    public void Next()
    {
        TutoGroup.SetActive(false);
    }
}
