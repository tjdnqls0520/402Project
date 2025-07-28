using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class pdata
{
    public string id;
    public string name;
    public int hp;
    public int atk;
    public int def;
    public float move_speed;
    public float attack_speed;
    public float gathering_speed;
    public int stm;
    public int defult_weight;
    public int limit_bag;
    public List<string> active_skills;
    public List<string> passive_skills;
    public int recommended;
    public string intro_story;
    public string base_weapon;
    public List<string> default_activeslot;
    public List<string> default_passiveslot;
    public List<string> default_male;
    
}

[System.Serializable]
public class pdataListWrapper
{
    public List<pdata> list;
}

public class player : MonoBehaviour
{
    public string cid;
    public float movespeed = 0;
    public string Name;
    public int hp = 0;
    public int atk = 0;
    public int def = 0;
    public float attackspeed = 0;
    public float gatheringspeed = 0;
    public int stm = 0;
    public int defultweight = 0;
    public int limitbag = 0;
    public string stat;
    Rigidbody2D rb;
    Animator ani;
    public GameObject pl;
    public GameObject reward;
    public GameObject inventory;
    int dir = 1;
    public float jumpforce = 0;
    bool jumpc = true;
    private float moveInput;
    public float climbSpeed = 3f;

    private bool isOnLadder = false;
    private bool isClimbing = false;

    void Start()
    {
        stat = Path.Combine(Application.streamingAssetsPath, "json/cstat.json");
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        if(isOnLadder)
        {
            float vertical = Input.GetAxis("Vertical");

            if(Mathf.Abs(vertical) > 0)
            {
                isClimbing = true;

                rb.gravityScale = 0f;

                rb.linearVelocity = new Vector2(0f, vertical * climbSpeed);
            }
            else if (isClimbing)
            {
                rb.linearVelocity = new Vector2(0f, 0f);
            }
        }
        else
        {
            if (isClimbing)
            {
                // 사다리에서 내려왔을 때 원래대로 복구
                rb.gravityScale = 2f;
                isClimbing = false;
            }
        }



        //이동
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {
            dir = -1;
            ani.SetBool("run", true);
        }
        else if (moveInput < 0)
        {
            dir = 1;
            ani.SetBool("run", true);
        }
        else
        {
            ani.SetBool("run", false);
        }

        transform.localScale = new Vector3(dir, 1, 1);

        if (Input.GetKeyDown(KeyCode.Space) && jumpc && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpforce);
            ani.SetBool("jump", true);
            jumpc = false;
        }


        //점프
        if (Input.GetKey(KeyCode.Space) && jumpc == true && isClimbing == false)
        {
            rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            ani.SetBool("jump", true);
            jumpc = false;
        }

    }
    pdata load(string id) // json 파싱
    {
        try
        {
            if (File.Exists(stat))
            {
                string json = File.ReadAllText(stat);
                Debug.Log(json);
                pdataListWrapper wrapper = JsonUtility.FromJson<pdataListWrapper>(json);

                if (wrapper == null)
                {
                    Debug.LogError("pdataListWrapper 파싱 실패: wrapper가 null입니다.");
                    return new pdata();
                }

                if (wrapper.list == null)
                {
                    Debug.LogError("pdataListWrapper.list가 null입니다. JSON 구조를 확인하세요.");
                    return new pdata();
                }

                pdata selected = wrapper.list.Find(p => p.id == id);

                if (selected == null)
                {
                    Debug.LogError($"id '{id}'에 해당하는 pdata를 찾을 수 없습니다.");
                    return new pdata();
                }

                Debug.Log("pdata 정상 로드 완료");
                return selected;
            }
            else
            {
                Debug.LogWarning("pdata.json 파일이 없습니다.");
                return new pdata();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("pdata 로딩 중 예외 발생: " + ex.Message);
            return new pdata(); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            jumpc = true;
            ani.SetBool("jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }

        if (collision.CompareTag("LottenCorpse"))
        {
            Debug.Log("상자 있음");
        }

        if (collision.CompareTag("LottenCorpse") && Input.GetKeyDown(KeyCode.F))
        {
            reward.SetActive(true);
            inventory.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("LottenCorpse") && Input.GetKeyDown(KeyCode.F))
        {
            reward.SetActive(true);
            inventory.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }

        if (collision.CompareTag("LottenCorpse"))
        {
            reward.SetActive(false);
            inventory.SetActive(false);
        }
    }

    


    public void worrior()
    {
        cid = "a1";
        pdata selected = load(cid);
        movespeed = selected.move_speed;
        Name = selected.name;
        hp = selected.hp;
        atk = selected.atk;
        def = selected.def;
        attackspeed = selected.attack_speed;
        gatheringspeed = selected.gathering_speed;
        stm = selected.stm;
        defultweight = selected.defult_weight;
        limitbag = selected.limit_bag;
    }

    public void spear()
    {
        cid = "a2";
        pdata selected = load(cid);
        movespeed = selected.move_speed;
        Name = selected.name;
        hp = selected.hp;
        atk = selected.atk;
        def = selected.def;
        attackspeed = selected.attack_speed;
        gatheringspeed = selected.gathering_speed;
        stm = selected.stm;
        defultweight = selected.defult_weight;
        limitbag = selected.limit_bag;
    }

    public void thief()
    {
        cid = "a3";
        pdata selected = load(cid);
        movespeed = selected.move_speed;
        Name = selected.name;
        hp = selected.hp;
        atk = selected.atk;
        def = selected.def;
        attackspeed = selected.attack_speed;
        gatheringspeed = selected.gathering_speed;
        stm = selected.stm;
        defultweight = selected.defult_weight;
        limitbag = selected.limit_bag;
    }

    public void cat()
    {
        cid = "a4";
        pdata selected = load(cid);
        movespeed = selected.move_speed;
        Name = selected.name;
        hp = selected.hp;
        atk = selected.atk;
        def = selected.def;
        attackspeed = selected.attack_speed;
        gatheringspeed = selected.gathering_speed;
        stm = selected.stm;
        defultweight = selected.defult_weight;
        limitbag = selected.limit_bag;
    }


}
