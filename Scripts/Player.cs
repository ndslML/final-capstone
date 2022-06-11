using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{//속도와 무기
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;

    public Camera followCamera;
    public GameManager manager;

    public AudioSource jumpSound;

    //파워업
    public Weapon[] weapon;
    public Bullet[] bullet;

    //각종 변수
    public int ammo;
    public int coin;
    public int health;
    public int score;
    //변수최대값
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    //이동과 버튼
    float hAxis;
    float vAxis;
    Vector3 moveVec;
    Vector3 dodgeVec;
    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    //무기교체
    bool sDown1;
    bool sDown2;
    bool sDown3;
    //상태확인
    public float jumpPower;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady;
    bool isReload;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;

    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    //연사속도
    float fireDelay;

    public AudioSource talk;
    public AudioSource getItem;
    public AudioSource quizItem;

    public AudioSource reload;
    public AudioSource swing;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        PlayerPrefs.SetInt("MaxScore", 100000);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Quiz quiz = manager.GetComponent<Quiz>();
        if (!quiz.isQuiz)
        {
            Move();

            Turn();

            Jump();

            Grenade();

            Attack();

            Reload();

            Dodge();

            Swap();

            Interation();
        }



    }
    //버튼받기
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    //움직이기
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isSwap || isDead)
        {
            moveVec = Vector3.zero;
        }
        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    //방향으로 돌기
    void Turn()
    {
        //키보드
        transform.LookAt(transform.position + moveVec);
        //마우스 지점이 플로어와 맞닿은 지점과 현재지점을 빼서 벡터를 구한다.
        if (fDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }
    //점프
    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            jumpSound.Play();
        }
    }
    //수류탄 던지기
    void Grenade()
    {
        if (hasGrenades == 0)
        {
            return;
        }
        if (gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 12;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                swing.Play();
                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    //공격
    void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    //재장전
    void Reload()
    {
        if (equipWeapon == null)
        {
            return;
        }
        if (equipWeapon.type == Weapon.Type.Melee)
        {
            return;
        }
        if (ammo == 0)
        {
            return;
        }
        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead && !isReload)
        {
            reload.Play();
            anim.SetTrigger("doReload");
            isReload = true;
            //재장전시간
            Invoke("ReloadOut", 3f);
        }

    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    //회피
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    //교체
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
        {
            return;
        }
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
        {
            return;
        }
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
        {
            return;
        }

        int weaponIndex = -1;

        if (sDown1)
        {
            weaponIndex = 0;
        }
        if (sDown2)
        {
            weaponIndex = 1;
        }
        if (sDown3)
        {
            weaponIndex = 2;
        }

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead && !isReload)
        {
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }
    //상호작용
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;

                hasWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                talk.Play();
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
            else if (nearObject.tag == "Book")
            {
                quizItem.Play();
                Quiz quiz = manager.GetComponent<Quiz>();
                quiz.isQuiz = true;
                quiz.isAnswer = true;
                Destroy(nearObject);
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    private void FixedUpdate()
    {//물리문제해결
        FreezeRotation();
        StopToWall();
    }
    //점프 확인
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
    //먹기
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                    {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                    {
                        coin = maxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                    {
                        health = maxHealth;
                    }
                    break;
                case Item.Type.Grenade:
                    if (hasGrenades == maxHasGrenades)
                    {
                        return;
                    }
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                    {
                        hasGrenades = maxHasGrenades;
                    }
                    break;
                case Item.Type.MaxHeart:
                    maxHealth += 10;
                    health += 10;
                    break;
                case Item.Type.MaxAmmo:
                    weapon[1].maxAmmo += 3;
                    weapon[2].maxAmmo += 10;
                    break;
                case Item.Type.MaxRapid:
                    weapon[1].rate *= 0.9f;
                    weapon[2].rate *= 0.9f;
                    break;
                case Item.Type.MaxSpeed:
                    speed += 2;
                    break;
                case Item.Type.MaxAttack:
                    weapon[0].damage += 7;
                    bullet[0].damage += 2;
                    bullet[1].damage += 1;
                    break;
            }
            getItem.Play();
            Destroy(other.gameObject);
        }
        //탄환피해를 받음
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }

        }

    }
    //피해를 받음
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        //플레이어의 모든 재질 색 변경
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAtk)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }
        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if (isBossAtk)
        {
            rigid.velocity = Vector3.zero;
        }



    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    //가까이 무장 확인
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop" || other.tag == "Book")
        {
            nearObject = other.gameObject;
        }
        //Debug.Log(nearObject.name);
    }
    //무장확인 끝
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Book")
        {
            nearObject = null;
        }
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
