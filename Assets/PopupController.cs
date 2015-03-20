using UnityEngine;
using System.Collections;

struct PopupWin
{
    public GameObject popupWin;
    public int orderNum;
    public System.Action WithMethod;
    public PopupWin(GameObject pw, int oNum, System.Action wd)
    {
        popupWin = pw;
        orderNum = oNum;
        WithMethod = wd;
    }
}

public class PopupController : MonoBehaviour
{
    public GameObject BalckScreen;

    ArrayList popupList = new ArrayList();

    void Awake()
    {
        BalckScreen.SetActive(false);
        //BalckScreen = GameObject.Find("Windows").transform.FindChild("HalfBLKPanel").gameObject;
    }

    public int PopupListCount()
    {
        int count = popupList.Count;
        Debug.Log("popupList.Count :::" + popupList.Count);
        if (popupList == null) count = 0;
        return count;
    }

    //실행되어야될 팝업들을 하나씩 쟁여놓는 기능을 해주는 메소드//
    //orderNum은 팝업 실행의 우선 순위이며 작은 수 일수록 먼저 실행하게 한다//
    public void AddPopWin(GameObject window, int orderNum =0, System.Action WithMethod = null, bool isClosePop = false, System.Action pastPopMtd = null)
    {
        /*
         * window : 추가될 팝업 윈도우 이름
         * orderNum : 추가될 팝업 윈도우 보임 우선순위
         * withMethod : 추가될 팝업이 실행될때 같이 실행되는 메소드
         * isClosePop : 이전에 떠있는 팝업이 있을때 그것을 끌지 말지 결정하는 불린값
         * pastPopMtd : 이전에 떠있는 팝업이 꺼지게 되면 그때 같이 실행될 메소드
         */ 
        window.SetActive(false);
        PopupWin win = new PopupWin(window, orderNum, WithMethod);
        popupList.Add(win);

        //혹시 먼저 실행되서 떠 있던 창을 끄고 연달아서 창을 켜는 상황이라면
        //이전창이 있을수 있다//
        //그때 이전창이 있다는 것을 true로 알리고 그와 관련된(실행되어야할)
        //메소드가 있다면 함께 넘겨주어 시행이 되도록 만들어 준다//
        if (isClosePop == true)
        {
            CloseWindow(pastPopMtd);
        }

        //열 팝업 창이 있는지 검사해서 팝업을 도와주는 메소드//
        Debug.Log("오픈에서 팝업 호출?");
        PopupEvent();
    }

    //제일 앞쪽의 팝업창을 열어주는 기능을 하는 메소드이다//
    void PopStart()
    {
        Debug.Log("Into PopUp");
        //팝업창을 열기전에 먼저 우선순위대로 정렬을 먼저 해준다//
        SortPopup(popupList);
        //배열의 제일 앞쪽의 스트럭트를 가지고 온다//
        PopupWin openWin = (PopupWin)popupList[0];
        //스트럭트에 담겨있는 팝업창을 true로 만들어 보이게 한다//
        openWin.popupWin.SetActive(true);
        Debug.Log(openWin.popupWin.name + " Active :: " + openWin.popupWin.activeSelf);
        //창이 나타날때 버블 액션을 해준다//
        StartCoroutine(BubbleAction(openWin.popupWin.transform));
        //창을 활성화 시키면서 창을 실행할때 필요한 것들이 있으면//
        //그 내용들을 실행한다(필요한것들은 액션(딜리게이트)를 이용하여 끌고올 수 있다)//
        if (openWin.WithMethod != null) openWin.WithMethod();
        //반투명 창을 활성화해서 뒤에 있는 내용들이 더이상 클릭되지 않게 한다//
        BalckScreen.SetActive(true);
    }

    IEnumerator BubbleAction(Transform Win, bool isOpen = true)
    {
        Vector3[] scaleV;

        Vector3[] openV = { new Vector3(0.7f, 0.7f, 1f), new Vector3(1.2f, 1.2f, 1), new Vector3(1f, 1f, 1f) };
        Vector3[] closeV = { new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1), new Vector3(0f, 0f, 0f) };

