using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uFAction;


public class UiButtonOnClick : MonoBehaviour
{
   // public List<EventDelegate> TargetMethod = new List<EventDelegate>();

    [ShowDelegate("On Click")]
    public KickassDelegate uAction = new KickassDelegate();

    public void ClickEvent()
    {
        uAction.InvokeWithEditorArgs();
        //EventDelegate.Execute(TargetMethod);
    }

    public void test()
    {
        Debug.Log("Test Test Test !!!!");

        UIButton.State.Hover.ToString();
    }
}
