using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice; 
namespace DynamicMeshCutter
{
	//todo：BundleLoader.Instance.Prefabs need to be addressed when BundleLoader is disabled
	
	//ReactAddPrefab->addPrefabAndBowl(#1)->add a list of gameobjects which include the foods and maybe the bowl to listoflist
	//when BundleLoader_loaded flag is on(#2), call addPrefabAndBowl on every foods inside mylist2(#3)
	//if build is true, then it is to be installed as a component on the app, so things like "outlines" should be disabled, as 
	//they are used for debugging (#4)
public class gamecontroller : MonoBehaviour
{

	// Singleton instance
	public static gamecontroller Instance { get; private set; }
	public bool build;
	public List<GameObject> currentlist;
	private bool bundleloader_enabled=false;
	public Dictionary<string, GameObject> Prefabs;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one instance of col found!");
			return;
		}

		Instance = this;
		build=true;//#4
		// Initialization of other variables...
	}
	public void ReactAddPrefab(string name){
		if ((BundleLoader.Instance?.IsLoaded ?? false))
		{	
			if(BundleLoader.Instance.Prefabs.ContainsKey(name)){
				Debug.Log("addPrefabAndBowl "+name);
				addPrefabAndBowl(BundleLoader.Instance.Prefabs[name]);
			} else {
				Debug.Log(name+" is not contained in Prefabs");
			}
		}
	}
	private bool checkScript(){
		BundleLoader scriptToCheck = GetComponent<BundleLoader>();

		if (scriptToCheck != null)
		{
			if (scriptToCheck.enabled)
			{
				Debug.Log("BundleLoader is enabled");
				return true;
			}
			else
			{
				Debug.Log("BundleLoader is not enabled");
				return false;
			}
		}
		else
		{
			Debug.Log("The target GameObject does not have BundleLoader attached!");
			return false;
		}
	}
	public void ReactConnected(){
		Debug.Log("unity connected");
	}
	public void ReactSlice(int n){
		MouseBehaviour.Instance.Slice(n);
	}
	public void ReactScroll (int index) {
		if(index!=1&&index!=-1){
			Debug.Log("index should be 1 or -1");
			return;
		}
		cameraIndex=cameraIndex+index;
		if(cameraIndex<0){
			cameraIndex=0;
		}
		if(cameraIndex>positions.Count-1){
			cameraIndex=positions.Count-1;
		}
		if(listOfLists[cameraIndex].Count==0){
			Debug.Log("scrolling to an index with empty list");
			return;
		}
		Debug.Log("unity ReactScroll:"+index);
			//rb = bowls[cameraIndex].GetComponent<Rigidbody>();
			Transform cameraTransform = Camera.main.gameObject.transform;
			RemoveAllOutlines(currentlist);
			currentlist=listOfLists[cameraIndex];
			AddAllOutlines(currentlist);
			
			
			// Set the position and rotation of the camera
			cameraTransform.position = new Vector3((float)positions[cameraIndex], (float)0.746, (float)0.514);
			cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
	}
	//below is a method that has the same purpose with ReactScroll
	public void ReactScrollTo (int index) {
		if(index<0||index>positions.Count-1){
			Debug.Log("index out of range:"+index);
			return;
		}
		cameraIndex=index;
		Debug.Log("ReactScrollTo index:"+index);
			//rb = bowls[cameraIndex].GetComponent<Rigidbody>();
			Transform cameraTransform = Camera.main.gameObject.transform;
			RemoveAllOutlines(currentlist);
			currentlist=listOfLists[cameraIndex];
			AddAllOutlines(currentlist);
			
			
			// Set the position and rotation of the camera
			cameraTransform.position = new Vector3((float)positions[cameraIndex], (float)0.746, (float)0.514);
			cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
	}
	public void ReactPut (int index) {
		if(index<0||index>positions.Count-1||index==cameraIndex){
			Debug.Log("index is wrong:"+index);
			return;
		}
		PutToBowl(index);
	}
	public static void AddOutline(GameObject target)
	{
		if(target != null) // check if the target is not null
    {
		Outline outline = target.GetComponent<Outline>();
		if(outline==null)
			outline=target.AddComponent<Outline>();
		outline.eraseRenderer=false;
	}
	else 
    {
        Debug.Log("Attempted to remove outline from a null GameObject.");
    }

	}
	public static void RemoveOutline(GameObject target)
	{
		if(target != null) // check if the target is not null
		{
			Outline outline = target.GetComponent<Outline>();
			if(outline==null)
				outline=target.AddComponent<Outline>();
			outline.eraseRenderer=true;
		}
		else 
		{
			Debug.Log("Attempted to remove outline from a null GameObject.");
		}

	}
	
		bool hasLoaded;		
    // Start is called before the first frame update
		string[] prefabnames;
		string[] mylist2;//#3
		Dictionary<string, GameObject> prefabDictionary;

		public float TurnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
		public float MoveSpeed = 4.0f;      // Speed of the camera going back and forth
		private List<float> positions;
		private List<GameObject> instances; 	
		List<List<GameObject>> listOfLists;
		private List<GameObject> bowls; 	
		public Queue<GameObject> qCutmeshes;
		GameObject bowl;
		GameObject pan;
		GameObject prefab;
		Rigidbody rb;


		private Vector3 direction = Vector3.zero;
		private int animationIndex = -1;
		
		private string animationName;
		
		private Dictionary<string, List<List<GameObject>>> animationObjectLists;
		private Dictionary<string, List<float>> rotateDegrees;
		private Dictionary<string, List<Vector3>> animationRotateAxiss;
		private Dictionary<string, List<float>> animationTimes;
		private float timer=0f;
		private float yaw = 0f;
		private float pitch = 0f;


		private int cameraIndex=0;
		public void TeleportGameObjects(List<GameObject> gameObjects, Vector3 newLocation)
		{
			if(gameObjects.Count == 0)
			{
				Debug.Log("No gameObjects to teleport.");
				return;
			}
			GameObject referenceObject = null;
			Vector3 oldLocation = Vector3.zero;
			foreach(GameObject go in gameObjects)
			{
				// Get the difference between the GameObject's position and the old location
				if(go!=null){
					referenceObject=go;
					oldLocation= referenceObject.transform.position;
					break;
				}
			}
			if(referenceObject==null){
				Debug.Log("All gameobjs in currentlist are null");
				return;
			}
			foreach(GameObject go in gameObjects)
			{
				if(go!=null){
				// Get the difference between the GameObject's position and the old location
				Vector3 diff = go.transform.position - oldLocation;

				// Apply that difference to the new location
				go.transform.position = newLocation + diff;

				}
			}
		}

		//(0,-0.432,2.225)
		public void addPrefabAndBowl(GameObject prefab){//#1
		
			List<GameObject> list1 = new List<GameObject>();
			var position = 2.722f;
			if(positions.Count!=0){
				position=positions[positions.Count-1]+0.6f;
			}
			positions.Add(position);
			if(prefab==null){
				Debug.Log("prefab is null");
			} else {
			GameObject instance = Instantiate(prefab, new Vector3(position,-0.238f,2.225f), Quaternion.Euler(-89.98f,0f,180f));
			
            list1.Add(instance);
			}
			listOfLists.Add(list1);
			GameObject bowladded = Instantiate(bowl, new Vector3(position,-0.419f,2.225f), Quaternion.Euler(-89.98f,0f,180f));
			
			bowls.Add(bowladded);
			
		}
		void addAnimation( string name,float time,int index=99999,GameObject obj=null, float rotateDegree=0f,Vector3 rotateAxis= default){
			
			if(!animationTimes.ContainsKey(name)){//init if the animation is new
				rotateDegrees.Add(name,new List<float>());
				animationObjectLists.Add(name,new List<List<GameObject>>());
				animationRotateAxiss.Add(name,new List<Vector3>());
				animationTimes.Add(name,new List<float>());
			}
			if(index>=animationTimes[name].Count){
				rotateDegrees[name].Add(rotateDegree);
				animationObjectLists[name].Add(new List<GameObject>());
				//animationObjectLists[name][animationObjectLists[name].Count-1].Add(obj);
				animationRotateAxiss[name].Add(rotateAxis);
				animationTimes[name].Add(time);
			}
			if(index<=0){
				rotateDegrees[name].Insert(0, rotateDegree);
				animationObjectLists[name].Insert(0, new List<GameObject>());
				//animationObjectLists[name][animationObjectLists[name].Count-1].Insert(0, obj);
				animationRotateAxiss[name].Insert(0, rotateAxis);
				animationTimes[name].Insert(0, time);
			}
		}
		void Start()
		{
			bundleloader_enabled=checkScript();
		hasLoaded = false;
		Debug.Log("hasLoaded set to false");

		positions=new List<float>();
		Debug.Log("positions initialized");

		bowls=new List<GameObject>();
		Debug.Log("bowls initialized");

		listOfLists = new List<List<GameObject>>();
		Debug.Log("listOfLists initialized");
		mylist2=new string[] {"carrot","potato"};//#3
		Debug.Log("mylist2:"+mylist2);
		Debug.Log("mylist2 initialized");
		/*
		string[]  prefablist = new string[] {"carrot","potato","cabbage","chicken_breast","steak","tomato","bowl"};
		Prefabs = new Dictionary<string, GameObject>();
		foreach(string a in prefablist)
		{
			GameObject toadd = Resources.Load<GameObject>(a);
			if(toadd!=null){
				Debug.Log("Added from resources"+toadd);
			Prefabs.Add(a,toadd);
			}
		}
		if(!build){

			foreach(string a in mylist2){
				if(Prefabs.ContainsKey(a)){
					Debug.Log("haha");
					addPrefabAndBowl(Prefabs[a]);
				} else {
					Debug.Log("not contained "+a);
				}
			}
		}
		*/
		bowl = Resources.Load<GameObject>("bowl");
		Debug.Log("bowl loaded");
		//pan=Resources.Load<GameObject>("pan");
		var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		Debug.Log("roots retrieved");

		//currentlist=listOfLists[0];
		Debug.Log("currentlist set");

		rotateDegrees=new Dictionary<string, List<float>>();
		Debug.Log("rotateDegrees initialized");

		animationObjectLists=new Dictionary<string, List<List<GameObject>>>();
		Debug.Log("animationObjectLists initialized");

		animationRotateAxiss=new Dictionary<string, List<Vector3>>();
		Debug.Log("animationRotateAxiss initialized");

		animationTimes=new Dictionary<string, List<float>>();
		Debug.Log("animationTimes initialized");

		addAnimation(time:0.6f,name:"put",rotateDegree:130f,rotateAxis:Vector3.forward);
		Debug.Log("First animation added");

		addAnimation(time:0.3f,name:"put",rotateDegree:-130f,rotateAxis:Vector3.forward);
		addAnimation(time:-1f,name:"put",rotateDegree:-130f,rotateAxis:Vector3.forward);
		Debug.Log("Second animation added");
		}

		private void FixedUpdate()
		{
			if(animationIndex!=-1){
				
				timer += Time.fixedDeltaTime;
				Debug.Log(animationName+animationIndex);
				Debug.Log(animationTimes==null);
				//Debug.Log("b"+animationTimes[animationName][animationIndex]);
				if(animationTimes[animationName][animationIndex]<0){
					Destroy(rb.gameObject);
					animationIndex++;
					timer=0f;
					if(animationIndex==animationObjectLists[animationName].Count){
						Debug.Log("end of animation");
						animationIndex=-1;
						return;
					}
					
				}
				if(timer>=animationTimes[animationName][animationIndex]){
					animationIndex++;
					timer=0f;
					if(animationIndex==animationObjectLists[animationName].Count){
						Debug.Log("end of animation");
						animationIndex=-1;
						return;
					}
					
				}
				//rb=animationObjectLists[animationName][animationIndex][0].GetComponent<Rigidbody>();//init
				float rotationSpeed=rotateDegrees[animationName][animationIndex]/animationTimes[animationName][animationIndex];//init

				Vector3 rotationAxis = transform.TransformDirection(animationRotateAxiss[animationName][animationIndex]); 
				
				Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * Time.fixedDeltaTime, rotationAxis);

				//(270.16, 277.12, 262.88)=(x,...,...) x++, when x==360,x=0,when x==0, bowl is D
				rb.rotation = rotation * rb.rotation;
				//Debug.Log(rb.rotation.eulerAngles.x);//the logged result is (-0.58886, 0.39124, 0.39138, -0.58907)
				
				
			}
		}

		void RemoveAllOutlines(List<GameObject> list){
			if(build){//#4
				return;
			}
			foreach (GameObject obj in list){
				RemoveOutline(obj);
			}
		}
		void AddAllOutlines(List<GameObject> list){
			if(build){//#4
				return;
			}
			foreach (GameObject obj in list){
				AddOutline(obj);
			}
		}
		void PutToBowl(int toIndex){
			currentlist.Add(bowls[cameraIndex]);
			TeleportGameObjects(currentlist,new Vector3((float)(positions[toIndex]+0.06), (float)-0.16, (float)2.225));
			currentlist.RemoveAt(currentlist.Count - 1);
			Transform cameraTransform = Camera.main.gameObject.transform;
			cameraTransform.position = new Vector3((float)(positions[toIndex]), (float)0.746, (float)0.514);
			cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
			rb=bowls[cameraIndex].GetComponent<Rigidbody>();
			foreach (GameObject item in currentlist)
			{
				listOfLists[toIndex].Add(item);
			}

			animationName="put";
			animationIndex=0;
			
			RemoveAllOutlines(currentlist);
			currentlist=listOfLists[toIndex];
			AddAllOutlines(currentlist);
			cameraIndex=toIndex;
			}
		void Update()
		{
			if (!hasLoaded){
				if(bundleloader_enabled==false){
					// Search for the prefab in assets.
					for (int i = 0; i < mylist2.Length;i++)
					{ 
						GameObject prefab = Resources.Load<GameObject>(mylist2[i]);
						addPrefabAndBowl(prefab);
					}
					hasLoaded = true;

				} else if(BundleLoader.Instance?.IsLoaded ?? false)//flag bundleloader #2
				{

					for (int i = 0; i < mylist2.Length; i++)
					{
						
						if(BundleLoader.Instance.Prefabs[mylist2[i]]!=null){
							addPrefabAndBowl(BundleLoader.Instance.Prefabs[mylist2[i]]);
						} else {
							Debug.Log(mylist2[i]+" is null");
						}
						if(i==0){
							//set the camera to look at the first position
							Transform cameraTransform = Camera.main.gameObject.transform;
							cameraTransform.position = new Vector3((float)positions[0], (float)0.746, (float)0.514);
							cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
							cameraIndex=0;
										
							currentlist=listOfLists[cameraIndex];
						}
					}
					hasLoaded = true;
				}
			}
			

			var camera = Camera.main.gameObject.transform;

			 direction = Vector3.zero;

        // Check for key press and determine direction
			if (Input.GetKey(KeyCode.UpArrow))
			{
				direction = Vector3.right;
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				direction = Vector3.left;
			}
			
			if (Input.GetKey(KeyCode.O))
            {    
				Transform cameraTransform = Camera.main.gameObject.transform;
        		// Set the position and rotation of the camera
				cameraTransform.position = new Vector3((float)1.135, (float)0.746, (float)0.514);
				cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
				Debug.Log("O");
			}
			if (Input.GetKeyDown(KeyCode.P))
            {
				PutToBowl(cameraIndex+1);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
            {     
				Debug.Log(cameraIndex);
				if(cameraIndex<(positions.Count-1)){
					cameraIndex=cameraIndex+1;
					Debug.Log(cameraIndex);
				}

				//rb = bowls[cameraIndex].GetComponent<Rigidbody>();
				Transform cameraTransform = Camera.main.gameObject.transform;
				RemoveAllOutlines(currentlist);
				currentlist=listOfLists[cameraIndex];
				AddAllOutlines(currentlist);
				
				foreach (GameObject item in currentlist)
				{
					Debug.Log(item);
				}
        		// Set the position and rotation of the camera
				cameraTransform.position = new Vector3((float)positions[cameraIndex], (float)0.746, (float)0.514);
				cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
				//Debug.Log("P");
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
            {   
				//Debug.Log(rb);
				if(cameraIndex>0){
					cameraIndex=cameraIndex-1;
					Debug.Log(cameraIndex);
				}
				rb = bowls[cameraIndex].GetComponent<Rigidbody>();
				Transform cameraTransform = Camera.main.gameObject.transform;
				RemoveAllOutlines(currentlist);
				currentlist=listOfLists[cameraIndex];
				AddAllOutlines(currentlist);
        		// Set the position and rotation of the camera
				cameraTransform.position = new Vector3((float)positions[cameraIndex], (float)0.746, (float)0.514);
				cameraTransform.rotation = Quaternion.Euler((float)30.72, (float)0,(float)0);
				//Debug.Log("P");
				
			}
			if (Input.GetMouseButton(2))
			{
				yaw += Input.GetAxis("Mouse X");
				pitch -= Input.GetAxis("Mouse Y");
				camera.eulerAngles = new Vector3(TurnSpeed * pitch, TurnSpeed * yaw, 0.0f);
			}
			if (Input.GetKeyDown(KeyCode.C))
            {
                //Slice(3);
				Debug.Log("C pressed");
                ReactSlice(4);
                //_isDragging = false;
            }
			if (Input.GetMouseButton(0))
			{
                ReactSlice(4);
			}
			/*
			if (Input.GetMouseButtonUp(0))
    {
        isMouseClicked = false;
    }
	*/
		}

}
}