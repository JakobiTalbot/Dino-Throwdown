using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour {

    public Image loadScreen;

	// Use this for initialization
	void Start () {
        loadScreen.gameObject.SetActive(true);
        loadScreen.canvasRenderer.SetAlpha(1f);
        loadScreen.CrossFadeAlpha(0f, 2f, true);
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            FadeOut();
        }

        

	}

    void FadeOut()
    {
        Invoke("LoadScene", 2f);
        loadScreen.CrossFadeAlpha(1f, 2f, true);
    }

    void LoadScene()
    {
        Debug.Log("Loading");
    }
}
