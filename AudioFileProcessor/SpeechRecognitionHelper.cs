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
    internal readonly SpeechRecognitionEngine speechRecognitionEngine;
    public SpeechRecognitionHelper()
    {
        this.speechRecognitionEngine = new SpeechRecognitionEngine();
        this.speechRecognitionEngine.LoadGrammar(new DictationGrammar());
        this.speechRecognitionEngine.BabbleTimeout = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
        this.speechRecognitionEngine.InitialSilenceTimeout = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
    }

    public string ParseSpeechToText(MemoryStream wavDataStream, int? timeoutSecondsAfterOpeningWords)
    {
        // we want to end the sample after hearing the initial words
        this.speechRecognitionEngine.EndSilenceTimeout = new TimeSpan(hours: 0, minutes: 0, seconds: timeoutSecondsAfterOpeningWords ?? 2);
        this.speechRecognitionEngine.SetInputToWaveStream(wavDataStream);

        try
        {
            var recognizedText = this.speechRecognitionEngine.Recognize();
            if (recognizedText == null)
            {
                return "No text recognized.";
            }
            return recognizedText.Text;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}
