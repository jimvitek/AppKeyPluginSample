using UnityEngine;
using System.Collections;

// Example implementation of AppKey integration
public class AppKeyEventListener : MonoBehaviour {
	
	public void Start() {
		// Check whether AppKey is allowed, and show the user an instructional wizard if it is not.
		// Event handler functions will still be called.
		AppKeyManager.CheckAppKeyWithWizard("[My Premium Content]");
	}
	
#if UNITY_ANDROID
	void OnEnable()
	{
		// Add AppKey event handlers when this script is enabled
		AppKeyManager.appKeyAllowEvent 		+= AppKeyCallback_Allow;
		AppKeyManager.appKeyDontAllowEvent 	+= AppKeyCallback_DontAllow;
	}
	
	void OnDisable() {
		// Remove event handlers when the script is disabled
		AppKeyManager.appKeyAllowEvent 		-= AppKeyCallback_Allow;
		AppKeyManager.appKeyDontAllowEvent 	-= AppKeyCallback_DontAllow;
	}
#endif
	
	public void AppKeyCallback_Allow() {
		// This will run when AppKey is installed, running, and active on the user's phone.
		
		// <Your code here.>
		
		Debug.Log("AppKey access allowed!");
	}
	
	public void AppKeyCallback_DontAllow(AppKeyManager.DontAllowReasons reason) {
		// This will run when AppKey is not installed or not running.
		
		// <Your code here.>
		
		Debug.Log("AppKey access not allowed for reason of "+reason);
	}
}
