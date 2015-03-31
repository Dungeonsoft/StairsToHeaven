using UnityEngine;
using System.Collections;

public class TextObjRotate : MonoBehaviour {

    public float rotMax = 30;

    bool isLeft = false;

    float rotY = 0;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        if (GameManager.jumpCount == 0) return;

        switch (GameManager.pStatus)
        {
            case GameManager.status.jumpLeft:
                isLeft = true;
                break;

            case GameManager.status.jumpRight:
                isLeft = false;
                break;
        }

        if (isLeft == true)
        {
            rotY = Mathf.Lerp(rotY,rotMax,0.25f);
        }
        else
        {
            rotY = Mathf.Lerp(rotY, -rotMax, 0.25f);
        }
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
	}

}
