#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace AudioSystem
{
    public class AudioSystemEditor : MonoBehaviour
    {
        const string AUDIODBPATH = Big2.resourcePath + "AudioDB.asset";
        const string AUDIOPATH = "AudioAssets/";
        const string GENERATEDEVENTPATH = Big2.scriptPath + "AudioSystem/AudioEventEnum.gen.cs";
        const string EVENTENUMNAME = "AudioEventEnum";

        [MenuItem("Tools/AudioSystem/Generate Audio Events")]
        public static void GenerateAudioEvents()
        {
            DirectoryInfo dir = new DirectoryInfo(Big2.resourcePath + AUDIOPATH);
            FileInfo[] info = dir.GetFiles("*.*");

            string generatedFilePath = GENERATEDEVENTPATH; //The folder Scripts/Enums/ is expected to exist

            AudioDatabase asset = ScriptableObject.CreateInstance<AudioDatabase>();
            AssetDatabase.CreateAsset(asset, AUDIODBPATH);
            asset.audioPathDictionary = new AudioPathDictionary();

            using (StreamWriter streamWriter = new StreamWriter(generatedFilePath))
            {
                streamWriter.WriteLine("namespace AudioSystem\n{");
                streamWriter.WriteLine("public enum AudioEventEnum");
                streamWriter.WriteLine("{");

                for (int i = 0; i < info.Length; i++)
                {
                    if (Regex.IsMatch(info[i].Name, @".*\.(wav|mp3|ogg|aif)$"))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(info[i].Name);
                        Regex toUnderscore = new Regex("[- ]+");
                        fileName = toUnderscore.Replace(fileName, "_");
                        Regex toRemove = new Regex("[^A-Za-z0-9_]");
                        fileName = toRemove.Replace(fileName, "");

                        //create a key so that when the enum is regenerated, references don't get shifted
                        var mystring = fileName;
                        MD5 md5Hasher = MD5.Create();
                        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(mystring));
                        var ivalue = BitConverter.ToInt32(hashed, 0);

                        streamWriter.WriteLine(fileName + " = " + ivalue + ",");
                        asset.audioPathDictionary.Add((AudioEventEnum)ivalue, AUDIOPATH + Path.GetFileNameWithoutExtension(info[i].Name));
                    }
                }
                streamWriter.WriteLine("}\n}");
            }
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("audio event generated");
        }
    }
}
#endif