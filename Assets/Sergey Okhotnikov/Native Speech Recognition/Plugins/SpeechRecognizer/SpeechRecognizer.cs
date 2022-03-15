using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Android;

namespace NativeSpeechRecognition
{

    public class SpeechRecognizer : AndroidJavaProxy, AndroidCallbackInterface
    {
        private string _language;
        private Action<string> _result;
        private Action<string> _error;

        private static SpeechRecognizer _instance;

        public static SpeechRecognizer Instance => _instance;

        #region Declare external C interface    
    #if UNITY_IOS 

        [DllImport("__Internal")]
        private static extern void stop_speech_recognition();

        [DllImport("__Internal")]
        private static extern void start_speech_recognition(string language, Action<string> result, Action<string> error);

        [MonoPInvokeCallback(typeof(Action<string>))] 
        private static void on_result(string msg) {
            Debug.Log("Message received: " + msg);
            _instance?._result(msg);
        }
        
        [MonoPInvokeCallback(typeof(Action<string>))] 
        private static void on_error(string msg) {
            Debug.LogError("Error received: " + msg);
            _instance?._error(msg);
        }

    #endif
        #endregion
        
#if UNITY_ANDROID 
        private AndroidJavaObject activity;
        private AndroidJavaObject _javaObject;
#endif    

        public static SpeechRecognizer Create(string language, Action<string> result, Action<string> error)
        {
            _instance?.Stop();

            _instance = new SpeechRecognizer(language,result,error);
            Debug.Log("Created new speech recognizer with language: " + language);
            return _instance;
        }

        public void Start()
        {
            Debug.Log("Starting recognition");
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
                Debug.Log("Asking for permission");  
                return;
            }
#endif
            
#if UNITY_IOS 
            start_speech_recognition(_language,on_result,on_error);
#endif
#if UNITY_ANDROID 
            activity.Call("runOnUiThread", new AndroidJavaRunnable(StartAndroid));
#endif
        }

        private void StartAndroid()
        {
#if UNITY_ANDROID            
            _javaObject.Call("start");
#endif
        }
        
        public void Stop()
        {
            Debug.Log("Stopping recognition");
#if UNITY_IOS 
            stop_speech_recognition();
#endif
#if UNITY_ANDROID 
            activity.Call("runOnUiThread", new AndroidJavaRunnable(StopAndroid));
#endif
        }
        private void StopAndroid()
        {
#if UNITY_ANDROID            
            _javaObject.Call("stop");
#endif
        }
        

        private SpeechRecognizer(string language, Action<string> result, Action<string> error) : base("net.okhotnikov.speech_recognizer.RecognizerCallbackReceiver")
        {
            _language = language;
            _result = result;
            _error = error;
#if UNITY_ANDROID 
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
            activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            _javaObject = new AndroidJavaObject("net.okhotnikov.speech_recognizer.SpeechRecognizerBridge", activity, this, _language);
#endif
        }

        public string Language
        {
            get => _language;
            set => _language = value;
        }

        public Action<string> Result
        {
            get => _result;
            set => _result = value;
        }

        public Action<string> Error
        {
            get => _error;
            set => _error = value;
        }

        public void onResult(string result)
        {
            _result(result);
        }

        public void onError(string error)
        {
            _error(error);
        }
        
        internal void OnPermissionDeniedAndDontAskAgain(string permissionName)
        {
            _error("insufficientPermissionsDontAskAgain");
        }
 
        internal void OnPermissionGranted(string permissionName)
        {
            Start();
        }
 
        internal void OnPermissionDenied(string permissionName)
        {
            _error("insufficientPermissions");
        }
    }

}
