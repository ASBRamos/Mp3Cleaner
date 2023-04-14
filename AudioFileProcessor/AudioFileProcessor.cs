using System.Runtime.Versioning;
using NAudio.Wave;

namespace AudioFileUtils;

[SupportedOSPlatform("windows")]
public class AudioFileProcessor
{
    public readonly string FilePath;
    public readonly MemoryStream WavAudioData;
    private readonly SpeechRecognitionHelper speechRecognitionHelper;
    private readonly WaveFormat AudioDataFormatInfo;

    #region offsets

    private const uint RIFF_OFFSET = 0;
    private const uint WAVE_OFFSET = 8;
    private const uint FMT_OFFSET = 12;
    private const uint DATA_OFFSET = 38;

    #endregion

    public AudioFileProcessor(string filePath)
    {
        this.FilePath = filePath;
        this.speechRecognitionHelper = new SpeechRecognitionHelper();
        this.WavAudioData = new MemoryStream();
        using var reader = new Mp3FileReader(this.FilePath);
        //WaveFileWriter.WriteWavFileToStream(this.WavAudioData, reader);
        WaveFileWriter.CreateWaveFile(filePath + ".wav", reader);
        AudioDataFormatInfo = reader.WaveFormat;
    }

    private void FlipTextEndianness()
    {
        MemoryStream flippedMemoryStream = new MemoryStream();
        flippedMemoryStream.Write(System.Text.Encoding.UTF8.GetBytes("FFIR"));

        byte[] copyData = new byte[4];
        WavAudioData.Seek(RIFF_OFFSET + 4, SeekOrigin.Begin);
        WavAudioData.Read(copyData, offset: 0, count: 4);
        flippedMemoryStream.Write(copyData);

        flippedMemoryStream.Write(System.Text.Encoding.UTF8.GetBytes("EVAW"));
        flippedMemoryStream.Write(System.Text.Encoding.UTF8.GetBytes(" tmf"));

        copyData = new byte[DATA_OFFSET - FMT_OFFSET - 4];
        WavAudioData.Seek(RIFF_OFFSET + 4, SeekOrigin.Begin);
        WavAudioData.Read(copyData, offset: 0, count: 4);
        flippedMemoryStream.Write(copyData);


    }

    public string OpeningText(int? silenceTimeout)
    {
        return speechRecognitionHelper.ParseSpeechToText(this.WavAudioData, silenceTimeout);
    }
}