        if (isOpen == true) scaleV = openV; else scaleV = closeV;
        for (int i = 0; i < scaleV.Length - 1; i++)
        {
            float val = 0;
            while (val < 1)
            {
                Win.localScale = Vector3.Lerp(scaleV[i], scaleV[i + 1], val);
                //Debug.Log("Win.localScale::: :" + i + ": ::: " + Win.localScale);

                val += Time.deltaTime * (10 + i);
                yield return null;
            }
        }

        Win.localScale = new Vector3(1, 1, 1);
        if (isOpen == false) Win.gameObject.SetActive(false);
    }


    public void CloseWindowAny(){ CloseWindow(); }
    //실행되고 종료되는 팝업을 여기서 처리해준다//
    //우선은 팝업을 하이드하고 목록에서 제거하여 다음 팝업이 실행될 수 있도록 해준다//
    public void CloseWindow(System.Action NextF = null)
    {
        if (popupList.Count <= 0) return;
        //제일 앞단에 있는 팝업을 불러온다//
        PopupWin openWinC = (PopupWin)popupList[0];

        Debug.Log("openWinC Name :: " + openWinC.popupWin.name);
        //먼저 팝업을 하이드 시킨다//
        StartCoroutine(BubbleAction(openWinC.popupWin.transform, false));
        //openWinC.popupWin.SetActive(false);
        //이미실행되고 하이드된 된 팝업이니 실행목록에서 제거한다.// 
        popupList.RemoveRange(0, 1);
        //팝업을 하이드 시키면서 실행해야될 내용들이 있어서 메소드를 넘겨 받았으면//
        //그 내용을 실행한다//
        if (NextF != null) NextF();

        //팝업창 리스트 갯수가 0이면 반투명 막을 꺼준다//
        if (popupList.Count == 0) BalckScreen.SetActive(false);

        //열 팝업 창이 있는지 검사해서 팝업을 도와주는 메소드//
        Debug.Log("클로즈에서 팝업 호출?");
        PopupEvent();
    }

    //번호가 작은 순서대로(먼저샐행되어야 될 순서대로) 오브젝트들을 정렬해준다//
    void SortPopup(ArrayList data)
    {
        Debug.Log("data.Count ::: " + data.Count + " ::::"); return;

        //실행해야할 팝업이 하나밖에 없다면 소팅이 필요없으니 그냥 리턴한다//
        if (data.Count <= 1) return;
        //두개이상일 경우 먼저 샐행되어야할 팝업을 정렬해주어야되기 때문에 아래내용을 실행한다//
        PopupWin tempP01 = new PopupWin();
        PopupWin tempP02 = new PopupWin();
        for (int i = 0; i < data.Count - 1; i++)
        {
            for (int wNum = 0; wNum < data.Count - 1; wNum++)
            {
                tempP01 = (PopupWin)data[wNum];
                Debug.Log("tempP01 :: " + tempP01.orderNum);
                tempP02 = (PopupWin)data[wNum + 1];
                Debug.Log("tempP02 :: " + tempP02.orderNum);
                if (tempP01.orderNum > tempP02.orderNum)
                {
                    data[wNum] = tempP02;
                    data[wNum + 1] = tempP01;
                }
            }
        }
    }




    //열 팝업 창이 있는지 검사해서 팝업을 도와주는 메소드//
    void PopupEvent()
    {
        //이 부분은 메 프레임마다 실행을 하기 때문에//
        //실행해야할 팝업이 하나라도 있으면 자동을 팝업을 열어준다//
        //일차로 팝업리스트가 하나라도 있는지 확인한다.//
        if (popupList.Count > 0)
        {
            //제일 앞에 있는 팝업이 활성화 되어있는지 확인한다//
            //활성화 되어있는 팝업이 없다는 것은 팝업 윈도우 처리가 다 되어있지 않다는 것이기//
            //때문에 팝업을 보여주는 메소드 PopStart()를 실행한다//
            PopupWin firstObject = (PopupWin)popupList[0];

            Debug.Log("firstObject  Name ::: " + firstObject.popupWin.name);
            Debug.Log("firstObject ::: " + firstObject.popupWin.activeSelf);

            if (firstObject.popupWin.activeSelf == false)
            {
                PopStart();
            }
        }
    }
}