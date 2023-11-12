#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace VFXSystem
{
    public class VFXSystemEditor : MonoBehaviour
    {
        const string VFXDBPATH = "Assets/Resources/VFXDB.asset";
        const string VFXPATH = "VFXAssets/";  //relative to the Resources folder, vfx assets has to be in resources folder
        const string GENERATEDVFXENUMPATH = "Assets/SEMISOFT/VFXSystem/VFXEnum.gen.cs";

        [MenuItem("Tools/SEMISOFT/VFXSystem/Generate VFX database")]
        public static void GenerateAudioEvents()
        {
            DirectoryInfo dir = new DirectoryInfo("Assets/Resources/"+ VFXPATH);
            FileInfo[] info = dir.GetFiles("*.*");

            string generatedFilePath = GENERATEDVFXENUMPATH; //The folder Scripts/Enums/ is expected to exist

            VFXDatabase asset = ScriptableObject.CreateInstance<VFXDatabase>();
            AssetDatabase.CreateAsset(asset, VFXDBPATH);
            asset.vfxDictionary = new VFXPathDictionary();

            using (StreamWriter streamWriter = new StreamWriter(generatedFilePath))
            {
                streamWriter.WriteLine("namespace SEMISOFT.VFXSystem\n{");
                streamWriter.WriteLine("public enum VFXEnum");
                streamWriter.WriteLine("{");

                for (int i = 0; i < info.Length; i++)
                {
                    if (Regex.IsMatch(info[i].Name, @".*\.prefab$"))
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
                        asset.vfxDictionary.Add((VFXEnum)ivalue, VFXPATH + Path.GetFileNameWithoutExtension(info[i].Name));
                    }
                }
                streamWriter.WriteLine("}\n}");
            }
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("vfx database generated");
        }
    }
}
#endif
