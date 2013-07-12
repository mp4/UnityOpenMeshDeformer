#define ANYMESH_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class AnyMesh : MonoBehaviour {
	MeshXT2 meshX;
	public Mesh mesh;
	public MeshXT2.generators gen = MeshXT2.generators.MT19937;
	public MeshXT2.distributions dist = MeshXT2.distributions.Beta;
	// Use this for initialization
	void Start () {
//		meshX = new MeshXT2(1, MeshXT2.generators.MT19937,
//		                    MeshXT2.distributions.Bernoili, 
//		                    new Dictionary<string, double>(){{"alpha",1.0}});
		//chi, ALF
//		meshX = new MeshXT2(1, gen, dist, 
//		                    new Dictionary<string, double>(){{"alpha",5},{"beta", 0.1},{"gamma", 0.2}});
		meshX = new MeshXT2(1, gen, dist, 
		                    new Dictionary<string, double>(){{"alpha",4},{"beta", 0.1},{"gamma", 0.2}, {"mu", 0.5}, {"theta", 1.0}});
		meshX.mesh = mesh;
		if(mesh == null)
		{
			Debug.Log("mesh input is null");
		}
#if UNITY_EDITOR
		Debug.Log("editor directive works starting commit");
#endif
		meshX.CommitToTempFinished += step0;
		meshX.CommitToTemp();
	}
	void step0()
	{
		#if UNITY_EDITOR
		Debug.Log("in step 0");
//		for(int i = 0; i < meshX.vertices.Length; i++)
//		{
//			Debug.Log(meshX.vertices[i] +":vertex");
//		}
//		Debug.Log("end of vertices");
//		for(int i =0; i<meshX.triangles.Length; i++)
//		{
//			Debug.Log(meshX.triangles[i] + ":triangle");
//		}
//		Debug.Log("end of triangles");
//		for(int i=0;i <meshX.uv.Length; i++)
//		{
//			Debug.Log(meshX.uv[i] + ":uv");
//		}
//		Debug.Log("end uv");
		#endif
//#if UNITY_EDITOR
//		Debug.Log("step 0");
//#endif
		meshX.Tesselate();
		meshX.Tesselate();
		meshX.Tesselate();
		meshX.RecalaculateNormals();
		meshX.DeformByNormal(0.0f, 2.0f);
		meshX.RecalaculateNormals();
		//meshX.Tesselate();
		meshX.DeformByNormal(-1.0f, 0.0f);
		meshX.CommitToMasterFinishedOnMain += step1;
		meshX.CommitToMaster();
	}
	void step1()
	{
		#if UNITY_EDITOR
		Debug.Log("in step 1");
//		for(int i = 0; i < meshX.mesh.vertices.Length; i++)
//		{
//			Debug.Log(meshX.mesh.vertices[i] +":vertex");
//		}
//		Debug.Log("end of vertices");
		#endif
		meshX.mesh.RecalculateNormals();
		gameObject.GetComponent<MeshFilter>().mesh = meshX.mesh;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
