using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace DynamicMeshCutter
{

    [RequireComponent(typeof(LineRenderer))]
    public class MouseBehaviour : CutterBehaviour
    {
        public static MouseBehaviour Instance { get; private set; } 

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Optionally keep this object alive when changing scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public LineRenderer LR => GetComponent<LineRenderer>();
        private Vector3 _from;
        private Vector3 _to;
        private bool _isDragging;

        protected override void Update()
        {
            base.Update();

/*            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;

                var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.05f);
                _from = Camera.main.ScreenToWorldPoint(mousePos);
            }

            if (_isDragging)
            {
                var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.05f);
                _to = Camera.main.ScreenToWorldPoint(mousePos);
                VisualizeLine(true);
            }
            else
            {
                VisualizeLine(false);
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                Cut();
                _isDragging = false;

            }
            */
        }
        public void SliceByAxis(int n,GameObject target){
            while (GameObject.Find("plane")!=null){
                GameObject gameObject=GameObject.Find("plane");
                Destroy(gameObject,0);
            }
            if (n<2){
                //Debug.Log("n smaller than 2");
                return;
            }
            
            Transform axis = target.transform.Find("axis");
            Transform axis_point_start = axis.transform.GetChild(0);
            Transform axis_point_end = axis.transform.GetChild(1);
            Vector3 differenceVector = axis_point_end.position - axis_point_start.position;

            List<GameObject> currentlist = gamecontroller.Instance.currentlist;
            //Debug.Log(target.transform.childCount);
            currentlist.Add(target.transform.GetChild(target.transform.childCount-1).gameObject);
            int index = -1;
            for (int i = 0; i < currentlist.Count; i++)
            {
                if(currentlist[i] != null){
                    index=i;
                    break;
                }
            }
            if(index==-1) {
                throw new Exception("no gameobject found in currentlist");
            }
           
            //Debug.Log("onCut"+"min:"+minx+"max:"+maxx);
            Vector3 deltaV3=differenceVector/5;
            //Plane[] planes=new Plane[n-1];
            for (int i = 0; i < n-1; i++)
            {   
                Vector3 point=axis_point_start.position+deltaV3*(i+1);
                Vector3 normal=deltaV3;
                AddCutPlane(point,new Vector3(normal.x*Mathf.Cos(Mathf.PI/2/2)-Mathf.Sin(Mathf.PI/2/2)*normal.z,normal.y,Mathf.Sin(Mathf.PI/2/2)*normal.x+Mathf.Cos(Mathf.PI/2)*normal.z));
                        //StartCoroutine(WaitOneFrame());
                //CreatePlane(new Vector3(minx+distance*(i+1),miny,minz),new Vector3(minx+distance*(i+1),miny,maxz),
                //new Vector3(minx+distance*(i+1),maxy,minz),new Vector3(minx+distance*(i+1),maxy,maxz));
            }
            
        }
        private void Cut()
        {
            Plane plane = new Plane(_from, _to, Camera.main.transform.position);

            List<GameObject> currentlist = gamecontroller.Instance.currentlist;
            foreach (GameObject root in currentlist)//
            {
                //Debug.Log("tag:"+root.tag+root);
                if(root==null)
                    continue;
                if (!root.activeInHierarchy)
                    continue;
                var targets = root.GetComponentsInChildren<MeshTarget>();
                foreach (var target in targets)
                { 
                    Cut(target, _to, plane.normal, null,onCreated);
                }
                
            }
        }

            
        private void VisualizeLine(bool value)
        {
            if (LR == null)
                return;

            LR.enabled = value;

            if (value)
            {
                LR.positionCount = 2;
                LR.SetPosition(0, _from);
                LR.SetPosition(1, _to);
            }
        }

    }
}
