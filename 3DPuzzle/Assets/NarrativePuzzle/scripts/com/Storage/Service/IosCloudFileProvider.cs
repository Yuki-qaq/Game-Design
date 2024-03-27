using UnityEngine;
using System.Collections;
using System;
// using Fabric.Crashlytics;
#if UNITY_IOS || UNITY_TVOS
//using Prime31;
#endif

namespace com
{
    public class IosCloudFileProvider : ICloudFileProvider
    {
        public event System.Action          OnCloudSaved = delegate { };
        public event System.Action<string>  OnCloudLoaded = delegate { };

        private float   _startTime;
        private float   _timeout;
        private bool    _isTimeout;
        private bool    _isNewFile = false;

        public IosCloudFileProvider()
        {
#if (UNITY_IOS || UNITY_TVOS) && false
            iCloudBinding.synchronize();
            Debug.Log("init icloud file provider, iCloud file is downloaded? : " + iCloudBinding.isFileDownloaded(EncryptedFileStorage.cloudFileName));
            _timeout = 20f;
#endif
        }

        public bool IsTimeout()
        {
            return _isTimeout;
        }

        public bool IsDownloaded()
        {
#if (UNITY_IOS || UNITY_TVOS) && false
			return iCloudBinding.isFileDownloaded( EncryptedFileStorage.cloudFileName ) || _isNewFile;
#else
            return false;
#endif
        }

        public void Save( string contents )
        {
#if UNITY_IOS && false
			Debug.Log( "saving file: " + EncryptedFileStorage.cloudFileName + "in iCloud" );
            bool success = false;
            try {
                success = P31CloudFile.writeAllText( EncryptedFileStorage.cloudFileName, contents );
            } catch(Exception e ) {
                string logMsg = "Error while saving save files: " + e.Message;
                Debug.LogError( logMsg );
                // Crashlytics.Log( logMsg );
            }

            if( success ) {
                OnCloudSaved.Invoke();
				Debug.Log ("iCloud file: " + EncryptedFileStorage.cloudFileName + " saved successfully");
            } else {
				Debug.Log ("iCloud file: " + EncryptedFileStorage.cloudFileName + " not saved");
            }

            if( iCloudBinding.synchronize () ) 
                Debug.Log( "save iCloud synchronized " );
            else
                Debug.Log( "save iCloud NOT synchronized " );
#endif
        }

        public void Load()
        {
            string result = "";
#if (UNITY_IOS || UNITY_TVOS) && false
            Debug.Log( "loading iCloud" );

            if ( iCloudBinding.synchronize () ) 
                Debug.Log( "iCloud synchronized " );
            else
                Debug.Log( "load iCloud NOT synchronized " );

			Debug.Log( "file is in cloud: " + iCloudBinding.isFileInCloud( EncryptedFileStorage.cloudFileName ) );
			Debug.Log( "file is downloaded: " + iCloudBinding.isFileDownloaded( EncryptedFileStorage.cloudFileName ) );

            _startTime = Time.time;

			while ( !iCloudBinding.isFileDownloaded( EncryptedFileStorage.cloudFileName ) 
				&& iCloudBinding.isFileInCloud( EncryptedFileStorage.cloudFileName ) ){
                if ( Time.realtimeSinceStartup - _startTime > _timeout || !iCloudBinding.documentStoreAvailable() ){
                    Debug.Log( "iCloud timeout" );
                    _isTimeout = true;
                    break;
                }
            }
           
            Debug.Log( "iCloud download time: " + ( Time.realtimeSinceStartup - _startTime ) ); 

			if ( iCloudBinding.isFileInCloud( EncryptedFileStorage.cloudFileName ) ) {
				if ( iCloudBinding.isFileDownloaded( EncryptedFileStorage.cloudFileName ) ){
					var lines = P31CloudFile.readAllLines( EncryptedFileStorage.cloudFileName );

                    if ( lines != null )
                        Debug.Log( "File contents in cloud: " + string.Join( "", lines ) );

					byte[] bytes = P31CloudFile.readAllBytes( EncryptedFileStorage.cloudFileName );

                    if ( bytes != null )
                        result = Util.FromBytes( bytes );
                    
                }else{
                    Debug.Log( "file is not downloaded :( " );
                }
            } else {
                _isNewFile = true;
				Debug.Log( "file: " + EncryptedFileStorage.cloudFileName + "not exist in iCloud" );
                Debug.Log( "return empty result " );
            }
            OnCloudLoaded ( result );
#endif
        }
    }
}
