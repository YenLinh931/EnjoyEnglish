using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int level = 1;
    public GameObject dich;
    public int speed = 1;
    public int doCao = 0;
    private Rigidbody2D rigid;
    private Animator anim;
    private int diChuyen = 0;
    private float oldScale = 0;
    private bool isRun = false;
    private bool isJump = false;

    private bool isStart = true;

    private bool isPause = false;

    private bool isDongBang = false;

    private float curTime = 0;

    private bool isButtonMoveLeft = false, isButtonMoveRight = false, isButtonJump = false;
    private float minX = 0, maxX = 0;
    private bool activeDB = false;
    void Start()
    {
        if(level == 1)
        {
            minX = -9;
            maxX = 24;
        }
        if(level == 2)
        {
            minX = -11.5f;
            maxX = 30.5f;
        }
        rigid = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        this.RegisterListener(EventID.isEndGame, (param) => {
            isStart = false;
        });
        this.RegisterListener(EventID.isPause, (param) => {
            isPause = !isPause;
        });
        this.RegisterListener(EventID.isStartGame, (param) => {
            isStart = true;
        });
        this.RegisterListener(EventID.isDongBang, (param) => {
            isDongBang = true;
        });
        this.RegisterListener(EventID.isDich, (param) => {
            if(dich.gameObject != null) {
                dich.GetComponent<BoxCollider2D>().enabled = true;
                dich.GetComponent<SpriteRenderer>().enabled = true;
                dich.transform.GetChild(0).gameObject.SetActive(true);
                dich.transform.GetChild(1).gameObject.SetActive(true);
                StartCoroutine(DelayEndCAm());
            }
        });
        this.RegisterListener(EventID.isMove, (param) => {
            if((int) param == 1) {
                isButtonMoveRight = false;
                isButtonMoveLeft = true;
            } else if((int) param == 2) {
                isButtonMoveLeft = false;
                isButtonMoveRight = true;
            } else {
                isButtonJump = true;
            }
        });
        this.RegisterListener(EventID.isNotMove, (param) => {
            isButtonMoveLeft = false;
            isButtonMoveRight = false;
        });
        oldScale = transform.localScale.x;
    }

    IEnumerator DelayEndCAm()
    {
        yield return new WaitForSeconds(2); 
        dich.transform.GetChild(1).gameObject.SetActive(false);
    }

    // lv1 -9 < x < 24
    // lv2 3 -11.5 < x < -30.5
    // Update is called once per frame
    void Update()
    {
        if(isStart && !isPause && !isDongBang) {
            if(isButtonMoveLeft || Input.GetKey(KeyCode.LeftArrow) && transform.position.x > minX) {
                if(!isRun) {
                    isRun = true;
                    ChangeAnim(2);
                }
                diChuyen = -1;
                transform.localScale = new Vector3(-oldScale, transform.localScale.y, transform.localScale.z);
            } 
            else if(isButtonMoveRight || Input.GetKey(KeyCode.RightArrow) && transform.position.x < maxX) {
                if(!isRun) {
                    isRun = true;
                    ChangeAnim(2);
                }
                diChuyen = 1;
                transform.localScale = new Vector3(oldScale, transform.localScale.y, transform.localScale.z);
            } 
            else {
                isRun = false;
                ChangeAnim(1);
                diChuyen = 0;
            }
            transform.Translate(Vector2.right * diChuyen * speed * Time.deltaTime);
            
            if((isButtonJump || Input.GetKeyDown(KeyCode.UpArrow)) && !isJump) {
                ChangeAnim(3);
                isJump = true;
                StartCoroutine(DelayJump());
                rigid.AddForce(Vector2.up * doCao, ForceMode2D.Impulse);
            } 
        }
        if(isDongBang) {
            if(!activeDB) {
                activeDB = true;
                transform.GetChild(1).gameObject.SetActive(true);
            }
            ChangeAnim(1);
            curTime += Time.deltaTime;
            if(curTime >= 3) {
                isDongBang = false;
                transform.GetChild(1).gameObject.SetActive(false);
                activeDB = false;
                curTime = 0;
            }
        }
    }
    IEnumerator DelayJump() {
        yield return new WaitForSeconds(1);
        isJump = false;
        isRun = false;
        isButtonJump = false;
    }
    IEnumerator DelayDongBang() {
        yield return new WaitForSeconds(3);
        isDongBang = false;
    }
    private void ChangeAnim(int typeAnim) {
        // 1: idle, 2: run, 3: jump
        anim.SetBool("isIdle", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        if(typeAnim == 1)
            anim.SetBool("isIdle", true);
        if(typeAnim == 2)
            anim.SetBool("isRun", true);
        if(typeAnim == 3)
            anim.SetBool("isJump", true);
    }

    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("dich")) {
            obj.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            isPause = true;
            this.PostEvent(EventID.isVeDich);
        }
    }
}
