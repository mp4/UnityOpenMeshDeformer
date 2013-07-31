using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Diagnostics;

[CustomEditor(typeof(ShaderImporter))]
public class OpenInNpp : Editor {
	Process x = null;
	void appQuit(System.Object sender, EventArgs o)
	{
		x = null;
		//UnityEngine.Debug.Log("setting proccess to null since it has exited");
	}
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		DrawDefaultInspector();
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if(GUILayout.Button("open in Npp"))
		{
			//UnityEngine.Debug.Log("open in Npp pressed" + path);
			try
			{
				if(x == null)
				{
					x = new Process();
					x.StartInfo.FileName = "notepad++.exe";// + " -lShaderLab d";
					x.StartInfo.Arguments = path;
					x.Exited += appQuit;
					x.Start();
				}
			}
			catch(Exception e)
			{
				//UnityEngine.Debug.LogException(e);
			}
		}
	}
}
