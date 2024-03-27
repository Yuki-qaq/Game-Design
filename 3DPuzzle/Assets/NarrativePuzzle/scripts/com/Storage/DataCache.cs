using UnityEngine;

namespace com
{
    public class DataCache<T> : DataCacheBase where T : class, new()
    {
        public T cache { get; protected set; }

        public override void Load()
        {
            try
            {
                cache = StorageService.instance.LoadDataCache<T>(fullKey);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                cache = null;
            }

            if (cache == null)
                Reset();
        }

        public override void Reset()
        {
            //Debug.Log("reset");
            cache = new T();
        }

        protected override void DoSave()
        {
            //Debug.Log("DoSave " + fullKey);
            StorageService.instance.SaveDataCache<T>(fullKey, cache);
        }
    }
}