using System;
using System.Collections.Generic;
using UnityEngine;

namespace com
{
    public abstract class StorageBase : IStorageService
    {
        public const string StorageKey       = "__storage";
        public const string ObjectsKey       = "__objects";

        public Dictionary<string, object>   _storage;
        public HashSet<string>              _objects;
        public HashSet<string>              _dontSyncKeys;
        public HashSet<string>              _stringLists;
        public HashSet<string>              _ints;

        protected BetterEvent                       _savingEvent;
        protected BetterEvent                       _loadedEvent;

        protected StorageBase()
        {
            _storage        = new Dictionary<string, object>();
            _objects        = new HashSet<string>();
            _dontSyncKeys   = new HashSet<string>();
            _stringLists    = new HashSet<string>();
            _ints           = new HashSet<string>();
            _savingEvent    = new BetterEvent();
            _loadedEvent    = new BetterEvent();
        }

        public virtual Dictionary<string, object> Storage
        { get { return _storage; } }

        public virtual HashSet<string> Objects
        { get { return _objects; } }

        public virtual HashSet<string> DontSyncKeys
        { get { return _dontSyncKeys; } }

        public virtual HashSet<string> StringLists
        { get { return _stringLists; } }

        public virtual HashSet<string> Ints
        { get { return _ints; } }

        public virtual BetterEvent Saving
        { get { return _savingEvent; } }

        public virtual BetterEvent Loaded
        { get { return _loadedEvent; } }

        public virtual void Clear()
        {
            _storage.Clear();
            _objects.Clear();
        }

        public virtual void DeleteKey( string key )
        {
            _storage.Remove( key );
            _objects.Remove( key );
        }

        public virtual bool GetBool( string key )
        {
            return GetBool( key, false );
        }

        public abstract bool GetBool( string key, bool defaultValue );

        public virtual void SetDate( string key, System.DateTime date )
        {
            SetLong( key, date.Ticks );
        }

        public virtual System.DateTime GetDate( string key )
        {
            return new System.DateTime( GetLong( key ) );
        }

        public virtual float GetFloat( string key )
        {
            return GetFloat( key, 0.0f );
        }

        public abstract float GetFloat( string key, float defaultValue );

        public virtual int GetInt( string key )
        {
            return GetInt( key, 0 );
        }
        public abstract int GetInt( string key, int defaultValue );

        public virtual long GetLong( string key )
        {
            return GetLong( key, 0L );
        }

        public abstract long GetLong( string key, long defaultValue );

        public virtual T GetObject<T>( string key )
        {
            return GetObject<T>( key, default( T ) );
        }

        public T GetClassObject<T>( string key, T defaultValue ) where T: class
        {
            object ret;
            if ( _storage.TryGetValue( key, out ret ) ) {
                try {
                    return (ret as T);
                } catch ( Exception e ) {
                    Debug.LogError( "Invalid cast for key: " + key + " , result: " + ret + " , Result Type: " + ret.GetType() + " , Desired Type: " + typeof( T ).ToString() + " " + e );
                    Debug.LogError( StackTraceUtility.ExtractStackTrace() );
                    return defaultValue;
                }
            } else {
                return defaultValue;
            }
        }

        public T GetObject<T>( string key, T defaultValue )
        {
            object ret;
            if ( _storage.TryGetValue( key, out ret ) ) {
                try {
                    return (T)Convert.ChangeType(ret, typeof( T ));
                } catch ( Exception e ) {
                    Debug.LogError( "Invalid cast for key: " + key + " , result: " + ret + " , Result Type: " + ret.GetType() + " , Desired Type: " + typeof( T ).ToString() + " " + e );
                    Debug.LogError( StackTraceUtility.ExtractStackTrace() );
                    return defaultValue;
                }
            } else {
                return defaultValue;
            }
        }

        public virtual string GetString( string key )
        {
            return GetString( key, "" );
        }

        public abstract string GetString( string key, string defaultValue );

        public abstract List<string> GetStringList( string key );

        public abstract bool HasKey( string key );

        public abstract void Load( bool isCloudSync );

        public virtual void Save()
        {
            Save( false );
        }

        public virtual void SaveImmiediate()
        {
            Save( true );
        }

        protected abstract void Save( bool immiediate );

        public abstract void SetBool( string key, bool value );

        public abstract void SetDontSync( string key );

        public abstract void SetFloat( string key, float value );

        public abstract void SetInt( string key, int value );

        public abstract void SetLong( string key, long value );

        public virtual void SetObject( string key, object value )
        {
            _objects.Add( key );
            _storage[key] = value;
        }

        public abstract void SetString( string key, string value );

        public abstract void SetStringList( string key, List<string> value );

        public static object BoxObject( string key, Dictionary<string, object> storage, HashSet<string> objects )
        {
            try {
                if ( objects.Contains( key ) ) {
                    bool containsKey = storage.ContainsKey(key);
                    Debug.Assert( containsKey, "No storage for key: " + key );
                    if ( containsKey ) {
                        return JsonUtility.ToJson(
                            new ObjectEntry
                            {
                                type = storage[key].GetType().AssemblyQualifiedName,
                                json = JsonUtility.ToJson( storage[key] )
                            } );
                    } else {
                        return null;
                    }
                } else {
                    return storage[key];
                }
            } catch ( System.Exception e ) {
                Debug.LogException( e );
                return null;
            }
        }


        protected void UnboxObjects()
        {
            foreach ( var key in _objects ) {
                object strObj = null;
                object obj = null;
                if ( _storage.TryGetValue( key, out strObj ) ) {
                    obj = UnboxObject( strObj as string );
                }
                if ( obj == null ) {
                    Debug.LogWarning( "There is no object at key:" + key + "\n" );
                    continue;
                }
                _storage[key] = obj;
            }
        }

        protected object UnboxObject( string boxJson )
        {
            var box = JsonUtility.FromJson<ObjectEntry>(boxJson);
            Debug.Assert( box != null, boxJson );
            if ( box == null )
                return null;
            Debug.Assert( box.json != null );
            Debug.Assert( box.type != null );
            if ( box == null && !string.IsNullOrEmpty( box.type ) && !string.IsNullOrEmpty( box.json ) ) {
                return null;
            }
            System.Type boxType = System.Type.GetType(box.type);
            Debug.Assert( boxType != null );
            if ( boxType == null ) {
                Debug.LogWarning( "Unknown type:" + box.type );
                return null;
            }
            return JsonUtility.FromJson( box.json, boxType );
        }

        [System.Serializable]
        public class ObjectEntry
        {
            public string type;
            public string json;
        }
    }
}
