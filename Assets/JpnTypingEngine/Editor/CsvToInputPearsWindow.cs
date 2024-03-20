using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace JpnTypingEngine.Editor
{
    public class CsvToInputPearsWindow: EditorWindow
    {
        private TextAsset csvFile;
        private string path = "Assets/JpnTypingEngine/Resources/HiraganaKeyPairList.asset";
        
        [MenuItem("Window/JpnTypingEngine/Csv Importer")]
        public static void ShowWindow()
        {
            GetWindow<CsvToInputPearsWindow>("InputPears CSV     Importer");
        }

        private void OnGUI()
        {
            csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
            path = EditorGUILayout.TextField("Asset Path", path);

            if (GUILayout.Button("Create HiraganaKeyPairList"))
            {
                if (csvFile == null)
                {
                    Debug.LogWarning("読み込むCSVファイルがセットされていません。");
                    return;
                }
                string csvText = csvFile.text;
                string[] afterParse = csvText.Split('\n');

                var hiraganaKeyPairsList = new List<HiraganaKeyPair>();
                for (var i = 0; i < afterParse.Length; i++)
                {
                    string[] parseByComma = afterParse[i].Split(',').Where(checkString => checkString != null && checkString.Trim().Length != 0).ToArray();

                    //Debug.Log(afterParse[i]);
                    
                    var hiragana = parseByComma[0];
                    var inputPairs = parseByComma.ToList();
                    inputPairs.RemoveAt(0);
                    
                    hiraganaKeyPairsList.Add(
                        new HiraganaKeyPair ( hiragana, inputPairs)
                        );
                    //Debug.Log(parseByComma[0]);
                }

                var pariList = ScriptableObject.CreateInstance<HiraganaKeyPairList>();
                pariList.HiraganaKeyPairs = hiraganaKeyPairsList.ToArray();

                var asset = (HiraganaKeyPairList)AssetDatabase.LoadAssetAtPath(path, typeof(HiraganaKeyPairList));
                if (asset == null)
                {
                    AssetDatabase.CreateAsset(pariList, path);
                }
                else
                {
                    EditorUtility.CopySerialized(pariList, asset);
                    AssetDatabase.SaveAssets();
                }

                AssetDatabase.Refresh();
            }
        }
    }
}