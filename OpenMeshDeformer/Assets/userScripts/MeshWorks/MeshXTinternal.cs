using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshXTinternal : MonoBehaviour {
	public volatile List<Action> commands;
	public volatile bool commandReady = false;
	public delegate void CommandDelegate();
	public event CommandDelegate CommandEvent;
	void Start()
	{
		commands = new List<Action>();
		//Debug.Log("in internal start");
	}
	// Update is called once per frame
	void Update () {
		if(commandReady)
		{
#if UNITY_EDITOR
			Debug.Log("internal running command(s) on main");
#endif
			lock(commands)
			{
				for(int i=0;i<commands.Count; i++)
				{
					commands[i]();
				}
				commands.Clear();
				commandReady = false;
				if(CommandEvent != null)
				{
					CommandEvent();
					CommandEvent = null;
				}
			}
		}
	}
}
