// using System;
// using UnityEngine;
// using System.Collections;
// using Tabasco.GameCenter;
// using Zenject;
// #if UNITY_ANDROID
// using GooglePlayGames.BasicApi.SavedGame;
// using GooglePlayGames.BasicApi;
// using GooglePlayGames;
// #endif

// namespace Tabasco.Common
// {
//     public class GoogleCloudFileProvider : ICloudFileProvider
//     {
//         public event Action OnCloudSaved;
//         public event Action<string> OnCloudLoaded = delegate {};

//         private byte[]  _saveData;
//         private string  _loadedData;
//         private bool    _isDownloaded;
        
//        
//         readonly GameCenterManager _gameCenterManager;

//         public GoogleCloudFileProvider()
//         {
//             _isDownloaded = false;
// #if UNITY_ANDROID
//             if ( PlayGamesPlatform.Instance.IsAuthenticated() ) {
//                 OpenCloudFile( false );
//             } else {
//                 AndroidGameCenterManager.OnAuthenticated += () => OpenCloudFile( false );
//             }
// #endif
//         }

//         public bool IsTimeout()
//         {
//             return false;
//         }

//         public void Save( string contents )
//         {
//             Debug.Log( "saving into google saved games" );
// #if UNITY_ANDROID
//             if ( _gameCenterManager.IsUserAuthenticated() ) {
//                 Debug.Log( "Saving: " + contents + " to google saved games" );
//                 _saveData = System.Text.Encoding.UTF8.GetBytes( contents );
//                 Debug.Log( _saveData.ToString() + " - save data" );
//                 OpenCloudFile( true );
//             }
// #else
//             OnCloudSaved.Invoke();
// #endif
//         }

//         public bool IsDownloaded()
//         {
//             return _isDownloaded;
//         }

//         public void Load()
//         {
//             Debug.Log( "loading from google saved games" );
//             _loadedData = "";

// #if UNITY_ANDROID && !UNITY_EDITOR
//             if ( _gameCenterManager.IsUserAuthenticated() ) {
//                 OpenCloudFile( false );
//             }
// #else
//             OnCloudLoaded( _loadedData );
// #endif
//         }

// #if UNITY_ANDROID

//         private void OpenCloudFile( bool isSave )
//         {
//             PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution( "cloudData.dat", DataSource.ReadNetworkOnly,
//                 ConflictResolutionStrategy.UseOriginal,
//                 ( status, game ) => {
//                     Debug.Log( "open file for save, isSave = " + isSave );

//                     if ( status == SavedGameRequestStatus.Success ) {

//                         if ( isSave ) {
//                             SaveToCloud( game );
//                         } else {
//                             LoadFromCloud( game );
//                         }

//                     } else {
//                         Debug.LogWarning( "Error while opening SavedGame: " + status );
//                     }
//                 } );
//         }

//         private void SaveToCloud( ISavedGameMetadata gameData )
//         {
//             Debug.Log( "Saving Saved Games to " + gameData.Filename );

//             SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
//                 .WithUpdatedPlayedTime(gameData.TotalTimePlayed.Add(new TimeSpan(0, 0, (int)Time.realtimeSinceStartup)))
//                 .WithUpdatedDescription("Saved Game at " + DateTime.Now);

//             SavedGameMetadataUpdate updatedMetadata = builder.Build();
//             PlayGamesPlatform.Instance.SavedGame.CommitUpdate( gameData, updatedMetadata, _saveData, SavedGameWritten );
//         }


//         private void LoadFromCloud( ISavedGameMetadata gameData )
//         {
//             Debug.Log( "Loading Saved Games from " + gameData.Filename );

//             PlayGamesPlatform.Instance.SavedGame.ReadBinaryData( gameData, SavedGameLoaded );
//         }

//         private void SavedGameWritten( SavedGameRequestStatus status, ISavedGameMetadata game )
//         {
//             if ( status == SavedGameRequestStatus.Success ) {
//                 Debug.Log( "Succesfully data saved to Saved Games: " + game.Description );
//             } else {
//                 Debug.Log( "Error saving game: " + status );
//             }
//         }

//         public void SavedGameLoaded( SavedGameRequestStatus status, byte[] data )
//         {
//             if ( status == SavedGameRequestStatus.Success ) {
//                 Debug.Log( "SaveGameLoaded, success=" + status );

//                 if ( data == null )
//                     Debug.Log( "data is null" );
//                 else
//                     Debug.Log( "data is not null" );

//                 _loadedData = Util.FromBytes( data );

//                 if ( _loadedData == null )
//                     Debug.Log( "loaded data is null" );
//                 else
//                     Debug.Log( "loaded data is not null" );

//                 _isDownloaded = true;

//                 OnCloudLoaded( _loadedData );
//             } else {
//                 Debug.LogWarning( "Error reading game: " + status );
//                 _loadedData = "";
//             }
//         }
// #endif
//     }
// }