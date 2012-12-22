using UnityEngine;
using System;
using System.Collections;

public class AppKeyManager : MonoBehaviour {
	public static event Action appKeyAllowEvent;
	public static event Action<DontAllowReasons> appKeyDontAllowEvent;
	public string AppID;
	public enum DontAllowReasons {
		NOT_INSTALLED=0,
		NOT_RUNNING=1,
		INACTIVE=2
	}
	private bool LOGD = true;	// Set to true to activate debug logs.
	private static AndroidJavaClass mAppKeyPluginClass;
	private static AndroidJavaObject mAppKeyPlugin;
	private static AndroidJavaObject mActivity;
	private static AppKeyManager instance;

	void Start () {
		instance=this;
		if (gameObject.name!="AppKeyManager") Debug.LogError("Game object name must be AppKeyManager");
		#if UNITY_ANDROID
			mAppKeyPluginClass = new AndroidJavaClass("com.appkey.plugin.AppKeyPlugin");
			mAppKeyPlugin = mAppKeyPluginClass.CallStatic<AndroidJavaObject>("INSTANCE");
			mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		#endif
	}

	public static void CheckAppKey() {
		instance._CheckAppKey();
	}
	
	public static void CheckAppKeyWithWizard(string premiumContentDescription) {
		_CheckAppKeyWithWizard(premiumContentDescription);
	}

	public static void CheckAppKeyWithWizard(string premiumContentDescription, string premiumAppURL) {
		_CheckAppKeyWithWizard(premiumContentDescription, premiumAppURL);
	}
		
	protected void _CheckAppKey() {
		#if UNITY_ANDROID
			if (instance.LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('checkAccess',mActivity,instance.AppID), where instance.AppID="+instance.AppID);
			mAppKeyPlugin.Call("checkAccess",mActivity,instance.AppID);
		#else
			Debug.LogWarning("AppKey only available on Android platform.");	
		#endif	
	}		
	
	protected static void _CheckAppKeyWithWizard(string premiumContentDescription, string premiumAppURL) {
		#if UNITY_ANDROID
			if (instance.LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('checkAccess',mActivity,instance.AppID,premiumContentDescription,premiumAppURL), where instance.AppID="+instance.AppID+", premiumContentDescription="+premiumContentDescription+", premiumAppURL="+premiumAppURL);
			mAppKeyPlugin.Call("checkAccessWithWizard",mActivity,instance.AppID,premiumContentDescription,premiumAppURL);
		#else
			Debug.LogWarning("AppKey only available on Android platform.");	
		#endif
	}
	
	protected static void _CheckAppKeyWithWizard(string premiumContentDescription) {
		_CheckAppKeyWithWizard(premiumContentDescription,"");
	}
	
	//public static void OpenAppKeyInStore() {
	//	#if UNITY_ANDROID
	//		Application.OpenURL("https://play.google.com/store/apps/details?id=com.appkey.widget");	
	//	#else
	//		Debug.LogWarning("AppKey only available on Android platform.");	
	//	#endif
	//}

	public void allow(string empty) {
		if(LOGD) Debug.Log("AppKeyManager.allow Called");
		if (appKeyAllowEvent!=null) {
			appKeyAllowEvent();
		} else {
			Debug.LogWarning("AppKeyManager: No event handler defined for appKeyAllowEvent");
		}
	}
	

	public void dontAllow(string strReason) {
		if(LOGD) Debug.Log("AppKeyManager.dontAllow Called.  Reason=" + strReason);
		DontAllowReasons reason;
		switch (strReason) {
			case ("NOT_INSTALLED"):
					reason=DontAllowReasons.NOT_INSTALLED;
					break;
			case ("NOT_RUNNING"):
					reason=DontAllowReasons.NOT_RUNNING;
					break;
			case ("INACTIVE"):
					reason=DontAllowReasons.INACTIVE;
					break;
			default:
				reason=DontAllowReasons.NOT_INSTALLED;
				break;
		}

		if (appKeyDontAllowEvent!=null) {
		    appKeyDontAllowEvent(reason);}
		else {
			Debug.LogWarning("AppKeyManager: No event handler defined for appKeyDontAllowEvent");
		}
	}
}
