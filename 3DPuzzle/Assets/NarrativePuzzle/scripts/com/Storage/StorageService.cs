using UnityEngine;

namespace com
{
    public class StorageService : MonoBehaviour
    {
        public static StorageService instance;

        public enum StorageType
        {
            PlayerPrefs,
            EncryptedPrefs,
            EncryptedFile,
        }

        public StorageType storageType;
        public IStorageService storageService { get; private set; }

        void Awake()
        {
            instance = this;

            switch (storageType)
            {
                case StorageType.EncryptedFile:
                    storageService = new EncryptedFileStorage();
                    break;

                case StorageType.EncryptedPrefs:
                    storageService = new EncryptedPrefsStorage();
                    break;

                case StorageType.PlayerPrefs:
                    storageService = new PlayerPrefsStorage();
                    break;
            }
        }

        T Load<T>(string key, T defaultValue)
        {
            string loaded = storageService.GetString(key, null);
            return loaded == null ? defaultValue : JsonUtility.FromJson<T>(loaded);
        }

        public T LoadDataCache<T>(string fullKey) where T : class
        {
            return Load<T>(fullKey, null);
        }

        public void SaveDataCache<T>(string fullKey, T data) where T : class
        {
            var s = JsonUtility.ToJson(data);
            //Debug.Log(s);
            storageService.SetString(fullKey, s);
        }
    }
}