using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YashLib
{
    internal class MediaPlayer : IDisposable
    {
        public float[] fft;
        private GCHandle fftHandle;
        private bool _disposedValue;

#if RELEASE
        const string lib = "UnityMediaPlayer";
#elif LINUX
        const string lib = "ASMedia";
#elif true
        const string lib = "UnityMediaPlayer";
#endif


        public MediaPlayer()
        {
            cppInit();
            this.fft = new float[0x200];
            this.fftHandle = GCHandle.Alloc(this.fft, GCHandleType.Pinned);
        }

        #region DLL Imports
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cppDispose();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetBytesPerSecond();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern float cppGetDuration(string filePath);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetFFT512(IntPtr fft);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppGetFFT512KISS(IntPtr fft);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cppGetMusicFolder();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern float cppGetPosition();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppGetPrescanWholeSongSums(IntPtr fft, int count);
        [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppInit();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppPause();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppPlay(string filePath);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cppPrescanWholeSong(string filePath);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppResume();
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppSetPosition(float secs);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cppSetVolume(float fVol);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppStartPrescan(string filePath);
        [DllImport(lib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool cppStop();
        #endregion

        #region Access Methods
        public int GetBytesPerSecond()
        {
            return cppGetBytesPerSecond();
        }

        public float GetDuration(string filePath)
        {
            return cppGetDuration(filePath);
        }

        public int GetFFT512()
        {
            return cppGetFFT512(this.fftHandle.AddrOfPinnedObject());
        }

        public int GetFFT512KISS()
        {
            return cppGetFFT512KISS(this.fftHandle.AddrOfPinnedObject());
        }

        public static string GetMusicFolder()
        {
            return Marshal.PtrToStringUni(cppGetMusicFolder());
        }

        public float GetPosition()
        {
            return cppGetPosition();
        }

        public bool GetPrescanWholeSongSums(float[] sums, int count)
        {
            GCHandle handle = GCHandle.Alloc(sums, GCHandleType.Pinned);
            bool flag = cppGetPrescanWholeSongSums(handle.AddrOfPinnedObject(), count);
            handle.Free();
            return flag;
        }

        public void Pause()
        {
            cppPause();
        }

        public void PlaySongFile(string filePath)
        {
            cppPlay(filePath);
        }

        public int PrescanWholeSong(string filePath)
        {
            return cppPrescanWholeSong(filePath);
        }

        public void Reset()
        {
            cppDispose();
            cppInit();
        }

        public void Resume()
        {
            cppResume();
        }

        public bool SetPosition(float secs)
        {
            return cppSetPosition(secs);
        }

        public void SetVolume(float fVol)
        {
            cppSetVolume(fVol);
        }

        public void StartPrescan(string filePath)
        {
            cppStartPrescan(filePath);
        }

        public void Stop()
        {
            cppStop();
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this.fftHandle.Free();
                }
                cppDispose();
                this._disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
