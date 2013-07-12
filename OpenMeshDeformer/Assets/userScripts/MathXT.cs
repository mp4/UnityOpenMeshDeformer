using UnityEngine;
using System.Collections;

public static class MathXT{

	public static Vector3 midPoint(Vector3 first, Vector3 second)
	{
		return new Vector3((first.x+second.x)/2.0f, (first.y+second.y)/2.0f, (first.z+second.z)/2.0f);
	}
	public static Vector2 midPoint(Vector2 first, Vector2 second)
	{
		return new Vector2((first.x + second.x)/2.0f, (first.y+second.y)/2.0f);
	}
}
