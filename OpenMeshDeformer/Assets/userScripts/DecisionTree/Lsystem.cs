using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

public class Lsystem :DecisionTree {


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
