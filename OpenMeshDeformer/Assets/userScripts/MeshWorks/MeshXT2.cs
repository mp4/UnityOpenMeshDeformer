using UnityEngine;
using System.Collections;
using System.Threading;
using Troschuetz.Random;
using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Collections.Generic;
using System.Security.Cryptography;

public class MeshXT2{
	public volatile Mesh mesh;
	public volatile Vector3[] vertices;
	public volatile int[] triangles;
	public volatile Vector2[] uv;
	public volatile string name;
	public volatile Vector3[] normals;
	Thread t;
	volatile MeshXTinternal internalObj;
	//may add half edges and faces later to be computed right after vertices are loaded in
	//MT19937Generator gen;
	Troschuetz.Random.Generator gen;
	Troschuetz.Random.Distribution dist = null;
	public delegate void nextStepDelegate();
	public MeshXT2() : this(0, MeshXT2.generators.MT19937){}

	public MeshXT2(int seed) : this (seed, MeshXT2.generators.MT19937){}

	public enum generators {
		/// <summary>
		/// The ALF
		/// </summary>
		ALF,
		/// <summary>
		/// The Mt19937.
		/// </summary>
		MT19937,
		/// <summary>
		/// The standard.
		/// </summary>
		Standard, 
		/// <summary>
		/// The xor shift.
		/// </summary>
		XorShift};

	public MeshXT2(int seed, generators genx) : this(seed, genx, MeshXT2.distributions.None, null){}

