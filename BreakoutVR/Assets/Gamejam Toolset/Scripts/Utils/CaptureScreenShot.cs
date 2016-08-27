#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CaptureScreenShot : MonoBehaviour {

	public static void TakeScreenShot () {
		Application.CaptureScreenshot("Screenshots.png", 2);
	}
}
#endif
