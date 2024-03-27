using System.Collections.Generic;

namespace com
{
    public interface IStorageService
    {
        //HashSet<string> Storage { get; }
        HashSet<string> Objects { get; }
        HashSet<string> DontSyncKeys { get; }
        HashSet<string> StringLists { get; }
        HashSet<string> Ints { get; }

        BetterEvent Saving { get; }
        BetterEvent Loaded { get; }

        void SetDontSync( string key );

        bool HasKey( string key );

        bool GetBool( string key );

        bool GetBool( string key, bool defaultValue );

        void SetBool( string key, bool value );

        int GetInt( string key );

        int GetInt( string key, int defaultValue );

        void SetInt( string key, int value );

        long GetLong( string key );

        long GetLong( string key, long defaultValue );

        void SetLong( string key, long value );

        float GetFloat( string key );

        float GetFloat( string key, float defaultValue );

        void SetFloat( string key, float value );

        string GetString( string key );

        string GetString( string key, string defaultValue );

        void SetString( string key, string value );

        void SetStringList( string key, List<string> value );

        List<string> GetStringList( string key );

        void SetDate( string key, System.DateTime date );

        System.DateTime GetDate( string key );

        void SetObject( string key, object value );

        T GetObject<T>( string key );

        T GetObject<T>( string key, T defaultValue );

        void DeleteKey( string key );

        void Clear();

        void SaveImmiediate();

        void Save();

        void Load(bool isCloudSync);        
    }
}
