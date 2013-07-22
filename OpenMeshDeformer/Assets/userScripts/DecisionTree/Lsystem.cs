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
	public void Add(LsysVar variable)
	{
		variables_.Add(variable);
	}
	public void Add(LsysOperation operation)
	{
		transforms_.Add(operation);
	}
}
public class LsysVar : VariableNode
{
	public float maxValue;
	public float minValue;
	public LsysVar(string name, VariableNode.variableDelegate onComplete, float max, float min):base(name, onComplete)
	{
		maxValue = max;
		minValue = min;
	}
}
public class LsysOperation : VariableTransformation
{
	public delegate LsysVar LsysOpdelegate(LsysVar input);
	public LsysOperation(List<string> fromThis, List<string> toThis): base(fromThis, toThis){}

	public override List<VariableNode> apply(List<VariableNode> variables, DecisionTreeBase tree)
	{
		Debug.Log("Lsys apply");
		if(fromThis == null)
		{
			throw new System.Exception("transform from this cannot be null");
		}
		if(condition())
		{
			List<string> temp = new List<string>();
			for(int i=0; i< variables.Count;i++)
			{
				temp.Add(variables[i].name_);
			}

			var indices = MathXT.ListInListAt(temp, fromThis);
			List<VariableNode> toInstansiate = new List<VariableNode>();
			try{
				for(int i=0; i< toThis.Count;i++)
				{
					toInstansiate.Add(tree.variables[toThis[i]]);
				}
			}
			catch
			{
				throw new System.Exception("a variable has not been defined");
			}
			//done building variable nodes
			//now replace nodes end to start

			//add transformaaion code here
			for(int i=indices.Count -1; i >=0; i--)
			{
				variables.RemoveRange(i, fromThis.Count);
				variables.InsertRange(i, toInstansiate);
			}
			//throw new NotImplementedException();
			return variables;
		}
		else
		{
			return variables;
		}
	}
}
