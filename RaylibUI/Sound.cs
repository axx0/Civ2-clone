using Civ2engine;
using NAudio.Wave;
using Raylib_cs;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace RaylibUI
{



/// <summary>
/// Sound Management Class
/// </summary>
/// 
//ToDo: I think the play sound functions need to be reworked and made seperate from effects/music
//Maybe we store that cache data as json if we're to add more to it? 
  public class Sound : IDisposable
  {
    private Thread cacheSyncThread;
    private bool soundDataCacheIsRunning = false;
    private DateTime soundLastPlayed = DateTime.Now;
    private bool soundDataCacheIsInvalid = false;
    private bool soundDataCacheUpdating = false;
    public ConcurrentBag<SoundData> soundDataCache = null;
    private static string strGameSoundDir = String.Empty;
    private static string strSoundConvDir = String.Empty;
    private static object SoundCacheLock = new object();
 
    public static string getConvDir
    {
      get
      {
      //ToDo: Consider moving this to maybe the app executing directory on release builds so that we don't add things outside this app scope.

        if (strSoundConvDir == String.Empty)
          strSoundConvDir = Settings.Civ2Path + Path.DirectorySeparatorChar + "CONVERTEDSOUNDS" + Path.DirectorySeparatorChar;
        return strSoundConvDir;
      }
    }

    public Sound()
    {
      if (!Directory.Exists(getConvDir))
        Directory.CreateDirectory(getConvDir);
      ReloadAllExistingConvertedSoundReferences();
      soundDataCacheIsRunning = true;
      cacheSyncThread = new Thread(CacheSyncLoop);
      cacheSyncThread.Start();
    }

    #region caching
    private void CacheSyncLoop()
    {
      while (soundDataCacheIsRunning)
      {
      
        // Check if 10 seconds have passed since soundLastPlayed
        if (soundDataCacheIsInvalid && !soundDataCacheUpdating && DateTime.Now - soundLastPlayed >= TimeSpan.FromSeconds(10) )
        {
          soundDataCacheIsInvalid = false;
          soundDataCacheUpdating = true;
          // Call SynchronizeCacheReference to update the cache
          SynchronizeCacheReference();
          soundDataCacheUpdating = false;
          
        }

        // Sleep for 5 seconds
        Thread.Sleep(5000);
      }
    }

    private void ReloadAllExistingConvertedSoundReferences()
    {
      string cachePath = $"{getConvDir}SoundCache.txt";
      soundDataCache = new ConcurrentBag<SoundData>();
      if (File.Exists(cachePath))
      {
        lock (SoundCacheLock)
        {
          using (StreamReader reader = new StreamReader(cachePath))
          {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
              SoundData soundData = SoundData.FromCacheString(line);
              if (soundData != null)
              {
                soundDataCache.Add(soundData);
              }
            }
          }
        }
      }
    }

    public void SynchronizeCacheReference()
    {
      string cachePath = $"{getConvDir}SoundCache.txt";
      lock (SoundCacheLock)
      {
        using (StreamWriter writer = new StreamWriter(cachePath))
        {
          foreach (SoundData soundData in soundDataCache)
          {
            string cacheString = soundData.ToCacheString();
            writer.WriteLine(cacheString);
          }
        }
      }
    }

    public void AddToCache(SoundData soundData)
    {
      soundDataCache.Add(soundData);
      soundDataCacheIsInvalid = true;
    }

    #endregion

    public SoundData PlayCIV2DefaultSound(string soundName, bool loop = false)
    {
      string pth = Settings.Civ2Path + Path.DirectorySeparatorChar + "SOUND" + Path.DirectorySeparatorChar + soundName;

      // This will cause issues on other operating systems like Linux where case matters!
      if (!pth.ToUpper().EndsWith(".WAV"))
        pth += ".WAV";

      return PlaySound(pth, loop);
    }

    public SoundData PlaySound(string soundPath, bool loop = false)
    {
      if (!File.Exists(soundPath))
        return null;

      var x = soundDataCache.FirstOrDefault(o => o.PathFull == soundPath);
      if (x == null)
      {
        x = new SoundData(soundPath);
        if (!x.IsConverted)
        {
          x.Convert();
          AddToCache(x);
        }
      }

      soundLastPlayed = DateTime.Now;

      if (x != null && x.IsConverted)
      {
        if (loop)
        {
          x.LoopSound();
          return x;
        }
        else
        {
          x.Play();
          return x;
        }
      }

      return null;
    }

    public void Dispose()
    {
      soundDataCacheIsRunning = false;
      cacheSyncThread.Join();
      this.SynchronizeCacheReference();

      foreach (var i in soundDataCache)
      {
        i.Dispose();
      }


    }

    public class SoundData : IDisposable
    {
      private string _strMD5 = String.Empty;
      private string _pathFull = String.Empty;
      private string _pathConv = String.Empty;
      private bool _isConverted = false;
      public DateTime? lastPlayed = null;
      private Raylib_cs.Sound rlSound;
      private bool rlSoundLoaded = false;
      public Raylib_cs.Music rlMusic;
      private bool rlMusicLoaded = false;

      public bool IsConverted
      {
        get { return _isConverted; }
        set { _isConverted = value; }
      }

      public string PathFull
      {
        get { return _pathFull; }
        set { _pathFull = value; }
      }

      public string PathConv
      {
        get
        {
          if (_pathConv == String.Empty)
          {
            _pathConv = $"{Sound.getConvDir}{StrMD5}.wav";
          }
          return _pathConv;
        }
        set { _pathConv = value; }
      }

      public string StrMD5
      {
        get
        {
          if (PathFull != string.Empty)
          {
            _strMD5 = GetUniqueIdentifier(PathFull);
          }
          return _strMD5;
        }
        set { _strMD5 = value; }
      }

      public SoundData(string _fullpath)
      {
        PathFull = _fullpath;
        IsConverted = File.Exists(PathConv);
      }

      public void Convert()
      {
        IsConverted = ConvertAudioPcmU8ToPcmS16LE(PathFull, PathConv);
      }

      public void Stop()
      {
        if (rlSoundLoaded)
          Raylib.StopSound(this.rlSound);
        else if (rlMusicLoaded)
          Raylib.StopMusicStream(this.rlMusic);
      }

      public Raylib_cs.Sound Play()
      {
        if (!rlSoundLoaded)
        {
          if (File.Exists(PathConv))
          {
            rlSound = Raylib.LoadSound(PathConv);
            rlSoundLoaded = rlSound.frameCount > 0;
          }
        }

        if (rlSoundLoaded)
        {
          Raylib.PlaySound(rlSound);
          this.lastPlayed = DateTime.Now;
        }

        return rlSound;
      }

      public Raylib_cs.Music LoopSound()
      {
        if (!rlMusicLoaded)
        {
          if (File.Exists(PathConv))
          {
            rlMusic = Raylib.LoadMusicStream(PathConv);
            rlMusicLoaded = rlMusic.frameCount > 0;
          }
        }

        if (rlMusicLoaded)
        {
          rlMusic.looping = true;
          Raylib.PlayMusicStream(rlMusic);
        }

        return rlMusic;
      }

      /// <summary>
      /// Music objects must call this within the main while loop!
      /// </summary>
      public void MusicUpdateCall()
      {
        if (rlMusic.frameCount > 0)
          Raylib.UpdateMusicStream(this.rlMusic);
      }


      public bool ConvertAudioPcmU8ToPcmS16LE(string inputFilePath, string outputFilePath)
      {
        using (var reader = new WaveFileReader(inputFilePath))
        {
          var format = new WaveFormat(22050, 16, 2); // Desired output format: 22.05 kHz, 16-bit, Stereo (for raylib)

          using (var conversionStream = new WaveFormatConversionStream(format, reader))
          {
            WaveFileWriter.CreateWaveFile(outputFilePath, conversionStream);
          }
        }

        return true;
      }

      public string GetUniqueIdentifier(string inputString)
      {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
          byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
          byte[] hashBytes = md5.ComputeHash(inputBytes);

          StringBuilder stringBuilder = new StringBuilder();
          for (int i = 0; i < hashBytes.Length; i++)
          {
            stringBuilder.Append(hashBytes[i].ToString("x2")); // Convert each byte to a hexadecimal string representation
          }

          return stringBuilder.ToString();
        }
      }

      public void Dispose()
      {
        if (rlSoundLoaded || rlSound.frameCount > 0)
        {
          Stop();
          Raylib.UnloadSound(rlSound);
        }
      }

      internal static SoundData FromCacheString(string line)
      {
        if (String.IsNullOrEmpty(line))
        {
          return null;
        }
        else
        {
          return new SoundData(line);
        }
      }


      internal string ToCacheString()
      {
        return $"{this.PathFull}";
      }
    }
  }
}
