
/*
namespace com
{
    public class StorageInstaller : Installer<StorageInstaller>
    {
        public override void InstallBindings()
        {
#if PLAYER_PREFS_STORAGE
            Container.Bind<IStorageService>().To<EncryptedPrefsStorage>().AsSingle().NonLazy();
#else
            Container.Bind<IStorageService>().To<EncryptedFileStorage>().AsSingle().NonLazy();
#endif
            Container.Bind<AutoSaveSystem>().AsSingle().NonLazy();
        }
    }
}
*/