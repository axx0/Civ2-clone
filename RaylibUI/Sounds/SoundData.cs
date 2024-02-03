using System.Security.Cryptography;
using System.Text;
using NAudio.Wave;
using Raylib_cs;

namespace RaylibUI;

public class SoundData : IDisposable
{
    public DateTime? LastPlayed { get; set; }
    private Raylib_cs.Sound _rlSound;
    private bool _rlSoundLoaded;
    public Music RlMusic;
    private bool _rlMusicLoaded;

    public bool IsConverted { get; set; }

    public string PathFull { get; }

    public string PathConv { get; }

    public SoundData(string fullpath, string convertedPath)
    {
        PathFull = fullpath;
        PathConv = Path.Combine(convertedPath, $"{GetUniqueIdentifier(fullpath)}.wav");
        IsConverted = File.Exists(PathConv);
    }

    public void Convert()
    {
        IsConverted = ConvertAudioPcmU8ToPcmS16Le(PathFull, PathConv);
    }

    public void Stop()
    {
        if (_rlSoundLoaded)
            Raylib.StopSound(_rlSound);
        else if (_rlMusicLoaded)
            Raylib.StopMusicStream(RlMusic);
    }

    public void Play()
    {
        if (!_rlSoundLoaded)
        {
            if (File.Exists(PathConv))
            {
                _rlSound = Raylib.LoadSound(PathConv);
                _rlSoundLoaded = _rlSound.FrameCount > 0;
            }
        }

        if (_rlSoundLoaded)
        {
            Raylib.PlaySound(_rlSound);
            LastPlayed = DateTime.Now;
        }
    }

    public Music LoopSound()
    {
        if (!_rlMusicLoaded)
        {
            if (File.Exists(PathConv))
            {
                RlMusic = Raylib.LoadMusicStream(PathConv);
                _rlMusicLoaded = RlMusic.FrameCount > 0;
            }
        }

        if (_rlMusicLoaded)
        {
            RlMusic.Looping = true;
            Raylib.PlayMusicStream(RlMusic);
        }

        return RlMusic;
    }

    /// <summary>
    /// Music objects must call this within the main while loop!
    /// </summary>
    public void MusicUpdateCall()
    {
        if (RlMusic.FrameCount > 0)
            Raylib.UpdateMusicStream(RlMusic);
    }


    public bool ConvertAudioPcmU8ToPcmS16Le(string inputFilePath, string outputFilePath)
    {
        try
        {
            using (var reader = new WaveFileReader(inputFilePath))
            {
                var format = new WaveFormat(22050, 16, 2); // Desired output format: 22.05 kHz, 16-bit, Stereo (for raylib)

                using (var conversionStream = new WaveFormatConversionStream(format, reader))
                {
                    WaveFileWriter.CreateWaveFile(outputFilePath, conversionStream);
                }
            }
        }
        catch (DllNotFoundException ex)
        {
            //TODO: find alternative sound methods for OSX
            return false;
        }

        return true;
    }

    private string GetUniqueIdentifier(string inputString)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(inputString);
            var hashBytes = md5.ComputeHash(inputBytes);

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("x2")); // Convert each byte to a hexadecimal string representation
            }

            return stringBuilder.ToString();
        }
    }

    public void Dispose()
    {
        if (_rlSoundLoaded || _rlSound.FrameCount > 0)
        {
            Stop();
            Raylib.UnloadSound(_rlSound);
        }
    }

    internal static SoundData? FromCacheString(string line, string soundsDir)
    {
        return string.IsNullOrEmpty(line) ? null : new SoundData(line, soundsDir);
    }


    internal string ToCacheString()
    {
        return PathFull;
    }
}