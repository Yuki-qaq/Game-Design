using System;

namespace com
{
    public interface ICloudFileProvider
    {
        event Action         OnCloudSaved;
        event Action<string> OnCloudLoaded;

        void Save( string contents );
        void Load();
        bool IsTimeout();
        bool IsDownloaded();
    }
}