	//TODO add links to help using each distribution
	public enum distributions {
		/// <summary>
		/// The bernoili distribution alpha is desired error level
		/// http://en.wikibooks.org/wiki/Statistics/Distributions/Bernoulli
		///  1= alpha
		/// 0 = 1-alpha
		/// </summary>
		Bernoili,
		/// <summary>
		/// The beta dist mean ~= alpha/(alpha + beta)
		/// http://stats.stackexchange.com/questions/47771/what-is-intuition-behind-beta-distribution
		/// </summary>
		Beta, 
		/// <summary>
		/// The beta prime.
		/// http://en.wikipedia.org/wiki/Beta_prime_distribution
		/// </summary>
		BetaPrime,
		/// <summary>
		/// The binomial.
		/// http://en.wikipedia.org/wiki/Binomial_distribution
		/// </summary>
		Binomial,
		/// <summary>
		/// The cauchy.
		/// </summary>
		Cauchy,
		/// <summary>
		/// The chi.
		/// </summary>
		Chi, 
		/// <summary>
		/// The chi squared.
		/// </summary>
		ChiSquared,
		/// <summary>
		/// The continuous uniform.
		/// </summary>
		ContinuousUniform,
		/// <summary>
		/// The discrete uniform.
		/// </summary>
		DiscreteUniform, 
		/// <summary>
		/// The erlang.
		/// </summary>
		Erlang, 
		/// <summary>
		/// The exponential.
		/// </summary>
		Exponential, 
		/// <summary>
		/// The fisher snedecor.
		/// </summary>
		FisherSnedecor, 
		/// <summary>
		/// The fisher tippett.
		/// </summary>
		FisherTippett,
		/// <summary>
		/// The gamma.
		/// </summary>
		Gamma, 
		/// <summary>
		/// The geometric.
		/// </summary>
		Geometric, 
		/// <summary>
		/// The laplace.
		/// </summary>
		Laplace, 
		/// <summary>
		/// The log normal.
		/// </summary>
		LogNormal,
		/// <summary>
		/// no distribution will be set
		/// </summary>
		None,
		/// <summary>
		/// The normal.
		/// </summary>
		Normal, 
		/// <summary>
		/// The pareto.
		/// </summary>
		Pareto, 
		/// <summary>
		/// The poisson.
		/// </summary>
		Poisson, 
		/// <summary>
		/// The power.
		/// </summary>
		Power, 
		/// <summary>
		/// The ray leigh.
		/// </summary>
		RayLeigh, 
		/// <summary>
		/// The students t.
		/// </summary>
		StudentsT,
		/// <summary>
		/// The triangular.
		/// </summary>
		Triangular, 
		/// <summary>
		/// The wei bull.
		/// </summary>
		WeiBull };
	/// <summary>
	/// Initializes a new instance of the <see cref="MeshXT2"/> class.
	/// while specifying the seed, generator and distribution to use as well as the 
	/// parameters for the distribution alpha corrosponds to p the amount of error in the 
	/// distribution generally smaller means less variance
	/// </summary>
	/// <param name="seed">Seed.</param>
	/// <param name="genx">Genx.</param>
	/// <param name="distx">Distx.</param>
	/// <param name="args">Arguments.</param>
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
		setGenerator(seed, genx);
		setDistribution(distx, args);
	}
	public Generator currentGenerator
	{
		get{return gen;}
		set 
		{
			gen = value;
		}
	}
	public Distribution currentDistribution
	{
		get{return dist;}
		set{dist = value;}
	}
	/// <summary>
	/// Sets the generator does not reset the generator the distribution is using to the new
	/// generator if only using a generator this is the only thing you need to call
	/// </summary>
	/// <param name="seed">Seed.</param>
	/// <param name="genx">Genx.</param>
	public void setGenerator(int seed, generators genx)
	{
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
	/// <summary>
	/// Sets the distribution for operations using the current genrator
	/// </summary>
	/// <param name="distx">Distx.</param>
	public void setDistribution(distributions distx, Dictionary<string, double> args)
	{
		//TODO check arguments to ensure they are making a change to the distribution 
		//otherwise throw an exception see laplace as a example of implementing this
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
				x7.Alpha = (int)args["alpha"];
				x7.Beta = (int)args["beta"];
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
				x8.Alpha = (int)args["alpha"];
				x8.Lambda = (int)args["lambda"];
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
				x10.Alpha = (int)args["alpha"];
				x10.Beta = (int)args["beta"];
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
		case distributions.Geometric:
			GeometricDistribution x13 = new GeometricDistribution(gen);
			if(args.ContainsKey("alpha"))
			{
				x13.Alpha = args["alpha"];
			}
			else
			{
				throw new System.Exception("Geometric distribution requires alpha value");
			}
			dist = x13;
			break;
		case distributions.Binomial:
			BinomialDistribution x14 = new BinomialDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("beta"))
			{
				x14.Alpha = args["alpha"];
				x14.Beta = (int)args["beta"];
			}
			else
			{
				throw new System.Exception("binomial distribution requires alpha and beta");
			}
			dist = x14;
			break;
		case distributions.None:
			break;
		case distributions.Laplace:
			LaplaceDistribution x15 = new LaplaceDistribution(gen);
			if(args.ContainsKey("alpha") && args.ContainsKey("mu"))
			{
				if(x15.IsValidAlpha(args["alpha"]) && x15.IsValidMu(args["mu"]))
				{
					x15.Alpha = args["alpha"];
					x15.Mu = args["mu"];
				}
				else throw new ArgumentException("alpha must be greater than zero");
			}
			else
			{
				throw new System.Exception("Laplace dist requires alpha and mu");
			}
			dist = x15;
			break;
		case distributions.LogNormal:
			LognormalDistribution x16 = new LognormalDistribution(gen);
			if(args.ContainsKey("mu")&& args.ContainsKey("sigma"))
			{
				x16.Mu = args["mu"];
				x16.Sigma = args["sigma"];
			}
			else
			{
				throw new System.Exception("lognormal distribution requires mu and sigma");
			}
			dist = x16;
			break;
		case distributions.Normal:
			NormalDistribution x17 = new NormalDistribution(gen);
			if(args.ContainsKey("mu") && args.ContainsKey("sigma"))
			{
				x17.Mu = args["mu"];
				x17.Sigma = args["sigma"];
			}
			else
			{
				throw new System.Exception("normal distribution requires mu and sigma");
			}
			dist = x17;
			break;
		case distributions.Pareto:
			ParetoDistribution x18 = new ParetoDistribution(gen);
			if(args.ContainsKey("alpha")&& args.ContainsKey("beta"))
			{
				x18.Alpha = args["alpha"];
				x18.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception("pareto distribution requires alpha and beta");
			}
			dist = x18;
			break;
		case distributions.Poisson:
			PoissonDistribution x19 = new PoissonDistribution(gen);
			if(args.ContainsKey("lambda"))
			{
				x19.Lambda = args["lambda"];
			}
			else
			{
				throw new System.Exception("Poisson distribution requires lambda");
			}
			dist = x19;
			break;
		case distributions.Power:
			PowerDistribution x20 = new PowerDistribution(gen);
			if(args.ContainsKey("alpha")&& args.ContainsKey("beta"))
			{
				x20.Alpha = args["alpha"];
				x20.Beta = args["beta"];
			}
			else
			{
				throw new System.Exception("Power dist requires alpha and beta");
			}
			dist = x20;
			break;
		case distributions.RayLeigh:
			RayleighDistribution x21 = new RayleighDistribution(gen);
			if(args.ContainsKey("sigma"))
			{
				x21.Sigma = args["sigma"];
			}
			else
			{
				throw new System.Exception("Rayleigh dist requires sigma");
			}
			dist = x21;
			break;
		case distributions.StudentsT:
			StudentsTDistribution x22 = new StudentsTDistribution(gen);
			if(args.ContainsKey("nu"))
			{
				x22.Nu = (int)args["nu"];
			}
			else
			{
				throw new System.Exception("StudentsT dist requirres nu");
			}
			dist = x22;
			break;
		case distributions.Triangular:
			TriangularDistribution x23 = new TriangularDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("beta")&& args.ContainsKey("gamma"))
			{
				x23.Alpha = args["alpha"];
				x23.Beta = args["beta"];
				x23.Gamma = args["gamma"];
			}
			else
			{
				throw new System.Exception("Triangular distribution requires alpha, beta and gamma");
			}
			dist = x23;
			break;
		case distributions.WeiBull:
			WeibullDistribution x24 = new WeibullDistribution(gen);
			if(args.ContainsKey("alpha")&&args.ContainsKey("lambda"))
			{
				x24.Alpha = args["alpha"];
				x24.Lambda = args["lambda"];
			}
			else
			{
				throw new System.Exception("WeiBull dist requires alpha and lambda");
			}
			dist = x24;
			break;
		default:
			throw new NotImplementedException("the distribution you want has not yet been implemented "+
			                                  "you could help everyone out by going and implementing it");
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
//	public void DeformByNormal(float min, float max)
//	{
//		for(int i = 0; i<vertices.Length ;i++)
//		{
//			Vector3 temp = normals[i]*gen.NextFloat(min, max);
//			for(int k=0; k<vertices.Length;k++)
//			{
//				if(i == k)
//					continue;
//				else if(vertices[i] == vertices[k])
//				{
//					vertices[k] += temp;
//				}
//			}
//			vertices[i] += temp;
//		}
//	}
	/// <summary>
	/// Deforms the by normal according to the distribution specified
	/// </summary>
	public void DeformByNormal()
	{
		if(dist == null)
		{
			throw new System.Exception("dist cannot be null when not specifying min and max");
		}
		for(int i = 0; i<vertices.Length ;i++)
		{
			Vector3 temp = normals[i]*(float)dist.NextDouble();
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
	public delegate float returnAFloat();
	/// <summary>
	/// Deforms the vertices by the funtion * normal
	/// should be one of the final deform by normal functions
	/// </summary>
	/// <param name="function">Function must retun a float value</param>
	public void DeformByNormal(returnAFloat function)
	{
		for(int i = 0; i<vertices.Length ;i++)
		{
			Vector3 temp = normals[i]*function();
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
	/// Deforms the by normal clamped between two values
	/// </summary>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
//	public void DeformByNormalClamped(float min, float max)
//	{
//		if(dist == null)
//		{
//			throw new System.Exception("dist cannot be null when not specifying min and max");
//		}
//		for(int i = 0; i<vertices.Length ;i++)
//		{
//			Vector3 temp = normals[i]*Mathf.Clamp((float)dist.NextDouble(),min, max);
//			for(int k=0; k<vertices.Length;k++)
//			{
//				if(i == k)
//					continue;
//				else if(vertices[i] == vertices[k])
//				{
//					vertices[k] += temp;
//				}
//			}
//			vertices[i] += temp;
//		}
//	}
	/// <summary>
	/// Deforms by normal scaled.
	/// </summary>
	/// <param name="scaleBy">Scale by.</param>
//	public void DeformByNormalScaled(float scaleBy)
//	{
//		if(dist == null)
//		{
//			throw new System.Exception("dist cannot be null when not specifying min and max");
//		}
//		for(int i = 0; i<vertices.Length ;i++)
//		{
//			Vector3 temp = normals[i]*scaleBy*(float)dist.NextDouble();
//			for(int k=0; k<vertices.Length;k++)
//			{
//				if(i == k)
//					continue;
//				else if(vertices[i] == vertices[k])
//				{
//					vertices[k] += temp;
//				}
//			}
//			vertices[i] += temp;
//		}
//	}
//	public void DeformByNormalFixedAmount(float amount)
//	{
//		if(dist == null)
//		{
//			throw new System.Exception("dist cannot be null when not specifying min and max");
//		}
//		for(int i = 0; i<vertices.Length ;i++)
//		{
//			Vector3 temp = normals[i]*amount;
//			for(int k=0; k<vertices.Length;k++)
//			{
//				if(i == k)
//					continue;
//				else if(vertices[i] == vertices[k])
//				{
//					vertices[k] += temp;
//				}
//			}
//			vertices[i] += temp;
//		}
//	}
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
	public void TesselateWithModifiedNormals(returnAFloat function)
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
				tempNormals[(i*12)+1] = normals[triangles[i*3]]*function();
				tempUv[(i*12)+1] = uvM2;

				tempVerts[(i*12)+2] = m1;
				tempTris[(i*12)+2] = (i*12)+2;
				tempNormals[(i*12)+2] = normals[triangles[i*3]]*function();
				tempUv[(i*12)+2] = uvM1;

				tempVerts[(i*12)+3] = vertices[triangles[(i*3)+1]];
				tempTris[(i*12)+3] = (i*12)+3;//triangles[(i*3)+1];
				tempNormals[(i*12)+3] = normals[triangles[(i*3)+1]];
				tempUv[(i*12)+3] = uv[triangles[(i*3)+1]];

				tempVerts[(i*12)+4] = m0;
				tempTris[(i*12)+4] = (i*12)+4;
				tempNormals[(i*12)+4] = normals[triangles[(i*3)+1]]*function();
				tempUv[(i*12)+4] = uvM0;

				tempVerts[(i*12)+5] = m2;
				tempTris[(i*12)+5] = (i*12)+5;
				tempNormals[(i*12)+5] = normals[triangles[(i*3)+1]]*function();
				tempUv[(i*12)+5] = uvM2;
				//triangle 3
				tempVerts[(i*12)+6] = vertices[triangles[(i*3)+2]];
				tempTris[(i*12)+6] = (i*12)+6;//triangles[(i*3)+2];
				tempNormals[(i*12)+6] = normals[triangles[(i*3)+2]];
				tempUv[(i*12)+6] = uv[triangles[(i*3)+2]];

				tempVerts[(i*12)+7] = m1;
				tempTris[(i*12)+7] = (i*12)+7;
				tempNormals[(i*12)+7] = normals[triangles[(i*3)+2]]*function();
				tempUv[(i*12)+7] = uvM1;

				tempVerts[(i*12)+8] = m0;
				tempTris[(i*12)+8] = (i*12)+8;
				tempNormals[(i*12)+8] = normals[triangles[(i*3)+2]]*function();
				tempUv[(i*12)+8] = uvM0;
				//triangle 4 last one
				tempVerts[(i*12)+9] = m0;
				tempTris[(i*12)+9] = (i*12)+9;
				tempNormals[(i*12)+9] = normals[triangles[(i*3)+1]]*function();
				tempUv[(i*12)+9] = uvM0;

				tempVerts[(i*12)+10] = m1;
				tempTris[(i*12)+10] = (i*12)+10;
				tempNormals[(i*12)+10] = normals[triangles[(i*3)+2]]*function();
				tempUv[(i*12)+10] = uvM1;

				tempVerts[(i*12)+11] = m2;
				tempTris[(i*12)+11] = (i*12)+11;
				tempNormals[(i*12)+11] = normals[triangles[(i*3)]]*function();
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
	public void ConvexTesselationBridging(float offset)
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

				Vector3 m0 = MathXT.midPoint(vertices[triangles[(i*3)+2]], vertices[triangles[(i*3)+1]])
					+ offset*normals[triangles[(i*3)+1]];
				Vector3 m1 = MathXT.midPoint(vertices[triangles[(i*3)]], vertices[triangles[(i*3)+2]])
					+ offset*normals[triangles[(i*3)+1]];
				Vector3 m2 = MathXT.midPoint(vertices[triangles[(i*3)]], vertices[triangles[(i*3)+1]])
					+ offset*normals[triangles[(i*3)+1]];

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
	/// <summary>
	/// returns a Deep copy of the current MeshXT2
	/// generators and distribution are not set
	/// </summary>
	/// <returns>The copy.</returns>
	public MeshXT2 DeepCopy()
	{
		var temp = new MeshXT2();
		temp.vertices = new Vector3[vertices.Length];
		for(int i=0; i<vertices.Length;i++)
		{
			temp.vertices[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
		}
		temp.triangles = new int[triangles.Length];
		for(int i=0; i< triangles.Length;i++)
		{
			temp.triangles[i] = triangles[i];
		}
		temp.normals = new Vector3[normals.Length];
		for(int i=0; i< normals.Length;i++)
		{
			temp.normals[i] = new Vector3(normals[i].x, normals[i].y, normals[i].z);
		}
		temp.name = new string(name.ToCharArray());
		temp.uv = new Vector2[uv.Length];
		for(int i=0; i<uv.Length;i++)
		{
			temp.uv[i] = new Vector2(uv[i].x, uv[i].y);
		}
		try
		{
			temp.mesh = mesh;
		}
		catch
		{}
		return temp;
	}
}
