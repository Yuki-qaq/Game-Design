using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace com
{
    public class EncryptedPrefsStorage : EncryptedFileStorage
    {
        public override void CreateSaveWorker()
        {
            _saveWorker = new EncryptedSavePrefsWorker(_storage, _dontSyncKeys, _stringLists, _objects, _ints);
        }

        protected override void SetFilePaths()
        {
            LocalFilePath = localFileName;
            CloudFilePath = cloudFileName;
        }

        protected override void LoadLocal(string filePath)
        {
            if (!PlayerPrefs.HasKey( filePath ) ||
                string.IsNullOrEmpty( PlayerPrefs.GetString( filePath ) ) )
                return;
            
            Debug.Log( "Load prefs: " + filePath );
            HandleLoad( PlayerPrefs.GetString( filePath ) );
        }

		protected override IDictionary GetDictionary( string data )
		{
            return MiscUtil.JsonDeserialize( data ) as IDictionary;
		}

		protected override void DeleteLocal( string filePath )
        {
            PlayerPrefs.DeleteKey( filePath );
        }

        protected override void Save( bool immiediate )
        {
            Saving.Invoke();
            _saveWorker.RunBlocking();
            // if ( _cloudSaveService.IsDeviceSynced() ){
            //     _cloudProvider.Save(_saveWorker.cloudStorageEncrypted);
            // }
        }
    }
}
