using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    int quest;

    public string[] quizData;
    public string[] answerData;
    public Text correctText;
    public Text quizText;
    public Text[] AnswerText;
    public string[] correctAnswer1Text;
    public string[] answer2Text;
    public string[] answer3Text;
    public Button[] answerButton;

    public GameManager manager;
    public GameObject Yes;
    public Text correct;
    public Text wrong;
    public GameObject quizNext;
    public int corNum;
    public int worNum;

    public GameObject answerGroup;
    public GameObject nextGroup;

    public RectTransform[] answer;

    public GameObject quizGroup;

    public bool isQuiz;
    public bool isAnswer;

    public GameObject[] pickUpItem;
    public GameObject[] powerPanel;
    public Player player;

    public int quizType;

    public GameObject quizTrigger;
    public GameObject tuto;



    public AudioSource corAudio;
    public AudioSource worAudio;

    private void LateUpdate()
    {

        //Debug.Log(quizData.Length);
        if (isQuiz)//퀴즈중
        {
            quizGroup.SetActive(true);
        }
        else
        {
            quizGroup.SetActive(false);
        }
        if (isAnswer)//대답함
        {
            QuizSelect();
            answerGroup.SetActive(true);
            nextGroup.SetActive(false);
            quizNext.SetActive(false);
            isAnswer = false;
        }
    }
    public void QuizSelect()//퀴즈 고르기
    {
        quest = Random.Range(0,quizData.Length);

        quizText.text = quizData[quest];
        AnswerText[0].text = correctAnswer1Text[quest];
        AnswerText[1].text = answer2Text[quest];
        AnswerText[2].text = answer3Text[quest];

        int i = Random.Range(0, 3);

        int j = Random.Range(0, 3);
        while (j == i)
        {
            j = Random.Range(0, 3);
        }

        int k = Random.Range(0, 3);
        while (k == i || k == j)
        {
            k = Random.Range(0, 3);
        }


        answer[i].anchoredPosition = new Vector2(-500, -300);
        answer[j].anchoredPosition = new Vector2(0, -300);
        answer[k].anchoredPosition = new Vector2(500, -300);
    }

    public void CorrectAnswerQuiz()//대답할때 실행할 버튼함수
    {
        quizText.text = answerData[quest];
        correctText.text = "정답";
        corAudio.Play();

        if (quizType == 0)
        {
            int ranDrop = Random.Range(1, 101);
            if (ranDrop > 0)
            {
                int ranPos = Random.Range(0, 5);
                Instantiate(powerPanel[ranPos], new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 7), Quaternion.identity);
            }
        }
        else if (quizType == 1)
        {
            corNum++;
            correct.text = "정답 : " + corNum;
            quizNext.SetActive(true);
        }
        

        answerGroup.SetActive(false);
        nextGroup.SetActive(true);
    }
    public void WrongAnswerQuiz()//대답할때 실행할 버튼함수
    {
        quizText.text = answerData[quest];
        correctText.text = "오답";
        worAudio.Play();

        if (quizType == 0)
        {
            int ranDrop = Random.Range(0, 101);
            if (ranDrop > 50)
            {
                int ranPos = Random.Range(0, 3);
                Instantiate(pickUpItem[ranPos], new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 7), Quaternion.identity);
            }
        }
        else if (quizType == 1)
        {
            worNum++;
            wrong.text = "오답 : " + worNum;
            quizNext.SetActive(true);
        }


        answerGroup.SetActive(false);
        nextGroup.SetActive(true);
    }
    public void Next()
    {
        isQuiz = false;
    }
    public void Next2()
    {
        isAnswer = true;
    }

    public void QuizStart()
    {
        tuto.SetActive(false);
        quizTrigger.SetActive(false);
        Yes.SetActive(true);
        isQuiz = true;
        isAnswer = true;
        corNum = 0;
        worNum = 0;
        quizType = 1;
        
    }
    public void Exit()
    {
        quizNext.SetActive(false);
        isQuiz = false;
        quizType = 0;
        Yes.SetActive(false);

        manager.BackLobby();
    }
}
