using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class Potato : MonoBehaviour {
	MeshXT2 meshX;
	public int seed = 1;
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

	Timer timer;
	// Use this for initialization
	void Start () {
		//		meshX = new MeshXT2(1, MeshXT2.generators.MT19937,
		//		                    MeshXT2.distributions.Bernoili, 
		//		                    new Dictionary<string, double>(){{"alpha",1.0}});
		//chi, ALF
		//		meshX = new MeshXT2(1, gen, dist, 
		//		                    new Dictionary<string, double>(){{"alpha",5},{"beta", 0.1},{"gamma", 0.2}});
		meshX = new MeshXT2(seed, gen, dist, 
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
		timer = new Timer();
		timer.Start();
		meshX.useTexture = false;
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
		meshX.RecalaculateNormals();
		//meshX.Tesselate();
		//meshX.Tesselate();
		//meshX.ConvexTesselationBridging(0.1f);
		//meshX.ConvexTesselationBridging(0.05f);
		meshX.TesselateWithModifiedNormals(()=>{return 1.1f;});
		meshX.DeformByNormal(()=>{return 0.1f;});
//		meshX.Tesselate();
		//meshX.RecalaculateNormals();
		//meshX.DeformByNormal();
		meshX.DeformByNormal(()=>{return 0.5f* (float)meshX.currentDistribution.NextDouble();});
		meshX.setDistribution(MeshXT2.distributions.Laplace, new Dictionary<string, double>(){{"alpha",alpha},{"mu",mu}});
		meshX.RecalaculateNormals();
		//meshX.Tesselate();
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
		Debug.Log(timer.Stop() + ":potato took to complete");
	}
	// Update is called once per frame
	void Update () {

	}
}
