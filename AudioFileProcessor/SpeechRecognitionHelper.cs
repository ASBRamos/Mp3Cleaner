using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Runtime.Versioning;

namespace AudioFileUtils;

[SupportedOSPlatform("windows")]
internal class SpeechRecognitionHelper
{
    private readonly SpeechRecognitionEngine speechRecognitionEngine;
    public SpeechRecognitionHelper()
    {
        this.speechRecognitionEngine = new SpeechRecognitionEngine();
        this.speechRecognitionEngine.LoadGrammar(new DictationGrammar());
        this.speechRecognitionEngine.BabbleTimeout = new TimeSpan(long.MaxValue);
        this.speechRecognitionEngine.InitialSilenceTimeout = new TimeSpan(long.MaxValue);
    }

    public string ParseSpeechToText(MemoryStream wavDataStream, int? timeoutSecondsAfterOpeningWords)
    {
        this.speechRecognitionEngine.EndSilenceTimeout = new TimeSpan(0, 0, timeoutSecondsAfterOpeningWords ?? 2); // we want to end the sample after hearing the initial words
        this.speechRecognitionEngine.SetInputToWaveStream(wavDataStream);

        StringBuilder sb = new StringBuilder();
        while (true)
        {
            try
            {
                var recognizedText = this.speechRecognitionEngine.Recognize();
                if (recognizedText == null)
                {
                    break;
                }

                sb.Append(recognizedText.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                break;
            }
        }

        return sb.ToString();
    }
}
