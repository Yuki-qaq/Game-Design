using UnityEngine;
using System.Collections.Generic;

namespace com
{
    public class PoolingService : MonoBehaviour
    {
        public static PoolingService instance;
        private List<InstantiateData> _instantiateData;
        public List<PoolingData> data;

        void Awake()
        {
            instance = this;
            _instantiateData = new List<InstantiateData>();
            foreach (var d in data)
            {
                _instantiateData.AddRange(d.data);
            }
        }

        public GameObject GetInstance(string id)
        {
            var d = GetInstantiateData(id);
            if (d == null)
            {
                Debug.LogError("GetInstance null: " + id);
                return null;
            }

            return d.Get();
        }

        public void Recycle(GameObject go)
        {
            if (go == null || !go.activeInHierarchy)
            {
                return;
            }

            //Debug.Log("Recycle " + go);
            var id = "";
            var pi = go.GetComponent<PoolingInstance>();
            if (pi == null)
            {
                Debug.LogWarning(go + " Recycle PoolingInstance null!");
                Destroy(go);
                return;
            }
            id = pi.id;

            if (id == "")
            {
                Debug.LogError("Recycle id null!");
                return;
            }

            var d = GetInstantiateData(id);
            if (d == null)
            {
                Debug.LogError("Recycle instantiateData null!");
                return;
            }

            d.Recycle(pi);
        }

        public InstantiateData GetInstantiateData(string id)
        {
            foreach (var d in _instantiateData)
            {
                if (d.id == id)
                {
                    return d;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class InstantiateData
    {
        public string id;
        public GameObject prefab;
        public int count;

        private List<GameObject> activeInstances = new List<GameObject>();
        private List<GameObject> inactiveInstances = new List<GameObject>();


        private GameObject Create()
        {
            var go = GameObject.Instantiate(prefab, prefab.transform.parent);
            go.name = prefab.name + "-" + activeInstances.Count;
            go.AddComponent<PoolingInstance>();
            go.GetComponent<PoolingInstance>().id = id;
            return go;
        }

        private void LogList()
        {
            foreach (var a in activeInstances)
            {
                Debug.Log(a);
            }
            foreach (var i in inactiveInstances)
            {
                Debug.Log(i);
            }
        }

        public void Recycle(PoolingInstance pi)
        {
            if (pi == null)
            {
                Debug.LogError("Recycle PoolingInstance null!");
                return;
            }
            var go = pi.gameObject;
            if (go == null)
            {
                Debug.LogError("Recycle null!");
                return;
            }

            int index = activeInstances.IndexOf(go);
            if (index < 0)
            {

                Debug.LogWarning("Recycle not in list! id = " + pi.id);
                //GameObject.Destroy(go);
                //LogList();
                return;
            }

            if (activeInstances.Count + inactiveInstances.Count >= count)
            {
                activeInstances.RemoveAt(index);
                GameObject.Destroy(go);
                //Debug.Log("Recycle destroy!");
                return;
            }

            inactiveInstances.Add(go);
            activeInstances.RemoveAt(index);
            go.SetActive(false);
            //Debug.Log("Recycle disactive!");
        }

        public GameObject Get()
        {
            GameObject go;

            if (activeInstances.Count >= count)
            {
                if (inactiveInstances.Count > 0)
                {
                    go = inactiveInstances[0];
                    activeInstances.Add(go);
                    inactiveInstances.RemoveAt(0);
                    go.SetActive(true);
                    return go;
                }
            }

            go = Create();
            go.SetActive(true);
            activeInstances.Add(go);
            return go;
        }
    }
}