using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace com
{
    public class EncryptedSaveWorker
    {
        public volatile bool isRunning;
        public volatile bool isSuccess;

        protected Dictionary<string, object>        _storage;
        protected HashSet<string>                   _dontSyncKeys;
        protected HashSet<string>                   _stringLists;
        protected HashSet<string>                   _objects;
        protected HashSet<string>                   _ints;

        public string                           cloudStorageEncrypted;

        protected Dictionary<string, object>      _cloudStorage;
        protected Dictionary<string, object>      _localStorage;
        protected Dictionary<string, object>      _bufferDict;

        public EncryptedSaveWorker( Dictionary<string, object> storage,
                                    HashSet<string> dontSyncKyes,
                                    HashSet<string> stringLists,
                                    HashSet<string> objects,
                                    HashSet<string> ints )
        {
            _storage = storage;
            _dontSyncKeys = dontSyncKyes;
            _stringLists = stringLists;
            _objects = objects;
            _ints = ints;

            _cloudStorage = new Dictionary<string, object>();
            _localStorage = new Dictionary<string, object>();
            _bufferDict = new Dictionary<string, object>();
        }

        public void RunThreaded()
        {
            isRunning = true;
            try {
                SplitKeysAndBoxObjects();
                new Thread( ThreadedEncryptAndWrite ).Start();
            } catch ( System.Exception e ) {
                isSuccess = false;
                isRunning = false;
                Debug.LogError( "Exception caught: " + e.Message + ", stack trace: " + e.StackTrace );
            }
        }

        protected void ThreadedEncryptAndWrite()
        {
            try {
                EncryptAndWrite();
                isSuccess = true;
            } catch ( System.Exception e ) {
                Debug.LogError( "Exception caught: " + e.Message + ", stack trace: " + e.StackTrace );
                isSuccess = false;
            } finally {
                isRunning = false;
            }
        }

        public void RunBlocking()
        {
            isRunning = true;
            try {
                SaveOnMainThread();
                isSuccess = true;
            } catch ( System.Exception e ) {
                isSuccess = false;
                Debug.LogError( "Exception caught: " + e.Message + ", stack trace: " + e.StackTrace );
            } finally {
                isRunning = false;
            }
        }

        protected void SaveOnMainThread()
        {
            SplitKeysAndBoxObjects();
            EncryptAndWrite();
            Debug.Log( "Succefully saved data to file: " + EncryptedFileStorage.LocalFilePath + " and to: " + EncryptedFileStorage.CloudFilePath );
        }

        private void EncryptAndWrite()
        {
            WriteLocal();
            WriteCloud();
        }

        private void SplitKeysAndBoxObjects()
        {
            _localStorage.Clear();
            _cloudStorage.Clear();
            foreach ( var key in _storage.Keys ) {
                if ( _dontSyncKeys.Contains( key ) ) {
                    _localStorage[key] = StorageBase.BoxObject( key, _storage, _objects );
                } else {
                    _cloudStorage[key] = StorageBase.BoxObject( key, _storage, _objects );
                }
            }
        }

        protected virtual void WriteCloud()
        {
            cloudStorageEncrypted = MiscUtil.SerializeAndEncrypt( DictionaryForSave( _cloudStorage ), "xoaWHL]-" );

            File.WriteAllText(
                EncryptedFileStorage.CloudFilePath, cloudStorageEncrypted );
        }

        protected virtual void WriteLocal()
        {
            File.WriteAllText(
                            EncryptedFileStorage.LocalFilePath,
                            MiscUtil.SerializeAndEncrypt( DictionaryForSave( _localStorage ), "xoaWHL]-" ) );
        }

        protected Dictionary<string, object> DictionaryForSave( Dictionary<string, object> storage )
        {
            _bufferDict.Clear();
            _bufferDict[EncryptedFileStorage.StorageKey] = storage;
            _bufferDict[EncryptedFileStorage.StringListsKey] = _stringLists.ToList();
            _bufferDict[EncryptedFileStorage.ObjectsKey] = _objects.ToList();
            _bufferDict[EncryptedFileStorage.IntsKey] = _ints.ToList();

            return _bufferDict;
        }
    }
}
