using UnityEngine;
using System.Collections;

public class FeverFxPlaneScript : MonoBehaviour {
    public Material mat;

    float matOffetY;
    bool toOpacity;
	// Use this for initialization
	void Awake () {
        mat.SetColor("_TintColor", new Color(1, 1, 1, 0));
	}
	
	// Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = new Vector2(0, matOffetY);

        if (matOffetY < 1 && toOpacity == false)
        {
            mat.SetColor("_TintColor", new Color(1, 1, 1, matOffetY));
        }
        else if (matOffetY > 1 && toOpacity == false)
        {
            toOpacity = true;
            mat.color = new Color(1, 1, 1, 1);
        }
        else if (matOffetY >= 9)
        {
            mat.SetColor("_TintColor", new Color(1, 1, 1, 10 - matOffetY));
        }
        matOffetY += Time.deltaTime;
    }

    public void Activate()
    {
        mat.SetColor("_TintColor", new Color(1, 1, 1, 0));

        matOffetY = 0;
        toOpacity = false;
    }
}
