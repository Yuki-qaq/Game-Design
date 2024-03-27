using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace com
{
    public class EncryptedFileStorage : StorageBase
    {
        public const string StringListsKey   = "__meta_string_lists";
        public const string IntsKey          = "__ints";
        public const string cloudFileName   = "cloudData.dat";
        public const string localFileName   = "localData.dat";

        protected EncryptedSaveWorker  _saveWorker;

        //private UpdateManager _updateManager;
        //
        // protected CloudSaveService _cloudSaveService;
        //
        // protected ICloudFileProvider _cloudProvider;

        private bool _isCloudSync;

       
        private void Init()
        {
            // _cloudProvider.OnCloudLoaded += HandleCloudLoaded;
            CreateSaveWorker();
            SetFilePaths();
            Load();
        }

        public virtual void CreateSaveWorker()
        {
            _saveWorker = new EncryptedSaveWorker(_storage, _dontSyncKeys, _stringLists, _objects, _ints);
        }

        protected virtual void SetFilePaths()
        {
            LocalFilePath = Path.Combine( Application.persistentDataPath, localFileName );
            CloudFilePath = Path.Combine( Application.persistentDataPath, cloudFileName );
        }

        public static string LocalFilePath
        {
            get;
            protected set;
        }

        public static string CloudFilePath
        {
            get;
            protected set;
        }

        public override void Clear()
        {
            base.Clear();
            _dontSyncKeys.Clear();
            _stringLists.Clear();
            _ints.Clear();
            DeleteLocal( LocalFilePath );
            DeleteLocal( CloudFilePath );
        }

        private void HandleCloudLoaded( string data )
        {
            bool firstDevice = IsFirstDevice( data );
            LogHandleCLoudLoaded( data, firstDevice );
            if (firstDevice){//( _cloudSaveService.IsSyncCompleted() || firstDevice ) {
                Debug.Log( "Cloud save loaded: " + data );
                LoadLocal( LocalFilePath );
                LoadLocal( CloudFilePath );
                HandleLoad( data, true );
                FinishLoadProcess();

                if ( firstDevice ) {
                    // _cloudSaveService.SyncThisDevice();
                    Save();
                    // _cloudSaveService.SetSyncCompleted();
                    LogSyncCompleted( data );
                }

                _isCloudSync = false;
            }
        }

        private bool IsFirstDevice( string data )
        {
            return string.IsNullOrEmpty(data);//( !_cloudSaveService.IsSyncCompleted() && !_cloudSaveService.IsDeviceSynced() ) && string.IsNullOrEmpty( data ) && !_cloudProvider.IsTimeout();
        }

        protected virtual void LoadLocal( string filePath )
        {
            if ( !File.Exists( filePath ) )
                return;
            Debug.Log( "Load Local file: " + filePath );
            HandleLoad( File.ReadAllText( filePath ) );
        }

        protected virtual void DeleteLocal( string filePath )
        {
            File.Delete( filePath );
        }

        private void LogHandleCLoudLoaded( string data, bool firstDevice )
        {
            // Debug.Log( "sync completed: " + _cloudSaveService.IsSyncCompleted() + " device synced: " + _cloudSaveService.IsDeviceSynced() + " cloud load result: " + data );
            Debug.Log( "is first device: " + firstDevice );
        }

        private void LogSyncCompleted( string data )
        {
            Debug.Log( "After load and sync first device" );
            // Debug.Log( "sync completed: " + _cloudSaveService.IsSyncCompleted() + " device synced: " + _cloudSaveService.IsDeviceSynced() + " cloud load result: " + data );
        }

        protected virtual IDictionary GetDictionary( string data )
        {
            return MiscUtil.DeserializeAndDecrypt(data, "xoaWHL]-") as IDictionary;
        }

        protected void HandleLoad( string toDeserialize, bool isLoadRemote = false )
        {
            var dict = GetDictionary( toDeserialize );
            if ( dict == null ) {
                Debug.LogError( "Error deserializing prefs, probably empty" );
                return;
            }
            if (!_isCloudSync && isLoadRemote ) {
                // _cloudSaveService.CheckAndSyncDevice( dict[StorageKey] as Dictionary<string, object> );
                return;
            }
            foreach ( string key in dict.Keys ) {
                _storage[key] = dict[key];
            }
            var storageDict = dict[StorageKey] as IDictionary;
            if ( storageDict != null ) {
                foreach ( string key in storageDict.Keys ) {
                    _storage[key] = storageDict[key];
                }
            }
            ListToSet( dict[StringListsKey] as List<object>, _stringLists );
            ListToSet( dict[ObjectsKey] as List<object>, _objects );
            ListToSet( dict[IntsKey] as List<object>, _ints );
            Debug.Log( "Succefully loaded data" );
        }

        protected void ListToSet( List<object> rawList, HashSet<string> set )
        {
            if ( rawList == null )
                return;
            set.Clear();
            foreach ( var listKey in rawList ) {
                set.Add( listKey.ToString() );
            }
        }

        protected void FinishLoadProcess()
        {
            Debug.Log( "Finishing Load Process" );
            ConvertLists();
            ConvertInts();
            UnboxObjects();
            Loaded.Invoke();
            System.GC.Collect();
            if ( _isCloudSync ) {
                // _cloudSaveService.SyncThisDevice( true );
                Save();
            }
        }

        protected void ConvertLists()
        {
            foreach ( var listKey in _stringLists ) {
                if ( _storage[listKey] != null ) {
                    try {
                        _storage[listKey] = ( ( List<object> )_storage[listKey] ).OfType<string>().ToList();
                    } catch ( Exception e ) {
                        Debug.LogException( e );
                    }
                }
            }
        }

        protected void ConvertInts()
        {
            foreach ( var intKey in _ints ) {
                if ( _storage.ContainsKey( intKey ) ) {
                    _storage[intKey] = Convert.ToInt32( _storage[intKey] );
                }
            }
        }

        public override bool GetBool( string key, bool defaultValue )
        {
            return GetObject( key, defaultValue );
        }

        public override float GetFloat( string key, float defaultValue )
        {
            return GetObject( key, defaultValue );
        }

        public override int GetInt( string key, int defaultValue )
        {
            return GetObject( key, defaultValue );
        }

        public override long GetLong( string key, long defaultValue )
        {
            return GetObject( key, defaultValue );
        }

        public override string GetString( string key, string defaultValue )
        {
            return GetObject( key, defaultValue );
        }

        public override List<string> GetStringList( string key )
        {
            return GetClassObject<List<string>>( key, null );
        }

        public override bool HasKey( string key )
        {
            return _storage.ContainsKey( key );
        }

        public override void Load( bool isCloudSync = false )
        {
            _isCloudSync = isCloudSync;
            if ( !isCloudSync ) {
                Debug.Log( "!Instance._isCloudSync = " + ( !isCloudSync ) );
                LoadLocal( LocalFilePath );
                LoadLocal( CloudFilePath );
                FinishLoadProcess();
            }
            // _cloudProvider.Load();
        }

        protected override void Save( bool immiediate )
        {
            Saving.Invoke();
            //_updateManager.StartCoroutine( SaveWorkerCoroutine( immiediate ) );
        }

        protected IEnumerator SaveWorkerCoroutine( bool isBlocking )
        {
            if ( isBlocking ) {
                _saveWorker.RunBlocking();
            } else {
                _saveWorker.RunThreaded();
            }
            while ( _saveWorker.isRunning )
                yield return null;
            // if ( _cloudSaveService.IsDeviceSynced() ) {
            //     _cloudProvider.Save( _saveWorker.cloudStorageEncrypted );
            // }
        }

        public override void SetBool( string key, bool value )
        {
            _storage[key] = value;
        }

        public override void SetDontSync( string key )
        {
            _dontSyncKeys.Add( key );
        }

        public override void SetFloat( string key, float value )
        {
            _storage[key] = value;
        }

        public override void SetInt( string key, int value )
        {
            _ints.Add( key );
            _storage[key] = value;
        }

        public override void SetLong( string key, long value )
        {
            _storage[key] = value;
        }

        public override void SetString( string key, string value )
        {
            _storage[key] = value;
        }

        public override void SetStringList( string key, List<string> value )
        {
            _stringLists.Add( key );
            _storage[key] = value;
        }
    }
}
