using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com
{
    public class PlayerPrefsStorage : StorageBase
    {
        //
        // protected ICloudFileProvider _cloudProvider;
        //
        // private CloudSaveService _cloudSaveService;

        public PlayerPrefsStorage() : base()
        {
            // _cloudProvider.OnCloudLoaded += HandleCloudLoaded;
            Load();
        }

        private void HandleCloudLoaded(string data)
        {
            bool firstDevice = IsFirstDevice(data);
         
            if( firstDevice ){//_cloudSaveService.IsSyncCompleted() || firstDevice ) {
                Debug.Log( "Cloud save loaded: " + data );
                Load();

                if ( firstDevice ) {
                    // _cloudSaveService.SyncThisDevice();
                    Save();
                    // _cloudSaveService.SetSyncCompleted();
                }
            }
        }

        private bool IsFirstDevice(string data)
        {
            return string.IsNullOrEmpty(data);//(!_cloudSaveService.IsSyncCompleted() && !_cloudSaveService.IsDeviceSynced()) && string.IsNullOrEmpty(data) && !_cloudProvider.IsTimeout();
        }

        public override void Clear()
        {
            base.Clear();
            _dontSyncKeys.Clear();
            _stringLists.Clear();
            _ints.Clear();
            PlayerPrefs.DeleteAll();
        }

        public override bool GetBool( string key, bool defaultValue )
        {
            if( PlayerPrefs.HasKey( key )){
                return PlayerPrefs.GetInt(key) != 0;
            }
            return defaultValue;
        }

        public override float GetFloat( string key, float defaultValue )
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public override int GetInt( string key, int defaultValue )
        {
            return PlayerPrefs.GetInt( key, defaultValue );
        }

        public override long GetLong( string key, long defaultValue )
        {
            return PlayerPrefs.HasKey( key ) ? long.Parse( PlayerPrefs.GetString( key ) ) : defaultValue;
        }

        public override string GetString( string key, string defaultValue )
        {
            return PlayerPrefs.GetString( key, defaultValue );
        }

        public override List<string> GetStringList( string key )
        {
            if ( !PlayerPrefs.HasKey( key ) ) {
                return null;
            }
            return ( MiniJSON.Json.Deserialize( PlayerPrefs.GetString( key, "[]" ) ) as List<object> ).OfType<string>().ToList();
        }

        public override bool HasKey( string key )
        {
            return PlayerPrefs.HasKey( key );
        }

        public override void Load(bool isCloudSync = false )
        {
            UnboxObjects();
            Loaded.Invoke();
        }

        protected override void Save( bool immiediate )
        {
            Saving.Invoke();
            PlayerPrefs.SetString( ObjectsKey, MiniJSON.Json.Serialize( _objects.ToList() ) );
            foreach ( var key in _objects ) {
                PlayerPrefs.SetString( key, ( string )BoxObject( key, _storage, _objects ) );
            }
            PlayerPrefs.Save();
        }

        public override void SetBool( string key, bool value )
        {
            PlayerPrefs.SetInt( key, value ? 1 : 0 );
        }

        public override void SetDontSync( string key )
        {
            _dontSyncKeys.Add( key );
        }

        public override void SetFloat( string key, float value )
        {
            PlayerPrefs.SetFloat( key, value );
        }

        public override void SetInt( string key, int value )
        {
            _ints.Add( key );
            PlayerPrefs.SetInt( key, value );
        }

        public override void SetLong( string key, long value )
        {
            PlayerPrefs.SetString( key, value.ToString() );
        }

        public override void SetString( string key, string value )
        {
            PlayerPrefs.SetString( key, value );
        }

        public override void SetStringList( string key, List<string> value )
        {
            _stringLists.Add( key );
            PlayerPrefs.SetString( key, MiniJSON.Json.Serialize( value ) );
        }
    }
}
