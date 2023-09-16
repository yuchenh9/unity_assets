using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cakeslice;

namespace DynamicMeshCutter
{
    public static class Select
    {
        public static List<GameObject> selected = new List<GameObject>();
        public static List<GameObject> selectedNext = new List<GameObject>();
        public static void Toggle(GameObject target)
        {
            if (target.tag=="Untagged")
            {   
                Outline outline = target.GetComponent<Outline>();
                if(outline==null)
                    outline=target.AddComponent<Outline>();
                outline.eraseRenderer=false;
                target.tag="0";
            } else {
                Outline outline = target.GetComponent<Outline>();
                if(outline==null)
                    outline=target.AddComponent<Outline>();
                outline.eraseRenderer=true;
                target.tag="Untagged";
            }
            //Debug.Log("Toggle:"+target.tag);

        }
        public static void Add(GameObject target)
        {
        
                Outline outline = target.GetComponent<Outline>();
                if(outline==null)
                    outline=target.AddComponent<Outline>();
                outline.eraseRenderer=true;
                target.tag="Untagged";

            //Debug.Log("Add");

        }
        public static void AddNext(GameObject target)
        {


        }

        public static void Remove(GameObject target)
        {


        }
        public static void Clear(){
        }
    }
}
