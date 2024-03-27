using UnityEngine;
using System.Collections;
using System;
// using Fabric.Crashlytics;
#if UNITY_IOS || UNITY_TVOS
//using Prime31;
#endif

namespace com
{
    public class TvOSCloudFileProvider : ICloudFileProvider
    {
        public event System.Action OnCloudSaved          = delegate { };
        public event System.Action<string> OnCloudLoaded = delegate { };

        readonly IStorageService _storage;

        public TvOSCloudFileProvider()
        {
#if UNITY_IOS || UNITY_TVOS
            //iCloudBinding.synchronize();
#endif
        }

        public bool IsTimeout()
        {
            return false;
        }

        public bool IsDownloaded()
        {
#if UNITY_TVOS
            return true;
#else
            return false;
#endif
        }

        public void Save( string contents )
        {
#if UNITY_TVOS
            Debug.Log( "saving prefs: " + EncryptedFileStorage.CloudFilePath + "in iCloud" );
            P31Prefs.setString( EncryptedFileStorage.CloudFilePath, contents );

            if( iCloudBinding.synchronize() )
                Debug.Log( "save iCloud synchronized " );
            else
                Debug.Log( "save iCloud NOT synchronized " );
#endif
        }

        public void Load()
        {
            string result = "";
#if (UNITY_IOS || UNITY_TVOS) && false
            result = P31Prefs.getString( EncryptedFileStorage.CloudFilePath );
            OnCloudLoaded ( result );
#endif
        }
    }
}
