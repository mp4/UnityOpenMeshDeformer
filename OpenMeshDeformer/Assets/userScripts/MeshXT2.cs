using UnityEngine;
using System.Collections;
using System.Threading;
using Troschuetz.Random;
using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Collections.Generic;


public class MeshXT2{
	public volatile Mesh mesh;
	public volatile Vector3[] vertices;
	public volatile int[] triangles;
	public volatile Vector2[] uv;
	public volatile string name;
	public volatile Vector3[] normals;
	Thread t;
	volatile MeshXTinternal internalObj;
	//MT19937Generator gen;
	Troschuetz.Random.Generator gen;
	Troschuetz.Random.Distribution dist = null;
	public delegate void nextStepDelegate();
	public MeshXT2()
	{
		if(Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>()==null)
		{
			internalObj = Camera.mainCamera.gameObject.AddComponent<MeshXTinternal> ();
		}
		else
		{
			internalObj = Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>();
		}
		gen = new MT19937Generator(0);
	}
	public MeshXT2(int seed)
	{
		if(Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>()==null)
		{
			internalObj = Camera.mainCamera.gameObject.AddComponent<MeshXTinternal> ();
		}
		else
		{
			internalObj = Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>();
		}
		gen = new MT19937Generator(seed);
	}
	public enum generators {ALF, MT19937, Standard, XorShift};
	public MeshXT2(int seed, generators genx)
	{
		if(Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>()==null)
		{
			internalObj = Camera.mainCamera.gameObject.AddComponent<MeshXTinternal> ();
		}
		else
		{
			internalObj = Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>();
		}
		switch(genx)
		{
		case generators.MT19937:
			gen = new MT19937Generator(seed);
			break;
		case generators.ALF:
			gen = new ALFGenerator(seed);
			break;
		case generators.Standard:
			gen = new StandardGenerator(seed);
			break;
		case generators.XorShift:
			gen = new XorShift128Generator(seed);
			break;
		}

	}
	public enum distributions {Bernoili, Beta, BetaPrime, Cauchy, Chi, ChiSquared,
		ContinuousUniform, DiscreteUniform, Erlang, Exponential, FisherSnedecor, FisherTippett,
	Gamma, Geometric, Laplace, LogNormal, Normal, Pareto, Poisson, Power, RayLeigh, StudentsT,
		Triangular, WeiBull };
	public MeshXT2(int seed, generators genx, distributions distx, Dictionary<string, double> args)
	{
		if(Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>()==null)
		{
			internalObj = Camera.mainCamera.gameObject.AddComponent<MeshXTinternal> ();
		}
		else
		{
			internalObj = Camera.mainCamera.gameObject.GetComponent<MeshXTinternal>();
		}
		switch(genx)
		{
			case generators.MT19937:
			gen = new MT19937Generator(seed);
			break;
			case generators.ALF:
			gen = new ALFGenerator(seed);
			break;
			case generators.Standard:
			gen = new StandardGenerator(seed);
			break;
			case generators.XorShift:
			gen = new XorShift128Generator(seed);
			break;
		}
		switch(distx)
		{
		case distributions.Bernoili:
			BernoulliDistribution x0 = new BernoulliDistribution(gen);
			if(args.ContainsKey("alpha"))
			{
				x0.Alpha = args["alpha"];
			}
			else
				throw new System.Exception("for Bernoili distribution you must provide an alpha");
			dist = x0;
			break;
		case distributions.Beta:
			BetaDistribution x1 = new BetaDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("beta"))
			{
				x1.Alpha = args["alpha"];
				x1.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception(" for beta distribution you must provide alpha and beta");
			}
			dist = x1;
			break;
		case distributions.BetaPrime:
			BetaPrimeDistribution x2 = new BetaPrimeDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("beta"))
			{
				x2.Alpha = args["alpha"];
				x2.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception(" for betaPrime distribution you must provide alpha and beta");
			}
			dist = x2;
			break;
		case distributions.Cauchy:
			CauchyDistribution x3 = new CauchyDistribution(gen);
			if(args.ContainsKey("alpha")&& args.ContainsKey("gamma"))
			{
				x3.Alpha = args["alpha"];
				x3.Gamma = args["gamma"];
			}
			else
			{
				throw new System.Exception("for cauchy dist you must provide alpha and gamma");
			}
			dist = x3;
			break;
		case distributions.Chi:
			ChiDistribution x4 = new ChiDistribution(gen);
			if(args.ContainsKey("alpha"))
			{
				x4.Alpha = (int)args["alpha"];
			}
			else
			{
				throw new System.Exception("for chi you must provide alpha");
			}
			dist = x4;
			break;
		case distributions.ChiSquared:
			ChiSquareDistribution x5 = new ChiSquareDistribution(gen);
			if(args.ContainsKey("alpha"))
			{
				x5.Alpha = (int)args["alpha"];
			}
			else
			{
				throw new System.Exception("for chiSquared you must provide alpha");
			}
			dist = x5;
			break;
		case distributions.ContinuousUniform:
			ContinuousUniformDistribution x6 = new ContinuousUniformDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("beta"))
			{
				x6.Alpha = args["alpha"];
				x6.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception("for ContinuousUniform you must provide alpha and beta");
			}
			dist = x6;
			break;
		case distributions.DiscreteUniform:
			DiscreteUniformDistribution x7 = new DiscreteUniformDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("beta"))
			{
				x7.Alpha = args["alpha"];
				x7.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception("for discrete uniform distribution you must provide alpha and beta");
			}
			dist = x7;
			break;
		case distributions.Erlang:
			ErlangDistribution x8 = new ErlangDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("lambda"))
			{
				x8.Alpha = args["alpha"];
				x8.Lambda = args["lambda"];
			}
			else
			{
				throw new System.Exception("for Erlang dist you must provide alpha and lambda");
			}
			dist = x8;
			break;
		case distributions.Exponential:
			ExponentialDistribution x9 = new ExponentialDistribution(gen);
			if(args.ContainsKey("lambda"))
			{
				x9.Lambda = args["lambda"];
			}
			else
			{
				throw new System.Exception("for exponential dist you must provide lambda");
			}
			dist = x9;
			break;
		case distributions.FisherSnedecor:
			FisherSnedecorDistribution x10 = new FisherSnedecorDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("beta"))
			{
				x10.Alpha = args["alpha"];
				x10.Beta =args["beta"];
			}
			else
			{
				throw new System.Exception("for FisherSnedecor you must provide alpha and beta");
			}
			dist = x10;
			break;
		case distributions.FisherTippett:
			FisherTippettDistribution x11 = new FisherTippettDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("mu"))
			{
				x11.Alpha = args["alpha"];
				x11.Mu = args["mu"];
			}
			else
			{
				throw new System.Exception("for FisherTippets you must provide alpha and mu");
			}
			dist = x11;
			break;
		case distributions.Gamma:
			GammaDistribution x12 = new GammaDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("theta"))
			{
				x12.Alpha = args["alpha"];
				x12.Theta = args["theta"];
			}
			else
			{
				throw new System.Exception("for Gamma dist you must provide alpha and theta");
			}
			dist = x12;
			break;
		}

	}
	/// <summary>
	/// Commits to temp and the local variables in the class this starts execution on a seperate thread and ends 
	/// when you call CommitToBase
	/// </summary>
	public delegate void CommitToTempDelegate();
	public event CommitToTempDelegate CommitToTempFinished;
	public void CommitToTemp()
	{
#if UNITY_EDITOR
		UnityEngine.Debug.Log("starting commit to temp");
#endif
		#if UNITY_EDITOR

		//			for(int i = 0; i < vertices.Length; i++)
		//			{
		//				Debug.Log(vertices[i] +":vertex");
		//			}
		//			Debug.Log("end of vertices");
//		for(int i =0; i<mesh.triangles.Length; i++)
//		{
//			Debug.Log(mesh.triangles[i] + ":triangle");
//		}
//		Debug.Log("end of triangles");
//		for(int i=0;i <mesh.uv.Length; i++)
//		{
//			Debug.Log(mesh.uv[i] + ":uv");
//		}
//		Debug.Log("end uv");
		#endif
		if(mesh == null)
		{
			throw new System.Exception("mesh cannot be null");
		}

				vertices = new Vector3[mesh.vertices.Length];
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = new Vector3(mesh.vertices[i].x,mesh.vertices[i].y, mesh.vertices[i].z);
				}

				triangles = new int[mesh.triangles.Length];
				//System.Buffer.BlockCopy(mesh.triangles, 0, triangles,0, mesh.triangles.Length);
		for(int i=0;i<triangles.Length; i++)
		{
			triangles[i] = mesh.triangles[i];
		}

				uv = new Vector2[mesh.uv.Length];
				for(int i=0; i<mesh.uv.Length; i++)
				{
					uv[i] = new Vector2(mesh.uv[i].x, mesh.uv[i].y);
				}
		normals = new Vector3[mesh.normals.Length];
		for(int i=0; i< normals.Length;i++)
		{
			normals[i] = new Vector3(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
		}
		name = mesh.name;

		t = new Thread(()=>{
			try{
				#if UNITY_EDITOR
				UnityEngine.Debug.Log("should have called CommitToTempFinished");
				#endif
			CommitToTempFinished();
			CommitToTempFinished = null;

			}
			catch (System.Exception e){
				while(e != null)
				{
					UnityEngine.Debug.Log(e.Message + ": Meshxt thread failed with");
					e = e.InnerException;
				}
			}
		});
		t.Start();

	}
	public delegate void CommitToMasterFinishedDelegate();
	public event CommitToMasterFinishedDelegate CommitToMasterFinished;
	public event CommitToMasterFinishedDelegate CommitToMasterFinishedOnMain;
	public void CommitToMaster()
	{
#if UNITY_EDITOR
		UnityEngine.Debug.Log("starting commit to master");
#endif

		internalObj.commands.Add(()=>{
			#if UNITY_EDITOR
			UnityEngine.Debug.Log("in Commit to master");
//			for(int i = 0; i < vertices.Length; i++)
//			{
//				Debug.Log(vertices[i] +":vertex");
//			}
//			Debug.Log("end of vertices");
//			for(int i =0; i<triangles.Length; i++)
//			{
//				Debug.Log(triangles[i] + ":triangle");
//			}
//			Debug.Log("end of triangles");
//			for(int i=0;i <uv.Length; i++)
//			{
//				UnityEngine.Debug.Log(uv[i] + ":uv");
//			}
//			UnityEngine.Debug.Log("end uv");
			#endif
			mesh = new Mesh();
			mesh.vertices = new Vector3[vertices.Length];
			Vector3[] temp = new Vector3[vertices.Length];
			for(int i=0; i<vertices.Length; i++)
			{
				temp[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
			}
			mesh.vertices = temp;
			var Temptriangles = new int[triangles.Length];
			//System.Buffer.BlockCopy(triangles,0, Temptriangles, 0, triangles.Length);
			for(int i=0; i< triangles.Length; i++)
			{
				Temptriangles[i] = triangles[i];
			}
			mesh.triangles = Temptriangles;

			 var meshuv = new Vector2[uv.Length];
			for(int i=0; i<uv.Length;i++)
			{
				meshuv[i] = new Vector2(uv[i].x, uv[i].y);
			}
			mesh.uv = meshuv;

			mesh.name = name;
			CommitToMasterFinishedOnMain();
			CommitToMasterFinishedOnMain = null;
			#if UNITY_EDITOR
			UnityEngine.Debug.Log("in end on commit to master");
//			for(int i = 0; i < mesh.vertices.Length; i++)
//			{
//				Debug.Log(mesh.vertices[i] +":vertex");
//			}
//			Debug.Log("end of vertices");
//			for(int i =0; i<mesh.triangles.Length; i++)
//			{
//				Debug.Log(mesh.triangles[i] + ":triangle");
//			}
//			Debug.Log("end of triangles");
			#endif
		});

//		CommitToMasterFinished();
//		CommitToMasterFinished = null;
//
//		internalObj.commands.Add(()=>{
//			CommitToMasterFinishedOnMain();
//			CommitToMasterFinishedOnMain = null;
//		});

		internalObj.commandReady =true;
	}
	//public event nextStepDelegate DefromByNormalDone;
	/// <summary>
	/// Deforms the mesh by the normals between the amounts specified
	/// </summary>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
	public void DeformByNormal(float min, float max)
	{
		for(int i = 0; i<vertices.Length ;i++)
		{
			Vector3 temp;
			if(dist == null)
				temp = normals[i]*(float)gen.NextDouble(min, max);
			else
				temp = normals[i]*(float)dist.NextDouble();
			for(int k=0; k<vertices.Length;k++)
			{
				if(i == k)
					continue;
				else if(vertices[i] == vertices[k])
				{
					vertices[k] += temp;
				}
			}
			vertices[i] += temp;
		}
	}
	/// <summary>
	/// Tesselate this mesh subdivides each triangle in the mesh into 4 triangles
	/// should not be run more than four times even on very simple meshes
	/// </summary>
	public void Tesselate()
	{
		try
		{
		var tempVerts = new Vector3[triangles.Length*4];
		var tempUv = new Vector2[triangles.Length*4];
		var tempTris = new int[triangles.Length *4];
		var tempNormals = new Vector3[triangles.Length*4];

		for(int i = 0; i < (triangles.Length/3); i++)
		{
			tempTris[i*12] = (i*12); //triangles[i*3];
			tempVerts[i*12] = vertices[triangles[i*3]];
			tempNormals[i*12] = normals[triangles[i*3]];
			tempUv[i*12] = uv[triangles[i*3]];

			Vector2 uvM0 = MathXT.midPoint(uv[triangles[(i*3)+1]], uv[triangles[(i*3)+2]]);
			Vector2 uvM1 = MathXT.midPoint(uv[triangles[(i*3)]], uv[triangles[(i*3)+2]]);
			Vector2 uvM2 = MathXT.midPoint(uv[triangles[(i*3)]], uv[triangles[(i*3)+1]]);

			Vector3 m0 = MathXT.midPoint(vertices[triangles[(i*3)+2]], vertices[triangles[(i*3)+1]]);
			Vector3 m1 = MathXT.midPoint(vertices[triangles[(i*3)]], vertices[triangles[(i*3)+2]]);
			Vector3 m2 = MathXT.midPoint(vertices[triangles[(i*3)]], vertices[triangles[(i*3)+1]]);

			tempVerts[(i*12)+1] = m2;
			tempTris[(i*12)+1] = (i*12)+1;
			tempNormals[(i*12)+1] = normals[triangles[i*3]];
			tempUv[(i*12)+1] = uvM2;

			tempVerts[(i*12)+2] = m1;
			tempTris[(i*12)+2] = (i*12)+2;
			tempNormals[(i*12)+2] = normals[triangles[i*3]];
			tempUv[(i*12)+2] = uvM1;

			tempVerts[(i*12)+3] = vertices[triangles[(i*3)+1]];
			tempTris[(i*12)+3] = (i*12)+3;//triangles[(i*3)+1];
			tempNormals[(i*12)+3] = normals[triangles[(i*3)+1]];
			tempUv[(i*12)+3] = uv[triangles[(i*3)+1]];

			tempVerts[(i*12)+4] = m0;
			tempTris[(i*12)+4] = (i*12)+4;
			tempNormals[(i*12)+4] = normals[triangles[(i*3)+1]];
			tempUv[(i*12)+4] = uvM0;

			tempVerts[(i*12)+5] = m2;
			tempTris[(i*12)+5] = (i*12)+5;
			tempNormals[(i*12)+5] = normals[triangles[(i*3)+1]];
			tempUv[(i*12)+5] = uvM2;
			//triangle 3
			tempVerts[(i*12)+6] = vertices[triangles[(i*3)+2]];
			tempTris[(i*12)+6] = (i*12)+6;//triangles[(i*3)+2];
			tempNormals[(i*12)+6] = normals[triangles[(i*3)+2]];
			tempUv[(i*12)+6] = uv[triangles[(i*3)+2]];

			tempVerts[(i*12)+7] = m1;
			tempTris[(i*12)+7] = (i*12)+7;
			tempNormals[(i*12)+7] = normals[triangles[(i*3)+2]];
			tempUv[(i*12)+7] = uvM1;

			tempVerts[(i*12)+8] = m0;
			tempTris[(i*12)+8] = (i*12)+8;
			tempNormals[(i*12)+8] = normals[triangles[(i*3)+2]];
			tempUv[(i*12)+8] = uvM0;
			//triangle 4 last one
			tempVerts[(i*12)+9] = m0;
			tempTris[(i*12)+9] = (i*12)+9;
			tempNormals[(i*12)+9] = normals[triangles[(i*3)+1]];
			tempUv[(i*12)+9] = uvM0;

			tempVerts[(i*12)+10] = m1;
			tempTris[(i*12)+10] = (i*12)+10;
			tempNormals[(i*12)+10] = normals[triangles[(i*3)+2]];
			tempUv[(i*12)+10] = uvM1;

			tempVerts[(i*12)+11] = m2;
			tempTris[(i*12)+11] = (i*12)+11;
			tempNormals[(i*12)+11] = normals[triangles[(i*3)]];
			tempUv[(i*12)+11] = uvM2;
		}
		triangles = tempTris;
		vertices = tempVerts;
		uv = tempUv;
		normals = tempNormals;
		}
		catch(System.Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}
	}
	public void RecalaculateNormals()
	{
		for(int i=0; i< (triangles.Length/3); i++)
		{
			var temp = Vector3.Cross(vertices[triangles[i*3]] - vertices[triangles[(i*3)+1]],
			                         vertices[triangles[(i*3)]] - vertices[triangles[(i*3)+2]]);
			normals[triangles[(i*3)]] = temp;
			normals[triangles[(i*3)+1]] = temp;
			normals[triangles[(i*3)+2]] = temp;
		}
	}
}
