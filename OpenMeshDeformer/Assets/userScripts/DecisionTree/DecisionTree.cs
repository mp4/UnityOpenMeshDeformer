using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Troschuetz.Random;
using System;
using UnityEditor;
using System.Linq;

/// <summary>
/// Decision tree can be run in parrallel to must unity proccesses 
/// watch that events do not call functions that must be called on the 
/// main thread
/// </summary>
public class DecisionTree{
	Generator gen;
	Distribution dist;
	DecisionNode head = null;
	public Dictionary<string, VariableNode> variables;

	public DecisionTree()
	{
		variables = new Dictionary<string, VariableNode>();
	}
	/// <summary>
	/// Adds the node to the tree if parent is null sets the node as head
	/// </summary>
	/// <param name="node">Node.</param>
	/// <param name="parent">Parent.</param>
	public void AddNode(DecisionNode node, DecisionNode parent)
	{
		registerVariables(node);
		if(parent == null)
		{
			head = node;
			return;
		}
		var temp = head;
		while(temp != null)
		{
			if(parent == temp)
			{
				parent.node = node;
				return;
			}
			temp = temp.node;
		}
		throw new ArgumentException("parent not found");
	}
	/// <summary>
	/// Adds the node to the end of the tree
	/// </summary>
	/// <param name="node">Node.</param>
	public void AddNode(DecisionNode node)
	{
		registerVariables(node);
		if(head == null)
		{
			head= node;
		}
		//there may be a bug in here
		var temp = head;
		while(temp != null)
		{
			temp = temp.node;
		}
		temp = node;
	}
	protected void registerVariables(DecisionNode node)
	{
		foreach(VariableNode vax in node.variables_)
		{
			if(!variables.ContainsKey(vax.name_))
			{
				variables.Add(vax.name_, vax);
			}
		}
	}
	public virtual void AddVariable(VariableNode node)
	{
		if(variables.ContainsKey(node.name_))
			throw new ArgumentException("this variable has already been registered");
		variables.Add(node.name_, node);
	}
	/// <summary>
	/// Evalutes the tree calls all of the variables that are left
	/// once the tree is evaluated
	/// </summary>
	public void EvaluteTree()
	{
		if(head != null)
		{
			head.Evaluate(this);
		}
		else
		{
			throw new ArgumentNullException("must have set head before evaluating");
		}
	}

	public Generator currentGenerator
	{
		get{return gen;}
		set{gen = value;}
	}
	public Distribution currentDistribution
	{
		get{return dist;}
		set{dist = value;}
	}
}

public class DecisionNode
{
	//maybe should protect them
	public List<VariableNode> variables_;
	public List<VariableTransformation> transforms_;
	public DecisionNode node = null;
	//now for all of the constructors
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionNode"/> class.
	/// </summary>
	public DecisionNode():this(new List<VariableNode>(),
	                           new List<VariableTransformation>(),
	                           null){}
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionNode"/> class.
	/// </summary>
	/// <param name="transforms">list of transforms.</param>
	public DecisionNode(List<VariableTransformation> transforms):
		this(new List<VariableNode>(), transforms, null){}
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionNode"/> class.
	/// </summary>
	/// <param name="xnode">a variable node</param>
	public DecisionNode(VariableNode xnode):
		this(new List<VariableNode>(){xnode},null,null){}
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionNode"/> class.
	/// </summary>
	/// <param name="variables">Variables.</param>
	/// <param name="transforms">Transforms.</param>
	/// <param name="next">Next.</param>
	public DecisionNode(List<VariableNode> variables,
	                    List<VariableTransformation> transforms,
	                    DecisionNode next)
	{
		transforms_ = transforms;
		variables_ = variables;
		node = next;
	}
	public void Evaluate(DecisionTree tree)
	{
		for(int i=0;i<transforms_.Count;i++)
		{
			variables_ = transforms_[i].apply(variables_, tree);
		}
		if(node != null)
		{
			Debug.Log("moving on to the next node");
			node.variables_ = variables_;
			node.Evaluate(tree);
		}
		else //this is the end call all the functions
		{
			Debug.Log("finalizing variables");
			for(int i =0; i< variables_.Count;i++)
			{
				variables_[i].XFinalize();
			}
		}
	}
}
/// <summary>
/// Variable node stores the information about a variable 
/// this can be a base class as well
/// </summary>
public class VariableNode
{
	public string name_;
	public delegate void variableDelegate();
	public event variableDelegate variableEvent;
	public VariableNode(string name):this(name, null){}
	public VariableNode(string name, variableDelegate function)
	{
		name_ = name;
		variableEvent += function;
	}
	public virtual void XFinalize()
	{
		variableEvent();
	}
}
public class VariableTransformation
{
	List<string> fromThis;
	List<string> toThis;
	//DecisionNode node = null;
	public delegate bool conditionDelegate();
	conditionDelegate condition ;//= ()=>{return true;};
	public VariableTransformation(List<string> fromT, List<string> toT):this(fromT, toT, ()=>{return true;}){}
	public VariableTransformation(List<string> fromT, List<string> toT, conditionDelegate function)
	{
		fromThis = fromT;
		toThis = toT;
		condition = function;
	}
	//I need to leave a tie in for changing the variables as a virtual function
	public List<VariableNode> apply(List<VariableNode> variables, DecisionTree tree)
	{
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

