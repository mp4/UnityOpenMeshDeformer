using UnityEngine;
using System.Collections;

public class Timer {
	float beginTime;
	float endTime;
	// Use this for initialization
	public void Start () {
		beginTime = Time.time;
	}
	
	public float Stop()
	{
		endTime = Time.time;
		return endTime - beginTime;
	}
}
