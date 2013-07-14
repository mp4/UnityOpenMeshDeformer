using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor;
using System;

public static class MathXT{

	public static Vector3 midPoint(Vector3 first, Vector3 second)
	{
		return new Vector3((first.x+second.x)/2.0f, (first.y+second.y)/2.0f, (first.z+second.z)/2.0f);
	}
	public static Vector2 midPoint(Vector2 first, Vector2 second)
	{
		return new Vector2((first.x + second.x)/2.0f, (first.y+second.y)/2.0f);
	}
	/// <summary>
	/// searches for and returns the indexs at which unique instances of searchedFor can be found
	/// defaults to finding the first instance
	/// example base = ababab and searchedFor=abab it will return an index of 0 not 0 and 2
	/// </summary>
	/// <returns>The <see cref="System.Collections.Generic.List`1[[System.Int32]]"/>.</returns>
	/// <param name="baseList">Base list.</param>
	/// <param name="searchFor">Search for.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static List<int> ListInListAt<T>(List<T> baseList, List<T> searchFor) where T: IComparable
	{
		if(searchFor == null || searchFor.Count == 0)
			return null;
		List<int> outPut = new List<int>();
		for(int i =0; i< baseList.Count; i++)
		{
			if((baseList.Count - i)< searchFor.Count)
				return outPut;

			if(baseList[i].Equals(searchFor[0]))
			{
				if(searchFor.Count == 1)
				{
					outPut.Add(i);
				}
				for(int k=1; k<searchFor.Count;k++)
				{
					//don't go off the end
					if((i+k) == baseList.Count)
					{
						return outPut;
					}
					if(k == (searchFor.Count -1) && baseList[i+k].Equals(searchFor[k]))//we have a match
					{
						outPut.Add(i);
						i = i+k+1;//kick i out of the current instance
						break;
					}
					if(!baseList[i+k].Equals(searchFor[k]))
						break;
				}
			}
		}

		return outPut;
	}
}
