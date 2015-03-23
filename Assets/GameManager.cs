using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{


    //public static bool isOnStep;
    public enum status
    {
        idle,
        jumpLeft,
        jumpRight,
    }

    int getCoinCount;


    public GameObject ReStartWindow;
    public GameObject player;
    public UISlider lifeGage;
    public UISlider feverGage;
    public UILabel stepView;
    public bool baseTimeLife;
    public float jumpSpeed = 20;
    float tempJumpSpeed;
    status pStatus;
  
    Vector3[] scaleV;
    GameObject[] playerPrefab;
    GameObject stepPrefab;
    GameObject[] buildingPrefab;
    GameObject coinPrefab;

    int playerNum;
    bool isPlayerDrop;
    float chFirstPosX;

    Vector3 camPos;
    Vector3 plPos;
    Vector3 nowPos;
    Vector3 newPos;
    Vector3 oriCamPos;

    int jumpCount;
    int addVal;
    int frameCount;

    bool isDaed;
    bool isPlayerDead;

    float camPosX;
    float camPosY;
    float playerPosY;
    float oriJumpSpeed;
    float collapseInter;
    float StepMakePosX;
    float StepMakePosY;
    float spendTime;

    public Transform instanceStepsParent;
    Transform GO;
    int stepChCount;
    
    public Transform stairs;
    Transform stairChild;

    int stairChCount;

    public Transform instanceBuildingParent;
    public Transform buildings;

    public float jumpX;
    public float jumpY;

    Transform nextStepT;
    GameObject objInstance;

    public GameObject[] moveBtn;

    bool isFever;

    int centerLR;

    float jumpClickInter;
    float jumpClick1;
    float jumpClick2;

    public GameObject feverLabel;
    public float feverTime = 10;
    float feverTimeOri;

    public GameObject leftText;
    public GameObject rightText;
    public GameObject touchText;
    bool touchFirst = false;

    public AudioSource bgSoundManager;
    public AudioClip normalSound;
    public AudioClip feverSound;

    public Transform instanceCoinParent;
    public Transform coins;
    int coinMakerRandom;
    public int coinMakerPer;
    public UILabel goldCountLabel;

    public AudioClip getCoin;
    public AudioClip chSound;

    public Transform feverFxPlane;

    void Awake()
    {
        Application.targetFrameRate = 60;
        feverTimeOri = feverTime;
        int TempI = 0;
        GameObject[] PrefabTemp;

        oriCamPos = Camera.main.transform.localPosition;
        ReStartWindow.SetActive(false);
        oriJumpSpeed = jumpSpeed;

        //계단 프리팹 로드//
        stepPrefab = Resources.Load("StepBox00") as GameObject;
        //계단 인스턴스 풀 생성//
        ObjPools(stepPrefab, "Step", 30, instanceStepsParent);

        //코인 프리팹 로드//
        coinPrefab = Resources.Load("Coin00") as GameObject;
        //코인 인스턴스 풀 생성//
        ObjPools(coinPrefab, "Step", 10, instanceCoinParent);

        //빌딩 프리팹 로드//
        buildingPrefab = new GameObject[10];
        TempI = 0;
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("BuildingAA" + i.ToString("D3"));
            if (Resources.Load("BG/BuildingAA" + i.ToString("D3")) == null) { Debug.Log("I값 : " + i); TempI = i; break; }
            buildingPrefab[i] = Resources.Load("BG/BuildingAA" + i.ToString("D3")) as GameObject;
        }
        PrefabTemp = buildingPrefab;
        buildingPrefab = new GameObject[TempI];
        for (int j = 0; j < TempI; j++)
        {
            buildingPrefab[j] = PrefabTemp[j];
        }
        //빌딩 인스턴스 풀 생성//
        for (int i = 0; i < buildingPrefab.Length; i++)
        {
            ObjPools(buildingPrefab[i], "BuildingAA", 30, instanceBuildingParent);
        }
        
        
        
        //캐릭터 프리팹 로드//
        playerPrefab = new GameObject[10];
        TempI = 0;
        for (int i = 0; i < 10; i++)
        {
            if (Resources.Load("Characters/Character" + i.ToString("D3")) == null) { Debug.Log("I값 : " + i); TempI = i; break; }
            playerPrefab[i] = Resources.Load("Characters/Character" + i.ToString("D3")) as GameObject;
        }
        PrefabTemp = playerPrefab;
        playerPrefab = new GameObject[TempI];
        for (int j = 0; j < TempI; j++)
        {
            playerPrefab[j] = PrefabTemp[j];
        }

        isDaed = true;
        GetComponent<PopupController>().AddPopWin(ReStartWindow);
        //Activate();

        leftText.SetActive(false);
        rightText.SetActive(false);
        touchText.SetActive(false);

        goldCountLabel.text = "0";
    }

    void ObjPools(GameObject obj, string newName, int count, Transform parentT)
    {
        for (int i = 0; i < count; i++)
        {
            objInstance = Instantiate(obj) as GameObject;
            objInstance.transform.parent = parentT;
            objInstance.name = newName + i.ToString("D5");
            objInstance.SetActive(false);
        }
    }

    void Activate()
    {
        #region//기본값 세팅//
        isPlayerDrop = false;
        frameCount = 0;
        jumpCount = 0;
        isDaed = false;
        isPlayerDead = false;

        spendTime = 0;

        StepMakePosX = 0;
        StepMakePosY = 0;

        jumpClickInter = 0;
        jumpClick1 =Time.time;
        jumpClick2 = 0;

        touchFirst = false;
        //최초상태 세팅//
        pStatus = status.idle;
        lifeGage.value = 1f;
        feverGage.value = 0f;
        stepView.text = "0";
        isFever = false;
        feverTime = 1 / feverTimeOri;

        //카메라 최초 위치
        Camera.main.transform.localPosition = oriCamPos;

        //goldCountLabel.text = "0";
        #endregion//기본값 세팅//

        jumpSpeed = oriJumpSpeed / 100;
        collapseInter = jumpSpeed / 50;

        //계단 인스턴스 화면 배치//
        StepMaker();

        //빌딩 인스턴스 화면 배치//
        BuildingMaker();


        //캐릭터 프리팹 화면에 배치//
        playerNum = Random.Range(0, playerPrefab.Length);
        GameObject selChar = playerPrefab[playerNum];
        
        player = Instantiate(selChar) as GameObject;

        player.transform.parent = GameObject.Find("GameRoot").transform;
        player.transform.localPosition = new Vector3(chFirstPosX, 0, 0);
        player.name = "Player";

        leftText.SetActive(true);
        rightText.SetActive(true);
        leftText.GetComponent<UILabel>().color = new Color(1, 1, 1, 1);
        rightText.GetComponent<UILabel>().color = new Color(1, 1, 1, 1);
        touchText.SetActive(false);
       
        bgSoundManager.clip = normalSound;
        bgSoundManager.Play();
    }

    IEnumerator StopFeverFx()
    {
        yield return new WaitForSeconds(10f);
        feverFxPlane.GetComponent<FeverFxPlaneScript>().Activate();
        feverFxPlane.GetComponent<FeverFxPlaneScript>().enabled = false;
    }

    void Update()
    {
        if (isDaed == true) return;

        camPos = Camera.main.transform.localPosition;
        plPos = player.transform.localPosition;

        camPosY = camPos.y;
        playerPosY = plPos.y;
        if (camPosY > (playerPosY + 3.8f)) isPlayerDrop = true;

        #region //계단을 밟지 못했을 경우// 라이프 게이지가 다 닳아 없어졌을때//화면 밖으로 나갔을 때//
        if (isPlayerDrop == true)
        {
            player.transform.localPosition += new Vector3(0, -0.1f, 0);
            if (isPlayerDead == false)
            {
                isPlayerDead = true;
                StartCoroutine(PlayerDead());
            }
            return;
        }
        #endregion //계단을 밟지 못했을 경우//

        #region //키보드 인풋//
        if (Input.GetKeyDown(KeyCode.LeftArrow)) JumpLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) JumpRight();
        #endregion //키보드 인풋//

        #region //카메라 위치 잡아줌//
        //캠 Y위치 교정//
        camPosY = camPos.y;
        if (camPosY - 1 <= plPos.y)
        {
            camPosY = Mathf.Lerp(camPos.y, plPos.y + 1, 0.1f);
            if (jumpCount > 0 && isFever == false && feverGage.value >= 1)
            {
                isFever = true;
                tempJumpSpeed = jumpSpeed;
                jumpSpeed = 1;
                moveBtn[0].SetActive(false);
                moveBtn[1].SetActive(false);
                moveBtn[2].SetActive(true);

                //피버 레이블 화면에 보여줌//
                StartCoroutine(ShowFeverLabel());
                touchText.SetActive(true);
                Debug.Log("피버참");
                bgSoundManager.clip = feverSound;
                bgSoundManager.Play();

                feverFxPlane.GetComponent<FeverFxPlaneScript>().enabled = true;
                StartCoroutine(StopFeverFx());
            }
        }
        //피버게이지가 다 찾으니 피버가 발동하면서 피버게이지 줄임.
        if (isFever == true)
        {
            feverGage.value -= Time.deltaTime * feverTime;
            if (feverGage.value <= 0)
            {
                isFever = false;
                feverGage.value = 0;

                moveBtn[0].SetActive(true);
                moveBtn[1].SetActive(true);
                moveBtn[2].SetActive(false);

                jumpSpeed = tempJumpSpeed;
                bgSoundManager.clip = normalSound;
                bgSoundManager.Play();
            }
        }
        else
        {
            #region //상단 생명력 표시 실시간 변화//
            switch (baseTimeLife)
            {
                case true:
                    //시간기준으로 생명력 단축//
                    if (spendTime < 200) spendTime += Time.deltaTime * 2;
                    //Debug.Log("Spend Time :: " + spendTime);
                    lifeGage.value -= Time.deltaTime * (0.3f + spendTime * 0.001f);
                    break;
                //시간기준으로 생명력 단축//

                case false:
                    //오른 계단 기준으로 생명력 단축//
                    if (jumpCount > 0)
                    {
                        if (jumpCount < 200) addVal = jumpCount; else addVal = 200;
                        lifeGage.value -= Time.deltaTime * (0.3f + addVal * 0.001f);
                    }
                    break;
                //오른 계단 기준으로 생명력 단축//
            }
            if (lifeGage.value <= 0)
                isPlayerDrop = true;
            //오른 계단 기준으로 생명력 단축//
            #endregion //상단 생명력 표시 실시간 변화//
        }


        //캠이 서서히 위로 올라가서 캐릭터가 화면 밑으로 내려가도록 Y값을 보정//
        camPosY += 0.01f;

        camPosX = Mathf.Lerp(camPos.x, plPos.x, 0.2f);
        Camera.main.transform.localPosition = new Vector3(camPosX, camPosY, camPos.z);
        #endregion //카메라 위치 잡아줌//

        #region //플레이어 상태에 따른 변화-실질적으로 점프 체크를 위해 필요//
        switch (pStatus)
        {
            case status.idle:
                player.transform.GetChild(0).animation.Play("idle");
                break;

            case status.jumpLeft:
                frameCount++;
                player.transform.localRotation = Quaternion.Euler(0, 45, 0);

                //피버일때는 바로 다음 위치로 이동하게 한다.//
                if (isFever == true)
                {
                    IsJumpFeverComplete();
                    //player.transform.localPosition = newPos; 
                    //pStatus = status.idle;
                    //nextStepT = stairs.FindChild("Step" + jumpCount.ToString("D5")).transform;
                    //nextStepT.localScale = Vector3.one;
                }
                else
                {
                    player.transform.localPosition = Vector3.Lerp(nowPos, newPos, jumpSpeed * frameCount);
                    IsJumpComplete();
                }
                break;

            case status.jumpRight:
                frameCount++;
                player.transform.localRotation = Quaternion.Euler(0, -45, 0);

                if (isFever == true)
                {
                    IsJumpFeverComplete();
                    //player.transform.localPosition = newPos; 
                    //pStatus = status.idle;
                    //nextStepT = stairs.FindChild("Step" + jumpCount.ToString("D5")).transform;
                    //nextStepT.localScale = Vector3.one;
                }
                else
                {
                    player.transform.localPosition = Vector3.Lerp(nowPos, newPos, jumpSpeed * frameCount);
                    IsJumpComplete();
                }
                break;
        }
        #endregion //플레이어 상태에 따른 변화-실질적으로 점프 체크를 위해 필요//

    }

    IEnumerator ShowFeverLabel()
    {
        StartCoroutine(OffFeverLabel());
        feverLabel.SetActive(true);
        feverLabel.transform.localScale = Vector3.zero;
        float val = 0;
        while (true)
        {
            feverLabel.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1, 1, 1), val);
            if (val >= 1) break;
            val += Time.deltaTime * 2;
            yield return null;
        }
        feverLabel.transform.localScale = new Vector3(1,1,1);
    }

    IEnumerator OffFeverLabel()
    {
        yield return new WaitForSeconds(feverTimeOri * 0.8f);
        touchText.SetActive(false);
        StartCoroutine(BlinkFeverLabel());
        yield return new WaitForSeconds(feverTimeOri * 0.15f);
        float val = 0;
        while (true)
        {
            feverLabel.transform.localScale = Vector3.Lerp(new Vector3(1, 0.8f, 1), Vector3.zero, val);
            if (val >= 1) break;
            val += Time.deltaTime * 2;
            yield return null;
        }
        feverLabel.SetActive(false);
    }

    IEnumerator BlinkFeverLabel()
    {
        float val = 0;
        float valLimitTime = feverTimeOri * 0.19f;
        while (true)
        {
            feverLabel.GetComponent<UILabel>().enabled = !feverLabel.GetComponent<UILabel>().enabled;
            //Debug.Log("Blink :: " + feverLabel.GetComponent<UILabel>().enabled);
            if (val >= valLimitTime)
            {
                feverLabel.GetComponent<UILabel>().enabled = true; break;
            }
            val += Time.deltaTime;
            yield return null;
            yield return null;
        }
    }

    void IsJumpFeverComplete()
    {
        player.transform.localPosition = newPos;
        pStatus = status.idle;
        nextStepT = stairs.FindChild("Step" + jumpCount.ToString("D5")).transform;
        nextStepT.localScale = Vector3.one;
        CoinCheck();
    }

    void IsJumpComplete()
    {
       
        float nowPosX = player.transform.localPosition.x;
        float newPosX = newPos.x;
        float inter2Pos = 0;

        switch (pStatus)
        {
            case status.jumpLeft:
                inter2Pos = nowPosX - newPosX;
                break;

            case status.jumpRight:
                inter2Pos = newPosX - nowPosX;
                break;
        }

        //계단을 밟았는지 여부 검사//
        if (inter2Pos < collapseInter)
        {
            player.transform.localPosition = newPos;
            pStatus = status.idle;
            frameCount = 0;
            nextStepT = stairs.FindChild("Step" + jumpCount.ToString("D5")).transform;
            nextStepT.localScale = Vector3.one;
            
            if (newPos.x != nextStepT.localPosition.x)
            {
                isPlayerDrop = true;
                return;
            }
           
            //코인 있는지 여부 검사//
            CoinCheck();
            FeverCheck();
        }
    }

    //코인 있는지 여부 검사//
    void CoinCheck()
    {
        if (nextStepT.GetComponent<StepPrefer>().isCoin == true)
        {
            //Debug.Log("코인검사");
            getCoinCount += nextStepT.GetComponent<StepPrefer>().coinValue;
            goldCountLabel.text = getCoinCount.ToString();
            Transform coin = nextStepT.GetComponent<StepPrefer>().coinT;
            //Debug.Log("코인이름 :: " + coin.name);
            coin.parent = instanceCoinParent;
            coin.gameObject.SetActive(false);

            audio.PlayOneShot(getCoin);
            StartCoroutine(BubbleAction(goldCountLabel.transform));
        }
    }

    void CoinMaker(Transform step)
    {
        coinMakerRandom = Random.Range(0,100);

        if (coinMakerRandom < coinMakerPer)
        {
            for (int i = 0; i < instanceCoinParent.childCount; i++)
            {
                if (instanceCoinParent.GetChild(i).gameObject.activeSelf == false)
                {
                    Transform coin = instanceCoinParent.GetChild(i);
                    coin.gameObject.SetActive(true);
                    coin.parent = coins;
                    coin.localPosition = step.localPosition;
                    step.GetComponent<StepPrefer>().isCoin = true;
                    step.GetComponent<StepPrefer>().coinValue = 1;
                    step.GetComponent<StepPrefer>().coinT = coin;
                    break;
                }
            }
        }
        else
        {
            step.GetComponent<StepPrefer>().isCoin = false;
            step.GetComponent<StepPrefer>().coinValue = 0;
        }
    }

    void StepMaker()
    {
        stepChCount = instanceStepsParent.childCount;

        StepMakePosX = Mathf.Floor(StepMakePosX);
        chFirstPosX = StepMakePosX;

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < stepChCount; j++)
            {
                GO = instanceStepsParent.GetChild(j);
                if (GO.gameObject.activeSelf == false)
                    GO.gameObject.SetActive(true);
                GO.localScale = new Vector3(1, 1, 0.1f);
                break;
            }

            GO.parent = stairs;
            GO.localPosition = new Vector3(StepMakePosX, StepMakePosY, 0.5f);
            GO.name = "Step" + i.ToString("D5");

            //코인 여부 결정하여 생성//
            if (i > 0) CoinMaker(GO);
            //다음 계단 위치 미리 지정//
            StepNextPos();
        }
        stairs.GetChild(0).localScale = Vector3.zero;
    }
    void StepRelocater(int StepNum)
    {
        if (jumpCount < 8) return;
        GO = stairs.GetChild(0);
        GO.parent = instanceStepsParent;
        GO.parent = stairs;
        GO.localPosition = new Vector3(StepMakePosX, StepMakePosY, 0.5f);
        GO.localScale = Vector3.zero;
        GO.name = "Step" + StepNum.ToString("D5");

        StartCoroutine(ScaleUpStep(GO.transform));

        //코인 여부 결정하여 생성//
        CoinMaker(GO);
        //다음 계단 위치 미리 지정//
        StepNextPos();
    }

    void StepNextPos()
    {
        int leftRight;
        StepMakePosY += jumpY;

        if (StepMakePosX <= -10)
        {
            StepMakePosX += jumpX; 
            return;
        }
        if (StepMakePosX >= 10)
        {
            StepMakePosX += -jumpX; 
            return;
        }

        leftRight = Random.Range(0, 100);

        if (leftRight < 50)
            StepMakePosX += -jumpX;
        else
            StepMakePosX += jumpX;
    }

    void BuildingMaker()
    {
        Transform bgObj;
        for (int i = 0; i < 6; i++)
        {
            bgObj = instanceBuildingParent.GetChild(0);
            bgObj.parent = buildings;
            bgObj.gameObject.SetActive(true);
            bgObj.localPosition = new Vector3(0, i * 3, 0.5f);
        }
    }

    void BuildingRelocater()
    {
        if (camPos.y > buildings.GetChild(0).localPosition.y + 9)
        {
            GO = buildings.GetChild(0);
            GO.parent = instanceBuildingParent;
            GO.parent = buildings;
            GO.localPosition += new Vector3(0, 18, 0);
        }
    }

    void JumpDefault(int leftRight =1)
    {
        JumpCounter();
        player.transform.GetChild(0).animation.Play("jump");
        nowPos = player.transform.localPosition;
        newPos = nowPos + new Vector3(leftRight*jumpX, jumpY, 0);

    }


    IEnumerator HideLeftRightText()
    {
        yield return new WaitForSeconds(1f);
        float val = 1;
        while (val > 0)
        {
            leftText.GetComponent<UILabel>().color = new Color(1, 1, 1, val);
            rightText.GetComponent<UILabel>().color = new Color(1, 1, 1, val);
            val -= Time.deltaTime;
            yield return null;
        }

        leftText.SetActive(false);
        rightText.SetActive(false);
    }
    
    public void JumpLeft()
    {
        if (pStatus == status.jumpLeft || pStatus == status.jumpRight || isPlayerDrop == true || player == null) return;

        if (touchFirst == false)
        {
            StartCoroutine(HideLeftRightText());
            touchFirst = true;
        }

        pStatus = status.jumpLeft;
        JumpDefault(-1);
    }

    public void JumpRight()
    {
        if (pStatus == status.jumpLeft || pStatus == status.jumpRight || isPlayerDrop == true || player == null) return;

        if (touchFirst == false)
        {
            StartCoroutine(HideLeftRightText());
            touchFirst = true;
        }

        pStatus = status.jumpRight;
        JumpDefault(1);
    }

    bool isFeverContinue;
    //피버여부 체크//
    public void FeverCheck()
    {
        jumpClick2 = Time.time;
        jumpClickInter = jumpClick2 - jumpClick1;
        //Debug.Log("jumpClickInter :: " + jumpClickInter); ;

        if (jumpClickInter <= 0.15f)
        {
            feverGage.value += 0.012f;
            if (isFeverContinue == true)
                feverGage.value += 0.006f;
            isFeverContinue = true;
        }
        else if (jumpClickInter <= 0.25f)
        {
            feverGage.value += 0.005f;
            if (isFeverContinue == true)
                feverGage.value += 0.0025f;
            isFeverContinue = true;
        }
        else if (jumpClickInter <= 0.4f)
        {
            feverGage.value += 0.002f;
            if (isFeverContinue == true)
                feverGage.value += 0.001f;
            isFeverContinue = true;
        }
        else isFeverContinue = false;

        jumpClick1 = jumpClick2;
    }

    public void JumpCenter()
    {
        if (pStatus == status.jumpLeft || pStatus == status.jumpRight || isPlayerDrop == true || player == null) return;

        JumpCounter();
        player.transform.GetChild(0).animation.Play("jump");
        nowPos = player.transform.localPosition;

        nextStepT = stairs.FindChild("Step" + (jumpCount).ToString("D5")).transform;
        if (nextStepT.localPosition.x > nowPos.x)
        { pStatus = status.jumpRight; centerLR = 1; }
        else
        { pStatus = status.jumpLeft; centerLR = -1; }

        newPos = nowPos + new Vector3(centerLR * jumpX, jumpY, 0);

        //Debug.Log("pStatus :::: "+pStatus.ToString());
        //Debug.Log("newPos :::: " + newPos);

    }

    //계단 자동 생성//
    void JumpCounter()
    {
        jumpCount++;

        stepView.text = jumpCount.ToString();
        StartCoroutine(BubbleAction(stepView.transform));
        GetComponent<AudioSource>().Play();

        //생명 연장//
        lifeGage.value += 0.1f;
        if (lifeGage.value > 1) lifeGage.value = 1;


        StepRelocater(jumpCount + 7);
        //ExtendStepMaker(jumpCount+6);
        //hideSteps();

        BuildingRelocater();
    }

    IEnumerator ScaleUpStep(Transform step)
    {
        float val = 0;
        while (val < 1)
        {
            step.localScale = new Vector3(val, val, val * 0.1f);
            yield return null;
            val += Time.deltaTime * 3;
        }
        step.localScale = new Vector3(1, 1, 0.1f);
    }

    IEnumerator PlayerDead()
    {
        yield return new WaitForSeconds(0.5f);
        isDaed = true;

        Destroy(GameObject.Find("Player"));

        GetComponent<PopupController>().AddPopWin(ReStartWindow);
    }

    public void ReStart()
    {
        int child = stairs.childCount;
        Transform cObj;
        for (int i = 0; i < child; i++)
        {
            cObj = stairs.GetChild(0);
            cObj.parent = instanceStepsParent;
            cObj.gameObject.SetActive(false);
        }

        child = coins.childCount;
        for (int i = 0; i < child; i++)
        {
            cObj = coins.GetChild(0);
            cObj.parent = instanceCoinParent;
            cObj.gameObject.SetActive(false);
        }

        child = buildings.childCount;
        for (int i = 0; i < child; i++)
        {
            cObj = buildings.GetChild(0);
            cObj.parent = instanceBuildingParent;
            cObj.gameObject.SetActive(false);
        }

        GetComponent<PopupController>().CloseWindow();
        StartCoroutine(ReAct());
    }

    IEnumerator ReAct()
    {
        yield return null;
        Activate();
    }

    IEnumerator BubbleAction(Transform obj)
    {
        scaleV = new Vector3[]{ new Vector3(0.7f, 0.7f, 1f), new Vector3(1.2f, 1.2f, 1), new Vector3(1f, 1f, 1f) };
        
        for (int i = 0; i < scaleV.Length - 1; i++)
        {
            float val = 0;
            while (val < 1)
            {
                obj.localScale = Vector3.Lerp(scaleV[i], scaleV[i + 1], val);
                //Debug.Log("Win.localScale::: :" + i + ": ::: " + Win.localScale);

                val += Time.deltaTime * (15 + i);
                yield return null;
            }
        }

        obj.localScale = new Vector3(1, 1, 1);
    }

}