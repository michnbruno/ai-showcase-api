using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace AiShowcase.Api.Services
{
    public class SpeechService
    {
        private readonly SpeechConfig _speechConfig;

        public SpeechService(string apiKey, string region)
        {
            _speechConfig = SpeechConfig.FromSubscription(apiKey, region);
            _speechConfig.SpeechRecognitionLanguage = "en-US";
        }

        public async Task<string> TranscribeAudioAsync(Stream audioStream)
        {
            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
            using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

            var buffer = memoryStream.ToArray();
            audioInputStream.Write(buffer);
            audioInputStream.Close();

            var result = await recognizer.RecognizeOnceAsync();

            return result.Reason switch
            {
                ResultReason.RecognizedSpeech => result.Text,
                ResultReason.NoMatch => "No speech could be recognized.",
                ResultReason.Canceled => "Recognition canceled.",
                _ => "Unknown result."
            };
        }
    }
}