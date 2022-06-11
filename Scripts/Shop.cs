using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObject;
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText;
    public GameObject[] shopGroup;

    public AudioSource buy;

    Player enterPlayer;

    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
        shopGroup[0].SetActive(true);
        shopGroup[1].SetActive(true);
    }

    // Update is called once per frame
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
        shopGroup[0].SetActive(false);
        shopGroup[1].SetActive(false);
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        buy.Play();

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        Instantiate(itemObject[index], itemPos[index].position + ranVec, itemPos[index].rotation);

    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
