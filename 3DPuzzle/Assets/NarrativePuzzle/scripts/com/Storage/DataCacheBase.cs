using UnityEngine;

namespace com
{
    public class DataCacheBase
    {
        public string fullKey;
        public string accountId;
        public StorageKey.Key key;
        bool _toSave;

        public void Setup(string pAccountId, StorageKey.Key pKey)
        {
            accountId = pAccountId;
            key = pKey;
            fullKey = StorageKey.GetFullKey(accountId, key);

            _toSave = false;
        }

        public void Setup(StorageKey.Key pKey)
        {
            Setup("", pKey);
        }

        public virtual void Load()
        {
        }

        public virtual void Reset()
        {
        }

        public void Save(bool instant = false)
        {
            if (instant)
            {
                DoSave();
            }
            else
            {
                _toSave = true;
            }
        }


        public void Tick()
        {
            if (_toSave)
            {
                _toSave = false;
                DoSave();
            }
        }

        protected virtual void DoSave()
        {

        }
    }
}