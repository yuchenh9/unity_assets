using System.Collections.Generic;
using UnityEngine;

namespace DynamicMeshCutter
{
    public class gamecontroller : MonoBehaviour
    {
        // Singleton instance
        public static gamecontroller Instance { get; private set; }

        public List<GameObject> currentlist;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
