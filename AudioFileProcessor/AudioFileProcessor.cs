using System.IO;
using System.Runtime.Versioning;
using NAudio.Wave;

namespace AudioFileUtils;

[SupportedOSPlatform("windows")]
public class AudioFileProcessor : IDisposable
{
    public readonly string FilePath;
    public MemoryStream WavAudioData;
    private readonly SpeechRecognitionHelper speechRecognitionHelper;

    public AudioFileProcessor(string filePath)
    {
        this.FilePath = filePath;
        this.speechRecognitionHelper = new SpeechRecognitionHelper();
        this.WavAudioData = new MemoryStream();

        using var reader = new Mp3FileReader(this.FilePath);
        WaveFileWriter.WriteWavFileToStream(this.WavAudioData, reader);
    }

    public string OpeningText(int? silenceTimeout)
    {
        WavAudioData.Seek(0, SeekOrigin.Begin);
        return speechRecognitionHelper.ParseSpeechToText(this.WavAudioData, silenceTimeout);
    }

    public void Dispose()
    {
        this.WavAudioData.Dispose();
        this.speechRecognitionHelper.speechRecognitionEngine.Dispose();
    }
}
