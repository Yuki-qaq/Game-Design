using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace com
{
    public class EncryptedSavePrefsWorker : EncryptedSaveWorker
    {
        public EncryptedSavePrefsWorker( Dictionary<string, object> storage,
                  HashSet<string> dontSyncKyes,
                  HashSet<string> stringLists,
                  HashSet<string> objects,
                  HashSet<string> ints ) : base( storage, dontSyncKyes, stringLists, objects, ints )
        {

        }

        protected override void WriteCloud()
        {
            cloudStorageEncrypted = MiscUtil.JsonSerialize( DictionaryForSave( _cloudStorage ) );
            PlayerPrefs.SetString( EncryptedFileStorage.CloudFilePath, cloudStorageEncrypted );
            PlayerPrefs.Save();
        }

        protected override void WriteLocal()
        {
            var localStorageEncrypted = MiscUtil.JsonSerialize( DictionaryForSave( _localStorage ) );
            PlayerPrefs.SetString( EncryptedFileStorage.LocalFilePath, localStorageEncrypted );
            PlayerPrefs.Save();
        }
    }
}