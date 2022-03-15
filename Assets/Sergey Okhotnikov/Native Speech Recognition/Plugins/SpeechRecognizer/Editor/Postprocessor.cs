using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class Postprocessor 
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {
 
        if (buildTarget == BuildTarget.iOS) {
            
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            
            PlistElementDict rootDict = plist.root;
            
            var spKey = "NSSpeechRecognitionUsageDescription";
            rootDict.SetString(spKey,"The application uses voice input to interact with a user. It requires speech recognition usage for this purposes.");
            
            var microKey = "NSMicrophoneUsageDescription";
            rootDict.SetString(microKey,"The application uses voice input to interact with a user. It requires microphone access for this purposes.");
            
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
