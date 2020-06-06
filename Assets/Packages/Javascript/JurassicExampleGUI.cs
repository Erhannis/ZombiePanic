using UnityEngine;

public class JurassicExampleGUI : MonoBehaviour {
	
	// This class just contains the code for the Unity GUI to load example JavaScripts.
	
	JurassicExampleExecute je;
	public TextAsset[] scriptTxts;
	
	void Start ()
	{
		je = GetComponent<JurassicExampleExecute>();		
	}

	void Update () {
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 210, 10, 200, Screen.height-20));
		GUILayout.BeginVertical();
		GUILayout.Label("Click to load example JavaScripts");
		foreach(TextAsset ta in scriptTxts)
		{
			if (GUILayout.Button(ta.name))
			{
				je.codeString = ta.text;
				transform.position = Vector3.zero;
				je.floatingText = "";
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
}