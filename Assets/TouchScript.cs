using UnityEngine;
using System.Collections;

public class TouchScript : MonoBehaviour
{
    string boxContent;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        touchClick();
    }

    // 터치 시 오브젝트 확인 함수
    void touchClick()
    {
        // 터치 입력이 들어올 경우
        if (Input.GetMouseButtonDown(0))
        {
            // 오브젝트 정보를 담을 변수 생성
            RaycastHit hit;
            // 터치 좌표를 담는 변수
            Ray touchray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 터치한 곳에 ray를 보냄
            Physics.Raycast(touchray, out hit);
            // ray가 오브젝트에 부딪힐 경우
            if (hit.collider != null && hit.transform.gameObject.GetComponent<UiButtonOnClick>() != null)
            {
                hit.collider.gameObject.GetComponent<UiButtonOnClick>().ClickEvent();
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}