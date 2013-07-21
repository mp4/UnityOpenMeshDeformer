using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LsysTest : MonoBehaviour {
	Timer timer;
	// Use this for initialization
	void Start () {
		Debug.Log("starting Lsys test");
//		timer = new Timer();
//		timer.Start();

		Lsystem lsys = new Lsystem();

		var block = new LsysBlock();
		block.Add(new LsysVar("a", aFinished, 1.0f, 0.5f));
		block.Add(new LsysOperation(new List<string>(){"a"}, new List<string>(){"b"}));
		lsys.AddBlock(block);
		lsys.AddVariable(new LsysVar("b", bFinished, 0.0f, 0.0f));
		lsys.EvaluateSystem();


		//Debug.Log(timer.Stop()+ ":lsys took to complete");
		Debug.Log("finishing Lsys test");
	}
	void aFinished(VariableNode node)//doesn't like LsysVar
	{
		Debug.Log("a finished");
	}
	void bFinished(VariableNode node)
	{
		Debug.Log("b finished");
	}
}
