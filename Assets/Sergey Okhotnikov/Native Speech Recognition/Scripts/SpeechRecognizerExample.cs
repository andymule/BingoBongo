using NativeSpeechRecognition;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognizerExample : MonoBehaviour
{
    [SerializeField] private Text ResultText;
    [SerializeField] private Text ErrorText;
    [SerializeField] private Button recognizeButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Dropdown dropdown;

    private string _language = "en-US";
    private SpeechRecognizer _recognizer;

    public void StartRecognition()
    {
        ErrorText.text = "";
        ResultText.text = "";
        _recognizer = HasRecognizerForTheLanguage(_language) ? _recognizer :
            SpeechRecognizer.Create(_language,OnResult,OnError);
        _recognizer.Start();
        recognizeButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    private bool HasRecognizerForTheLanguage(string language)
    {
        return _recognizer != null && _recognizer.Language == language;
    }
    
    public void StopRecognition()
    {
        _recognizer?.Stop();
        _recognizer = null;
        RecognitionNotStarted();
    }

    private void OnResult(string msg)
    {
        ResultText.text = msg;
    }

    private void OnError(string msg)
    {
        ErrorText.text = msg;

        if (msg != "noError")
        {
            RecognitionNotStarted();
        }
    }

    private void RecognitionNotStarted()
    {
        recognizeButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
    }

    
    public void DropdownValueChanged(int index)
    {
         _language = dropdown.options[dropdown.value].text;
         Debug.Log(dropdown.value);
         Debug.Log("selected language: " + _language);

         if (_recognizer != null && _recognizer.Language != _language)
         {
             StopRecognition();
         }
    }

    private void OnDestroy()
    {
        _recognizer?.Stop();
    }

    void Update()
    {
        
    }
}
