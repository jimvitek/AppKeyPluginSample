using UnityEngine;
using System;
using System.Collections;

public class AppKeyManager : MonoBehaviour {
	public static event Action appKeyAllowEvent;
	public static event Action<DontAllowReasons> appKeyDontAllowEvent;
	public string AppID;
	public bool AnalyticsEnabled;
	public enum DontAllowReasons {
		NOT_INSTALLED=0,
		NOT_RUNNING=1,
		INACTIVE=2
	}
	private bool LOGD = true;	// Set to true to activate debug logs.
	
#if UNITY_ANDROID
	private static AndroidJavaClass mAppKeyPluginClass;
	private static AndroidJavaObject mAppKeyPlugin;
	private static AndroidJavaObject mActivity;
#endif
	
	private static AppKeyManager instance;

	void Start () {
		if(LOGD) Debug.Log("AppKeyManager: Start() called");
		instance=this;
		
		if (gameObject.name!="AppKeyManager") Debug.LogError("AppKeyManager: Game object name must be AppKeyManager");
		#if UNITY_ANDROID
			mAppKeyPluginClass = new AndroidJavaClass("com.appkey.plugin.AppKeyPlugin");
			mAppKeyPlugin = mAppKeyPluginClass.CallStatic<AndroidJavaObject>("INSTANCE");
			mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		#endif
	}
	
	public static bool IsAppKeySupported() {
		#if UNITY_ANDROID	
			return true;
		#else
			return false;
		#endif
	}

	public static void CheckAppKey() {
		if(instance == null) {
			Debug.LogWarning("AppKeyManager: No AppKeyManager instance detected, please make sure to add the AppKeyManager prefab to your scene.");
			return;
		}
		instance._CheckAppKey();
	}
	
	public static void CheckAppKeyWithWizard(string premiumContentDescription) {
		instance._CheckAppKeyWithWizard(premiumContentDescription);
	}

	public static void CheckAppKeyWithWizard(string premiumContentDescription, string premiumAppURL) {
		if(instance.LOGD) Debug.Log("AppKeyManager: Checking AppKey with wizard. Premium Content: " + premiumContentDescription + ", Premium App URL: " + premiumAppURL);
		instance._CheckAppKeyWithWizard(premiumContentDescription, premiumAppURL);
	}
		
	protected void _CheckAppKey() {
		#if UNITY_ANDROID
			if (LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('checkAccess',mActivity,instance.AppID, instance.AnalyticsEnabled), where instance.AppID="+instance.AppID+", instance.AnalyticsEnabled="+instance.AnalyticsEnabled);
			mAppKeyPlugin.Call("checkAccess",mActivity,instance.AppID,instance.AnalyticsEnabled);
		#else
			Debug.LogWarning("AppKeyManager: AppKey only available on Android platform.");	
		#endif	
	}		
	
	protected void _CheckAppKeyWithWizard(string premiumContentDescription, string premiumAppURL) {
		#if UNITY_ANDROID
			if (LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('checkAccess',mActivity,instance.AppID,instance.AnalyticsEnabled,premiumContentDescription,premiumAppURL), where instance.AppID="+instance.AppID+", instance.AnalyticsEnabled="+instance.AnalyticsEnabled+", premiumContentDescription="+premiumContentDescription+", premiumAppURL="+premiumAppURL);
			mAppKeyPlugin.Call("checkAccessWithWizard",mActivity,instance.AppID,instance.AnalyticsEnabled,premiumContentDescription,premiumAppURL);
		#else
			Debug.LogWarning("AppKeyManager: AppKey only available on Android platform.");	
		#endif
	}
	
	protected void _CheckAppKeyWithWizard(string premiumContentDescription) {
		_CheckAppKeyWithWizard(premiumContentDescription,"");
	}
	
	public static void OpenAppKeyInStore() {
		#if UNITY_ANDROID
			if(instance.LOGD) Debug.Log("AppKeyManager: Opening AppKey store link");
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.appkey.widget");	
		#else
			Debug.LogWarning("AppKeyManager: AppKey only available on Android platform.");	
		#endif
	}

	public void allow(string empty) {
		if(LOGD) Debug.Log("AppKeyManager: allow() Called");
		if (appKeyAllowEvent!=null) {
			appKeyAllowEvent();
		} else {
			Debug.LogWarning("AppKeyManager: No event handler defined for appKeyAllowEvent");
		}
	}
	

	public void dontAllow(string strReason) {
		if(LOGD) Debug.Log("AppKeyManager: dontAllow() Called.  Reason=" + strReason);
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
