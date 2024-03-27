using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;


namespace com
{
    public class PersistentStorageEditorUtil
    {
        [MenuItem("Custom/Copy save file contents")]
        public static void CopySaveFileContents()
        {
            var dict = MiscUtil.DeserializeAndDecrypt( File.ReadAllText( EncryptedFileStorage.LocalFilePath ),  "xoaWHL]-" ) as IDictionary;            
            EditorGUIUtility.systemCopyBuffer = MiniJSON.Json.Serialize( dict );
        }
    }
}
