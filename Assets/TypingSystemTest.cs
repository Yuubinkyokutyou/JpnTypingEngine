//前のタイピングシステムを用いた簡易テスト
// using JpnTypingEngine;
// using RomanInputSystem.Converter;
// using RomanInputSystem.Question;
// using UnityEngine;
//
// public class TypingSystemTest :MonoBehaviour
// {
//     [SerializeField] private QuestionsAsset _questionsAsset;
//     private void Start()
//     {
//         var newTypingSystem = new TypingGameSystem();
//         var oldTypingSystem = new HiraganaToRomanCandidate(new SingleCharacterConverter());
//         
//         foreach (var question in _questionsAsset.QuestionList)
//         {
//            var hiragana = question.HIRAGANA;
//            var oldKeyList = oldTypingSystem.GetAllTypeSpell(hiragana);
//
//            foreach (var oldKey in oldKeyList)
//            {
//                // Debug.Log("Start:"+hiragana+" "+oldKey);
//                newTypingSystem.SetQuestion(hiragana);
//                foreach (var inputKey in oldKey)
//                {
//                    try
//                    {
//                        // Debug.Log(inputKey.ToString());
//                        //
//                        if ((int)inputKey==13)
//                        {
//                            continue;
//                        }
//
//                        var result = newTypingSystem.InputKey(inputKey);
//                        
//                        if (result.IsMiss)
//                        {
//                            //すべての情報を出力
//                            Debug.LogError("Miss:"+hiragana+" oldKey:"+oldKey+
//                                           " inputKey:"+inputKey+" InputtedHiragana:"+
//                                           result.InputtedHiragana+" : "+result.ViewInputKeys+
//                                           " inputtedKeys:"+result.InputtedSectionKeys);
//                            // Debug.Log((int)inputKey);
//
//                            //resultをjson
//                             // Debug.Log(JsonUtility.ToJson(result));
//                        }
//                    }
//                    catch
//                    {
//                        // ignored
//                        Debug.LogError("Error:"+hiragana+" "+oldKey+""+inputKey+"+Error");
//                    }
//                }
//
//                if (!newTypingSystem.TypingInputResult.IsFinished)
//                {
//                    Debug.LogError("入力未完了:"+hiragana+" "+oldKey);
//                }
//                else
//                {
//                    Debug.Log("入力完了:"+hiragana+" "+oldKey);
//                }
//            }
//         }
//         
//         Debug.Log("Finish TypingSystemTest");
//         newTypingSystem.Dispose();
//     }
// }