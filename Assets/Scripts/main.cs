using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {

	private string AppKeyResult="";
	
	void Start() {
		AppKeyManager.appKeyAllowEvent += AppKeyCallback_Allow;	
		AppKeyManager.appKeyDontAllowEvent += AppKeyCallback_DontAllow;	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape)) {
			Application.Quit();	
		}
	}
	
	void OnGUI () {
		
		if (GUI.Button(new Rect(0,0,200,200),"Call AppKey")) {
			AppKeyManager.CheckAppKey();
			Debug.Log("AppKey called");
		}
		GUI.Label(new Rect(300, 0, 200, 200), "AppKeyResult="+AppKeyResult);

		if (GUI.Button(new Rect(0,300,200,200),"Call AppKey with Wizard")) {
			AppKeyManager.CheckAppKeyWithWizard("[Premium Features]","");
		}
		
	}

	public void AppKeyCallback_Allow() {
		Debug.Log("AppKey access allowed!");
		AppKeyResult="Allow";
	}
	
	public void AppKeyCallback_DontAllow(AppKeyManager.DontAllowReasons reason) {
		Debug.Log("AppKey access not allowed for reason of "+reason);
		AppKeyResult="Don't allow for reason of "+reason;
	}

}
