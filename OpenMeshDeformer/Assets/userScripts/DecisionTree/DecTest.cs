using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("starting decTreeTest");
		decTreeTest x = new decTreeTest();
		x.begin();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
public class decTreeTest
{
	public void begin()
	{
		DecisionTree tree = new DecisionTree();

		tree.AddNode(new DecisionNode(new List<VariableNode>()
		{
			new VariableNode("a", aCalled),new VariableNode("a", aCalled)
		},
		new List<VariableTransformation>()
		{
			new VariableTransformation(new List<string>(){"a", "a"}, new List<string>(){"b"})
		}
		,null));


		tree.AddVariable(new VariableNode("b", bCalled));
		//tree.EvaluteTree();
		tree.EvaluateTree();
	}
	void aCalled(VariableNode node)
	{
		Debug.Log("a called");
	}
	void bCalled(VariableNode node)
	{
		Debug.Log("b called");
	}
}