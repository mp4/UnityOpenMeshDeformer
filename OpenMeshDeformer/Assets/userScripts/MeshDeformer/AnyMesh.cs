using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class AnyMesh : MonoBehaviour {
	MeshXT2 meshX;
	public Mesh mesh;
	public MeshXT2.generators gen = MeshXT2.generators.MT19937;
	public MeshXT2.distributions dist = MeshXT2.distributions.Beta;
	public double alpha = 0.05;
	public double beta = 0.1;
	public double gamma = 0.2;
	public double mu = 0.5;
	public double theta = 1.0;
	public double lambda = 0.3;
	public double nu = 0.05;
	public double sigma = 1;
	Timer timer = new Timer();
	// Use this for initialization
	void Start () {
//		meshX = new MeshXT2(1, MeshXT2.generators.MT19937,
//		                    MeshXT2.distributions.Bernoili, 
//		                    new Dictionary<string, double>(){{"alpha",1.0}});
		//chi, ALF
//		meshX = new MeshXT2(1, gen, dist, 
//		                    new Dictionary<string, double>(){{"alpha",5},{"beta", 0.1},{"gamma", 0.2}});
		meshX = new MeshXT2(1, gen, dist, 
		                    new Dictionary<string, double>(){{"alpha",alpha},{"beta", beta}
			,{"gamma", gamma}, {"mu", mu}, {"theta", theta},{"lambda", lambda}, {"nu", nu},
			{"sigma", sigma}});
		meshX.mesh = mesh;
		if(mesh == null)
		{
			Debug.Log("mesh input is null");
		}
#if UNITY_EDITOR
		Debug.Log("editor directive works starting commit");
#endif
		timer.Start();
		//meshX.useTexture = false;
		meshX.CommitToTempFinished += step0;
		meshX.CommitToTemp();
	}
	void step0()
	{
		#if UNITY_EDITOR
		//Debug.Log("in step 0");
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
		//meshX.DeformByNormal(0.0f, 2.0f);
		meshX.DeformByNormal();
		meshX.RecalaculateNormals();
		//meshX.Tesselate();
		//meshX.DeformByNormal(-1.0f, 0.0f);
		meshX.DeformByNormal();
		//meshX.clearTexToBlack();
		meshX.setPixels(()=>{return new Color(
				(float)meshX.currentDistribution.NextDouble(),
				(float)meshX.currentDistribution.NextDouble(),
				//0,0,
				meshX.currentGenerator.NextBoolean()? 0.75f: 0.25f
				//(float)meshX.currentDistribution.NextDouble()
				);});
		//meshX.uvToSphericalCoords();
		//meshX.uvTofourCubesCoords();
		//meshX.uvToNbuCoords();
		meshX.xyzLT1();
		meshX.CommitToMasterFinishedOnMain += step1;
		meshX.CommitToMaster();
	}
	void step1()
	{
		#if UNITY_EDITOR
		//Debug.Log("in step 1");
//		for(int i = 0; i < meshX.mesh.vertices.Length; i++)
//		{
//			Debug.Log(meshX.mesh.vertices[i] +":vertex");
//		}
//		Debug.Log("end of vertices");
		#endif
		meshX.mesh.RecalculateNormals();
		gameObject.GetComponent<MeshFilter>().mesh = meshX.mesh;
		meshX.commitPixelsToTex();

		//gameObject.GetComponent<MeshRenderer>().material.SetTexture("Base (RGB)", meshX.tex);
		gameObject.GetComponent<MeshRenderer>().material.mainTexture = meshX.tex;
		Debug.Log(timer.Stop() + ":any mesh took <- seconds to complete");
		Debug.Log(meshX.tex.GetPixel(0,0) + "pixel 0,0 after assignment");
		transform.localScale = new Vector3(meshX.scale,meshX.scale,meshX.scale);
		//renderer.material.mainTexture =meshX.tex;
		//Debug.Log(gameObject.GetComponent<MeshRenderer>().material.mainTexture)
		//gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("", meshX.tex);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
