using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C,D };
    public Type enemyType;

    public int maxHealth;
    public int curHealth;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    public GameObject[] pickUpItem;
    public GameObject[] powerPanel;
    public GameObject qnABook;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    public AudioSource hit;

    int ranPos = 0;
    int ranDrop = 0;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if (enemyType != Type.D)
        {
            Invoke("ChaseStart", 2);
        }

    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    
    // Update is called once per frame
    void Update()
    {
        //추격할떄만 추격
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;

            Quiz quiz = manager.GetComponent<Quiz>();
            if (quiz.isQuiz)
            {
                nav.isStopped = true;
            }


        }

    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

    }

    void Targerting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;//판정범위
            float targetRange = 0;//인식거리

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;

                    break;
                case Type.B:
                    targetRadius = 1.5f;
                    targetRange = 12f;

                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;

                    break;
            }

            RaycastHit[] rayHit = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            // 범위안 공격
            if (rayHit.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.5f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }


        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {//물리문제해결
        Targerting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            Debug.Log("melee " + curHealth);
            StartCoroutine(OnDamage(reactVec,false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            Debug.Log("range " + curHealth);
            StartCoroutine(OnDamage(reactVec,false));
        }
    }
    //수류탄 피해량
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;

        StartCoroutine(OnDamage(reactVec,true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        

        if (curHealth > 0)
        {
            //hit.Play();

            yield return new WaitForSeconds(0.1f);

            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 12;
            isDead = true;
            isChase = false;
            nav.enabled = false;

            anim.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.score += score;

            ranPos = Random.Range(-3, 4);
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], new Vector3(transform.position.x + ranPos, transform.position.y, transform.position.z + ranPos), Quaternion.identity);

            //드랍테이블
            ranDrop = Random.Range(0, 101);
            if (ranDrop > 50)
            {
                ranPos = Random.Range(-3, 4);
                Instantiate(qnABook, new Vector3(transform.position.x + ranPos, transform.position.y, transform.position.z + ranPos), Quaternion.identity);
            }
            ranDrop = Random.Range(0, 101);
            if (ranDrop > 95)
            {
                int pick = Random.Range(0, 3);
                ranPos = Random.Range(-3, 4);
                Instantiate(pickUpItem[pick], new Vector3(transform.position.x + ranPos, transform.position.y, transform.position.z + ranPos), Quaternion.identity);
            }
            ranDrop = Random.Range(0, 101);
            if (ranDrop > 80)
            {
                int ranPanel = Random.Range(0, 5);
                ranPos = Random.Range(-3, 4);
                Instantiate(powerPanel[ranPanel], new Vector3(transform.position.x + ranPos, transform.position.y+1, transform.position.z + ranPos), Quaternion.identity);
            }

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCountA--;
                    break;
                case Type.B:
                    manager.enemyCountB--;
                    break;
                case Type.C:
                    manager.enemyCountC--;
                    break;
                case Type.D:
                    manager.enemyCountD--;
                    break;
            }

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up/2;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

                Destroy(gameObject, 4);
            

        }
    }
}
