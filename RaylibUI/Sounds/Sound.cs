using System.Collections.Concurrent;
using Civ2engine;

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
    private readonly Thread _cacheSyncThread;
    private bool _soundDataCacheIsRunning;
    private DateTime _soundLastPlayed = DateTime.Now;
    private bool _soundDataCacheIsInvalid;
    private bool _soundDataCacheUpdating;
    public readonly ConcurrentBag<SoundData> soundDataCache;

    private static string _convertedSoundsDir = String.Empty;
    private static readonly object SoundCacheLock = new();

    public Sound()
    {
      _convertedSoundsDir = Path.Combine(Settings.BasePath, "CONVERTEDSOUNDS");
      if (!Directory.Exists(_convertedSoundsDir))
        Directory.CreateDirectory(_convertedSoundsDir);
      soundDataCache = ReloadAllExistingConvertedSoundReferences();
      _soundDataCacheIsRunning = true;
      _cacheSyncThread = new Thread(CacheSyncLoop);
      _cacheSyncThread.Start();
    }

    #region caching

    private void CacheSyncLoop()
    {
      while (_soundDataCacheIsRunning)
      {
        // Check if 10 seconds have passed since soundLastPlayed
        if (_soundDataCacheIsInvalid && !_soundDataCacheUpdating &&
            DateTime.Now - _soundLastPlayed >= TimeSpan.FromSeconds(10))
        {
          _soundDataCacheIsInvalid = false;
          _soundDataCacheUpdating = true;
          // Call SynchronizeCacheReference to update the cache
          SynchronizeCacheReference();
          _soundDataCacheUpdating = false;
        }

        // Sleep for 5 seconds
        Thread.Sleep(5000);
      }
    }

    private ConcurrentBag<SoundData> ReloadAllExistingConvertedSoundReferences()
    {
      string cachePath = Path.Combine(_convertedSoundsDir,"SoundCache.txt");
      var cache = new ConcurrentBag<SoundData>();
      if (File.Exists(cachePath))
      {
        lock (SoundCacheLock)
        {
          using (StreamReader reader = new StreamReader(cachePath))
          {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
              SoundData soundData = SoundData.FromCacheString(line, _convertedSoundsDir);
              if (soundData != null)
              {
                cache.Add(soundData);
              }
            }
          }
        }
      }

      return cache;
    }

    public void SynchronizeCacheReference()
    {
      string cachePath = $"{_convertedSoundsDir}SoundCache.txt";
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
      _soundDataCacheIsInvalid = true;
    }

    #endregion

    public SoundData? PlayCIV2DefaultSound(string soundName, bool loop = false)
    {
      var pth = Utils.GetFilePath(soundName, Settings.SearchPaths.Select(p=>Path.Combine(p,"Sound")), "wav");

      return pth != null ? PlaySound(pth, loop) : null;
    }

    public SoundData? PlaySound(string soundPath, bool loop = false)
    {
      if (!File.Exists(soundPath))
        return null;

      var x = soundDataCache.FirstOrDefault(o => o.PathFull == soundPath);
      if (x == null)
      {
        x = new SoundData(soundPath, _convertedSoundsDir);
        if (!x.IsConverted)
        {
          x.Convert();
          AddToCache(x);
        }
      }

      _soundLastPlayed = DateTime.Now;

      if (x.IsConverted)
      {
        if (loop)
        {
          x.LoopSound();
          return x;
        }

        x.Play();
        return x;
      }

      return null;
    }

    public void Dispose()
    {
      _soundDataCacheIsRunning = false;
      _cacheSyncThread.Join();
      SynchronizeCacheReference();

      foreach (var i in soundDataCache)
      {
        i.Dispose();
      }


    }
  }
}
