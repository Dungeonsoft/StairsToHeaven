using UnityEngine;
using System.Collections;

public class ScreenSizeMatchingScript : MonoBehaviour
{
    public enum OrienteScreen : byte { landscape, portrait };
    public enum ArrangeScreen : byte { top, bottom, left, right };

    int scrX;
    int scrY;
    float scrRatio;
    public OrienteScreen oriScreen;
    public ArrangeScreen arScreen;
    //public bool isRight;
    public int intervalPixel = 100;

    int plusMinus;
    float xRatio;
    float yRatio;
    float dftXVal;
    float dftYVal;
    float fixedRatio;
    float xVal;
    float yVal;
    float zVal;

    void Awake()
    {
        switch (arScreen)
        {
            case ArrangeScreen.top :
            case ArrangeScreen.right :
               plusMinus = 1;
               break;

            case ArrangeScreen.bottom:
            case ArrangeScreen.left:
                 plusMinus = -1;
              break;
        }



        switch (oriScreen)
        {
            case OrienteScreen.portrait:
                dftXVal = 720f;
                dftYVal = 1280f;
                break;

            case OrienteScreen.landscape:
                dftXVal = 1280f;
                dftYVal = 720f;
                break;
        }


        scrX = Screen.width;
        scrY = Screen.height;
        fixedRatio = (dftXVal/dftYVal) / ((float)scrX / scrY) ;
        xRatio = scrX / dftXVal;
        yRatio = scrY / dftYVal;
        xVal = 0;
        yVal = 0;
        zVal = 0;

        switch (arScreen)
        {
            case ArrangeScreen.top:
            case ArrangeScreen.bottom:
                yVal = plusMinus * (dftYVal / 2 - intervalPixel) * fixedRatio;
                break;

            case ArrangeScreen.left:
            case ArrangeScreen.right:
                xVal = plusMinus * (dftXVal / 2 - intervalPixel) * fixedRatio;
                break;
        }
        transform.localPosition = new Vector3(xVal, yVal, zVal);
    }
}
