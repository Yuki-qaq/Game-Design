using UnityEngine;
using DG.Tweening;

namespace com
{
    [System.Serializable]
    public class MusicData
    {
        public MusicService.MusicType music;
        public AudioSource source;
        public float vol;
    }

    public class MusicService : MonoBehaviour
    {
        public enum MusicType
        {
            None = 0,
            MenuMusicVolume = 1,
            VomMusicVolume = 2,
            BiomeForestMusicVolume = 5,
            BiomeDesertMusicVolume = 6,
            BossMusicVolume = 11,
        }

        public MusicData[] musics;
        public static MusicService instance { get; private set; }
        public float transitionTime = 2.5f;
        private MusicType _crtMusic = MusicType.None;

        void Awake()
        {
            instance = this;
            //var ms = Enum.GetValues(typeof(MusicType));
            //foreach (var v in ms)
            //{
            //    var musicEnum = (MusicType)v;
            //}
            IsEnabled = false;
            //  _adService.SignToAdEvents(OnAdStarted, OnAdEnded);
        }

        void StopAllMusics()
        {
            foreach (var m in musics)
                m.source.Stop();
        }

        MusicData GetMusicData(MusicType m)
        {
            foreach (var md in musics)
            {
                if (md.music == m)
                    return md;
            }

            return null;
        }

        public void PlayMusic(MusicType m, bool fade, bool playSameMusic = false)
        {
            var md = GetMusicData(m);
            if (md == null)
                return;

            if (!playSameMusic && _crtMusic != MusicType.None && _crtMusic == m)
                return;

            StopMusic(true);

            var source = md.source;
            _crtMusic = m;
            if (!IsEnabled)
                return;

            if (fade)
            {
                // UnityEngine.Debug.Log("fadein " + md.music.ToString());
                source.volume = 0;
                source.DOKill();
                source.Play();
                source.DOFade(md.vol, transitionTime);
            }
            else
            {
                source.volume = md.vol;
                source.DOKill();
                source.Play();
            }
        }

        public void StopMusic(bool fade)
        {
            foreach (var md in musics)
            {
                if (md.music == _crtMusic)
                {
                    var source = md.source;
                    if (fade)
                    {
                        //UnityEngine.Debug.Log("fadeout " + md.music.ToString());
                        source.DOKill();
                        source.DOFade(0, transitionTime).OnComplete(() => { source.Stop(); });
                    }
                    else
                    {
                        //UnityEngine.Debug.Log("cut " + md.music.ToString());
                        source.DOKill();
                        source.Stop();
                    }
                }
                else
                {
                    //UnityEngine.Debug.Log("cut stop " + md.music.ToString());
                    md.source.DOKill();
                    md.source.Stop();
                }
            }
            _crtMusic = MusicType.None;
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;

                if (_isEnabled)
                    PlayMusic(_crtMusic, false, true);
                else
                    StopAllMusics();
            }
        }
    }
}