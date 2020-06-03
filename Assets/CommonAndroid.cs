//#undef UNITY_EDITOR // Lets you edit Android code easily with formatting, comment out before going back to editor.
#if UNITY_ANDROID && !UNITY_EDITOR  // stop auto formatter removing unused using.
using UnityEngine;
#endif

// https://gist.github.com/StewMcc/9021882852013081ebd3a037a3f301dc
/// <summary>
/// @StewMcc 21/02/2018
/// </summary>

	/// <summary>
	/// Used for retrieving common Android classes, and objects for use with plug-ins or JNI calls.
	/// </summary>
	public class CommonAndroid {

#if UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass androidUnityActivity = null;

		/// <summary>    
		/// <para> Gets the current UnityActivity used on Android. </para>
		/// It will store the AndroidJavaClass for later use ensuring it is not creating a new
		/// class in memory every call.
		/// </summary>
		/// <returns> The AndroidActivity with the UnityPlayer running in it. </returns>
		public static AndroidJavaObject GetUnityActivity() {
			if (androidUnityActivity == null) {
				androidUnityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			}
			return androidUnityActivity.GetStatic<AndroidJavaObject>("currentActivity");
		}
#endif
	}