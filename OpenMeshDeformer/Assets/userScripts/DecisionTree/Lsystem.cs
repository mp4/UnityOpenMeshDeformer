using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

public class Lsystem :DecisionTreeBase {
	public void AddBlock (LsysBlock block)
	{
		base.AddNode (block);
	}
	public void EvaluateSystem()
	{
		base.EvaluteTree();
	}
//	public void AddNode(LsysBlock node,LsysBlock parent)
//	{
//
//	}
	public void AddVariable(LsysVar variable)
	{
		base.AddVariable(variable);
	}
}
public class LsysBlock : DecisionNode
{

}
public class LsysVar : VariableNode
{
	public LsysVar(string name, VariableNode.variableDelegate onComplete, float inVal):base(name, onComplete)
	{}
}
public class LsysOperation : VariableTransformation
{
	public delegate LsysVar LsysOpdelegate(LsysVar input);
	public LsysOperation(List<string> fromThis, List<string> toThis): base(fromThis, toThis){}
}
