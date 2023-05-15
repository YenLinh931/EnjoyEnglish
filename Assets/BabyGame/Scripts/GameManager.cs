using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Sprite mauDung, mauDefault;
    public Text dapAnLV3;
    public SoundManager soundManager;
    //prefabs
    public GameObject level1, level2, level3;
    public GameObject khoaLV2, khoaLV3;
    public Text sumCauHoiLV1, sumCauHoiLV2, sumCauHoiLV3;
    //canvas
    public GameObject canvasLoading, canvasTrangChu, canvasChooseLevel, canvasGamePlay, canvasPause, canvasLose, canvasWin, canvasThongBao, canvasDieuKhien, canvasCauHoiLV3, canvasGioiThieu;

    // so keo trong canvasTrangChu
    public Text countKeoTrangChu;

    //so keo, so sao canvasLose
    public Text countKeoCanvasLose, countSaoCanvasLose;

    //so keo, so sao canvasWin
    public Text countKeoCanvasWin, countSaoCanvasWin;

    public GameObject canvasCauHoi;
    public Text countKeoCanvasGamePlay, countCauHoiCanvasGamePlay;
    public GameObject tymGroup;
    private int countTym = 3;

    // level
    private int level = 1, levelPlaying = -1;

    //Cau hoi
    public GameObject[] listCauHois;
    public int[] listDapAn;
    public int limitRandom = 1, limitRandomLV3 = 1;
    public InputField textDapAnLV3;
    public GameObject truKeoLV3, errLV3;
    private int indexCauHoi = 0;
    private bool isClickDapAn = false;

    public GameObject[] listCauHoiLV3s;
    public string[] dapAnLV3s;
    
    private GameObject curLevel1, curLevel2, curLevel3;

    //Score gamePlay
    private int countKeoInLevel = 0;
    private int countCauHoiInLevel = 0;

    private bool isDich = false;


    // index thong bao: 0. đủ câu hỏi để hiển thị đích

    // setting
    public Sprite iconBat, iconTat;
    public Button soundTrangChu, soundPause;
    private bool isAmThanh = true;
    private bool setIconAmThanh = false;

    // update level
    private bool isLevel2 = false, isLevel3 = false;

    private int limitTime = 10;
    private float curTime = 0;
    private bool endTimeCauHoi = false;
    public Text time, timeLV3;
    public Image bgTime, bgTimeLV3;
    private bool isTLSai = false;
    private int countGT = 0;
    

    private bool isBatTu = false;
    void Start()
    {
        soundManager.PlayBG();
        StartCoroutine(DelayLoading());
        this.RegisterListener(EventID.isVaChamCNV, (param) => {
            if(countTym > 0 && !isBatTu) {
                countTym--;
                tymGroup.transform.GetChild(countTym).gameObject.SetActive(false);
                if(countTym == 0) {
                    this.PostEvent(EventID.isEndGame);
                    canvasLose.SetActive(true);
                    soundManager.PlayLose();
                    countKeoCanvasLose.text = countKeoInLevel.ToString();
                    countSaoCanvasLose.text = countCauHoiInLevel.ToString();
                }
            }
        });
        this.RegisterListener(EventID.isAnKeo, (param) => {
            countKeoInLevel++;
            soundManager.PlayAnKeo();
            countKeoCanvasGamePlay.text = countKeoInLevel.ToString();
        });
        this.RegisterListener(EventID.isAnCauHoi, (param) => {
            isClickDapAn = false;
            isTLSai = false;
            this.PostEvent(EventID.isPause);
            StartCoroutine(DelayCauHoi());
        });
        this.RegisterListener(EventID.isVeDich, (param) => {
            StartCoroutine(DelayWin());
        });
    }
    IEnumerator DelayWin() {
        yield return new WaitForSeconds(1);
        canvasWin.SetActive(true);
        if(levelPlaying == 1)
            level = 2;
        if(levelPlaying >= 2)
            level = 3;
        soundManager.PlayWin();
        countKeoCanvasWin.text = countKeoInLevel.ToString();
        countSaoCanvasWin.text = countCauHoiInLevel.ToString();
        UpdateSumCauHoi(countCauHoiInLevel);
    }
    IEnumerator DelayCauHoi()
    {
        yield return new WaitForSeconds(0.2f);
        soundManager.PlayRun();
        soundManager.PlayAnCauHoi();
        if(levelPlaying < 3) {
            bgTime.GetComponent<Image>().fillAmount = 0;
            limitTime = 10;
            canvasCauHoi.SetActive(true);
            if(levelPlaying == 1)
                indexCauHoi = UnityEngine.Random.Range(0, limitRandom);
            if(levelPlaying == 2)
                indexCauHoi = UnityEngine.Random.Range(limitRandom, limitRandom * 2);
            listCauHois[indexCauHoi].SetActive(true);
            time.text = limitTime.ToString();
        } else {
            canvasCauHoiLV3.SetActive(true);
            bgTimeLV3.GetComponent<Image>().fillAmount = 0;
            limitTime = 20;
            indexCauHoi = UnityEngine.Random.Range(0, limitRandomLV3);
            listCauHoiLV3s[indexCauHoi].SetActive(true);
            listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Animator>().SetBool("isGoiY", false);
            StartCoroutine(DelaySound());
            timeLV3.text = limitTime.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.U))
            level = 2;
        if(Input.GetKeyDown(KeyCode.I))
            level = 3;
        if(Input.GetKeyDown(KeyCode.Return) && canvasCauHoiLV3.activeInHierarchy) {
            OnClickDapAnLV3(textDapAnLV3);
        }
        if(Input.GetKeyDown(KeyCode.X)) 
            isBatTu = !isBatTu;
        if(Input.GetKeyDown(KeyCode.Return) && canvasTrangChu.activeInHierarchy)
            OnClickStartGame();
        if(setIconAmThanh) {
            setIconAmThanh = false;
            if(isAmThanh) {
                soundTrangChu.GetComponent<Image>().sprite = iconBat;
                soundPause.GetComponent<Image>().sprite = iconBat;
            } else {
                soundTrangChu.GetComponent<Image>().sprite = iconTat;
                soundPause.GetComponent<Image>().sprite = iconTat;
            }
        }
        if((canvasCauHoi.activeInHierarchy || canvasCauHoiLV3.activeInHierarchy) && limitTime > 0) {
            curTime += Time.deltaTime;
            bgTime.GetComponent<Image>().fillAmount += Time.deltaTime/10;
            bgTimeLV3.GetComponent<Image>().fillAmount += Time.deltaTime/20;
            if(curTime >= 1) {
                curTime = 0;
                limitTime--;
                time.text = limitTime.ToString();
                timeLV3.text = limitTime.ToString();
            }
            if(limitTime <= 0) {
                soundManager.StopRun();
                if(levelPlaying < 3)
                    OnClickDapAn(-1);
                else {
                    if(!isTLSai) {
                        soundManager.PlayTraLoiSai();
                        dapAnLV3.text = dapAnLV3s[indexCauHoi];
                        dapAnLV3.gameObject.SetActive(true);
                        truKeoLV3.GetComponent<Animator>().enabled = false;
                        errLV3.GetComponent<Animator>().enabled = false;
                        StartCoroutine(DelayAnCauHoiLV3());
                    }
                }
            }
        }

        if(level == 2 && !isLevel2) {
            isLevel2 = true;
            khoaLV2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            khoaLV2.transform.GetChild(1).gameObject.SetActive(false);
            sumCauHoiLV2.gameObject.SetActive(true);
        }
        if(level == 3 && !isLevel3) {
            isLevel3 = true;
            khoaLV3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            khoaLV3.transform.GetChild(1).gameObject.SetActive(false);
            sumCauHoiLV3.gameObject.SetActive(true);
        }
        if(!isDich && countCauHoiInLevel >= 2 + levelPlaying) {
            isDich = true;
            soundManager.PlayHienDich();
            canvasThongBao.SetActive(true);    
            canvasThongBao.transform.GetChild(0).gameObject.SetActive(true);
            this.PostEvent(EventID.isDich);
        }
    }
    IEnumerator DelaySound() {
        yield return new WaitForSeconds(1);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = true;

        yield return new WaitForSeconds(3);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = false;
        
        yield return new WaitForSeconds(1);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = true;

        yield return new WaitForSeconds(3);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = false;

        yield return new WaitForSeconds(1);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = true;

        yield return new WaitForSeconds(3);
        listCauHoiLV3s[indexCauHoi].transform.GetChild(1).GetComponent<AudioSource>().enabled = false;
    }
    IEnumerator DelayAnThongBao() {
        yield return new WaitForSeconds(5);
        canvasThongBao.transform.GetChild(0).gameObject.SetActive(false);
        canvasThongBao.SetActive(false);
    }
    public void OnClickDapAn(int indexDapAn) {
        if(isClickDapAn)    return;
        soundManager.StopRun();
        isClickDapAn = true;
        if(indexDapAn == listDapAn[indexCauHoi]) {
            listCauHois[indexCauHoi].SetActive(false);
            canvasCauHoi.SetActive(false);
            countCauHoiInLevel++;
            countCauHoiCanvasGamePlay.text = countCauHoiInLevel.ToString();
            soundManager.PlayTraLoiDung();
            this.PostEvent(EventID.isPause);
        } else {
            soundManager.PlayTraLoiSai();
            listCauHois[indexCauHoi].transform.GetChild(1).GetChild(listDapAn[indexCauHoi] - 1).GetComponent<Image>().sprite = mauDung;
            StartCoroutine(DelayAnCauHoi());
        }
    }
    
    public void OnClickDapAnLV3(InputField input) {
        if(isClickDapAn)    return;
        soundManager.StopRun();
        isClickDapAn = true;
        if(input.text.ToLower().Trim().CompareTo(dapAnLV3s[indexCauHoi].ToLower().Trim()) == 0) {
            listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Animator>().enabled = false;
            listCauHoiLV3s[indexCauHoi].SetActive(false);
            canvasCauHoiLV3.SetActive(false);
            countCauHoiInLevel++;
            countCauHoiCanvasGamePlay.text = countCauHoiInLevel.ToString();
            soundManager.PlayTraLoiDung();
            input.text = "";
            //listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Image>().enabled = true;
            listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Animator>().SetBool("isGoiY", false);
            this.PostEvent(EventID.isPause);
        } else {
            dapAnLV3.text = dapAnLV3s[indexCauHoi];
            dapAnLV3.gameObject.SetActive(true);
            soundManager.PlayTraLoiSai();
            isTLSai = true;
            StartCoroutine(DelayAnCauHoiLV3());
        }
        truKeoLV3.GetComponent<Animator>().enabled = false;
        errLV3.GetComponent<Animator>().SetBool("isErr", false);
    }
    IEnumerator DelayAnCauHoi() {
        yield return new WaitForSeconds(3);
        listCauHois[indexCauHoi].transform.GetChild(1).GetChild(listDapAn[indexCauHoi] - 1).GetComponent<Image>().sprite = mauDefault;
        listCauHois[indexCauHoi].SetActive(false);
        canvasCauHoi.SetActive(false);
        this.PostEvent(EventID.isVaChamCNV);
        this.PostEvent(EventID.isPause);
    }
    
    IEnumerator DelayAnCauHoiLV3() {
        yield return new WaitForSeconds(3);
        dapAnLV3.gameObject.SetActive(false);
        listCauHoiLV3s[indexCauHoi].SetActive(false);
        
        textDapAnLV3.text = "";
        //listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Image>().enabled = true;
        listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Animator>().SetBool("isGoiY", false);
        canvasCauHoiLV3.SetActive(false);
        this.PostEvent(EventID.isVaChamCNV);
        this.PostEvent(EventID.isPause);
    }
    public void OnClickGoiY() {
        soundManager.PlayClick();
        if(countKeoInLevel >= 10) {
            truKeoLV3.GetComponent<Animator>().enabled = true;
            countKeoInLevel -= 10;
            countKeoCanvasGamePlay.text = countKeoInLevel.ToString();
            //listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Image>().enabled = false;
            listCauHoiLV3s[indexCauHoi].transform.GetChild(0).GetComponent<Animator>().SetBool("isGoiY", true);
        } else {
            errLV3.GetComponent<Animator>().SetBool("isErr", true);
            StartCoroutine(DelayErr());
        }
    }
    IEnumerator DelayErr() {
        yield return new WaitForSeconds(2);
        if(canvasCauHoiLV3.activeInHierarchy)
            errLV3.GetComponent<Animator>().SetBool("isErr", false);
    }

    IEnumerator DelayLoading() {
        yield return new WaitForSeconds(2.5f);
        canvasLoading.SetActive(false);
        canvasTrangChu.SetActive(true);
    }

    public void OnClickStartGame() {
        soundManager.PlayClick();
        canvasTrangChu.SetActive(false);
        canvasChooseLevel.SetActive(true);
    }

    public void OnClickBackHome() {
        soundManager.PlayClick();
        canvasGioiThieu.transform.GetChild(countGT).gameObject.SetActive(false);
        canvasGioiThieu.transform.GetChild(0).gameObject.SetActive(true);
        canvasGioiThieu.transform.GetChild(7).gameObject.SetActive(true);
        countGT = 0;
        canvasGioiThieu.SetActive(false);
        canvasChooseLevel.SetActive(false);
        canvasTrangChu.SetActive(true);
    }
    public void OnClickGT() {
        soundManager.PlayClick();
        canvasGioiThieu.SetActive(true);
        canvasTrangChu.SetActive(false);
    }
    public void OnClickNextGT(){
        if(countGT < 6) {
            soundManager.PlayClick();
            countGT++;
            canvasGioiThieu.transform.GetChild(countGT - 1).gameObject.SetActive(false);
            canvasGioiThieu.transform.GetChild(countGT).gameObject.SetActive(true);
            if(countGT == 6)
                canvasGioiThieu.transform.GetChild(7).gameObject.SetActive(false);
        } else {
            canvasGioiThieu.transform.GetChild(countGT).gameObject.SetActive(false);
            canvasGioiThieu.transform.GetChild(0).gameObject.SetActive(true);
            canvasGioiThieu.transform.GetChild(7).gameObject.SetActive(true);
            OnClickBackHome();
        }
    }

    public void OnClickChooseLevel(int level) {
        soundManager.PlayClick();
        if(level <= this.level) {
            canvasChooseLevel.SetActive(false);
            canvasGamePlay.SetActive(true);
            // canvasDieuKhien.SetActive(true);
            levelPlaying = level;
            Replay(1, level);
        }
    }

    public void OnClickPauseGame() {
        soundManager.PlayClick();
        canvasPause.SetActive(true);
        this.PostEvent(EventID.isPause);
    }
    public void OnClickReplay(int type) {
        soundManager.PlayClick();
        //1: lose, 2: win
        if(type == 1) {
            canvasPause.SetActive(false);
            canvasLose.SetActive(false);
            Replay(1, levelPlaying);
        } else {
            canvasWin.SetActive(false);
            Replay(2, levelPlaying);
        }
    }
    public void OnContinue() {
        soundManager.PlayClick();
        canvasPause.SetActive(false);
        this.PostEvent(EventID.isPause);
    }
    public void OnClickGamePlayToHome() {
        soundManager.PlayClick();
        canvasPause.SetActive(false);
        canvasLose.SetActive(false);
        canvasWin.SetActive(false);
        canvasGamePlay.SetActive(false);
        canvasDieuKhien.SetActive(false);
        canvasTrangChu.SetActive(true);
        var countKeo = int.Parse(countKeoTrangChu.text);
        countKeoTrangChu.text = (countKeo + countKeoInLevel).ToString();
        DestroyLevel();
    }
    private void Replay(int typeReplay, int level) {
        //1: lose, 2: win
        if(typeReplay == 2) {
            var countKeo = int.Parse(countKeoTrangChu.text);
            countKeoTrangChu.text = (countKeo + countKeoInLevel).ToString();
        }
        countCauHoiInLevel = 0;
        countKeoInLevel = 0;
        countTym = 3;
        countCauHoiCanvasGamePlay.text = countCauHoiInLevel.ToString();
        countKeoCanvasGamePlay.text = countCauHoiInLevel.ToString();
        canvasThongBao.transform.GetChild(0).gameObject.SetActive(false);
        canvasThongBao.SetActive(false);
        isDich = false;
        for (int i = 0; i < tymGroup.transform.childCount; i++)
        {
            tymGroup.transform.GetChild(i).gameObject.SetActive(true);
        }
        if(level == 1) {
            DestroyLevel();
            curLevel1 = Instantiate(level1, Vector3.zero, Quaternion.identity);
        } else if(level == 2) {
            DestroyLevel();
            curLevel2 = Instantiate(level2, Vector3.zero, Quaternion.identity);
        } else if(level == 3) {
            DestroyLevel();
            curLevel3 = Instantiate(level3, Vector3.zero, Quaternion.identity);
        }
        this.PostEvent(EventID.isStartGame);
    }
    public void SetAmThanh() {
        if(isAmThanh) {
            soundManager.StopBG();
            isAmThanh = false;
        } else {
            soundManager.PlayBG();
            isAmThanh = true;
        }
        setIconAmThanh = true;
    }
    public void OnNextLevel() {
        canvasWin.SetActive(false);
        canvasChooseLevel.SetActive(true);
    }
    private void DestroyLevel() {
        if(curLevel1 != null)
            Destroy(curLevel1.gameObject);
        if(curLevel2 != null)
            Destroy(curLevel2.gameObject);
        if(curLevel3 != null)
            Destroy(curLevel3.gameObject);
    }

    private void UpdateSumCauHoi(int count) {
        if(levelPlaying == 1) {
            var curCount = int.Parse(sumCauHoiLV1.text);
            if(count > curCount) {
                sumCauHoiLV1.text = count.ToString();
            }
        } else if(levelPlaying == 2) {
            var curCount = int.Parse(sumCauHoiLV2.text);
            if(count > curCount) {
                sumCauHoiLV2.text = count.ToString();
            }
        } else if(levelPlaying == 3) {
            var curCount = int.Parse(sumCauHoiLV3.text);
            if(count > curCount) {
                sumCauHoiLV3.text = count.ToString();
            }
        }
    }
}
