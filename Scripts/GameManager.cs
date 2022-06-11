using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCountA;
    public int enemyCountB;
    public int enemyCountC;
    public int enemyCountD;

    public Transform[] enemyZone;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Image weapon1Image;
    public Image weapon2Image;
    public Image weapon3Image;
    public Image weaponRImage;
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHeanthGroup;
    public RectTransform bossHeanthBar;
    public Text curScoreText;
    public Text bestScoreText;

    public GameObject lobby;
    public GameObject tutorial;
    public GameObject[] stage1;
    public GameObject quizShow;
    public GameObject gameUI;

    public GameObject shopGroup;

    public GameObject[] storyObj;

    public GameObject quizTrigger;

    public AudioSource startButton;

    int enemyNum;

    // Start is called before the first frame update
    void Awake()
    {
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        startButton.Play();

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        overPanel.SetActive(true);
        gamePanel.SetActive(false);

        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }

    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void GoTuto()//게임 시작
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        lobby.SetActive(false);

        tutorial.SetActive(true);
        player.ammo = 999;
    }

    public void GoQuiz()
    {
        player.transform.position = Vector3.up * 0.8f;
        player.gameObject.SetActive(false);

        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        lobby.SetActive(false);
        gameUI.SetActive(false);

        quizShow.SetActive(true);
        Instantiate(storyObj[5], player.transform.position, player.transform.rotation);
        quizTrigger.SetActive(true);

    }

    public void BackLobby()
    {
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        lobby.SetActive(true);
        gameUI.SetActive(true);

        quizShow.SetActive(false);
        tutorial.SetActive(false);
        player.ammo = 300;

        player.transform.position = Vector3.up * 0.8f;
        player.gameObject.SetActive(true);
    }

    public void StageStart()//게임 시작
    {
        if (stage == 0)
            player.transform.position = Vector3.up * 0.8f;
        if (stage < 3)
        {
            itemShop.SetActive(false);
            weaponShop.SetActive(false);
        }
        startZone.SetActive(false);
        lobby.SetActive(false);
        if (stage == 0)
            stage1[stage].SetActive(true);

        foreach (Transform zone in enemyZone)
        {
            zone.gameObject.SetActive(true);
        }
        if (stage == 0)
        {
            enemyNum = 2;
        }
        else if (stage == 1)
        {
            enemyNum = 25;
        }
        else if (stage == 2)
        {
            enemyNum = 40;
        }
        else if (stage == 4)
        {
            enemyNum = 99999;
        }

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        if (stage >= 1)
        {
            player.transform.position = Vector3.up * 0.8f;
        }


        if (stage >= 1)
        {
            itemShop.SetActive(true);
            weaponShop.SetActive(true);
        }

        if (stage >= 1)
        {
            stage1[stage - 1].SetActive(false);
        }
        stage1[stage].SetActive(true);

        Instantiate(storyObj[stage], player.transform.position, player.transform.rotation);

        startZone.SetActive(true);

        foreach (Transform zone in enemyZone)
        {
            zone.gameObject.SetActive(false);
        }

        enemyCountA = 0;
        enemyCountB = 0;
        enemyCountC = 0;

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage == 3)
        {
            enemyCountD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZone[2].position, enemyZone[2].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for(int index = 0; index < enemyNum; index++)//스테이지 수 만큼 적수 추가
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCountA++;
                        break;
                    case 1:
                        enemyCountB++;
                        break;
                    case 2:
                        enemyCountC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone= Random.Range(0, 3);//리스폰 지점
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZone[ranZone].position, enemyZone[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);

                yield return new WaitForSeconds(4);
            }
        }

        while (enemyCountA + enemyCountB + enemyCountC + enemyCountD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4);
        boss = null;
        StageEnd();
    }

    private void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        playerHealthText.text = player.health + " / " + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", player.coin);

        if (player.equipWeapon == null)
        {
            playerAmmoText.text = "- / " + player.ammo;
        }
        else if (player.equipWeapon.type == Weapon.Type.Melee)
        {
            playerAmmoText.text = "- / " + player.ammo;
        }
        else
        {
            playerAmmoText.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }

        weapon1Image.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Image.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Image.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImage.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        if (stage != 4)
        {

            enemyAText.text = enemyCountA.ToString();
            enemyBText.text = enemyCountB.ToString();
            enemyCText.text = enemyCountC.ToString();
        }
        else if (stage == 4)
        {

            enemyAText.text = "999";
            enemyBText.text = "999";
            enemyCText.text = "999";
        }

        if (boss != null)
        {
            bossHeanthGroup.anchoredPosition = Vector3.down * 30;

            bossHeanthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHeanthGroup.anchoredPosition = Vector3.up * 200;
        }

    }
}
