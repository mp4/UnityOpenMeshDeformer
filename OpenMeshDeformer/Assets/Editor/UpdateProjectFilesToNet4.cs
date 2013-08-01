using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
/// <summary>
/// Update project files to net4 from net 3.5 this will allow you to compile
/// net/mono4.0 code in Monodevelop instead of having to allow unity to background compile it
/// for you tested on Win7 by Marsh Poulson
/// </summary>
[InitializeOnLoad]
public class UpdateProjectFilesToNet4{


	static UpdateProjectFilesToNet4()
	{
//		Debug.Log(" project files would be updated");
//		Debug.Log(EditorApplication.applicationContentsPath+":E contents path");
//		Debug.Log(EditorApplication.applicationPath + ":E application path");
//		Debug.Log(Application.dataPath + ":app datapath");
		try
		{
			string dp = Application.dataPath;
			dp = dp.TrimEnd("/Assets".ToCharArray());
			//Debug.Log(dp + ":trimmed data path");
			string file = File.ReadAllText(dp + "/Assembly-CSharp.csproj");

			file = file.Replace("3.5", "4.0");
			File.WriteAllText(dp  + "/Assembly-CSharp.csproj", file);

			//one down three to go
			file = File.ReadAllText(dp + "/Assembly-CSharp-Editor.csproj");
			file = file.Replace("3.5","4.0");
			File.WriteAllText(dp +  "/Assembly-CSharp-Editor.csproj", file);

			file = File.ReadAllText(dp + "/Assembly-CSharp-firstpass.csproj");
			file = file.Replace("3.5", "4.0");
			File.WriteAllText(dp + "/Assembly-CSharp-firstpass.csproj", file);
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}
	}
}
