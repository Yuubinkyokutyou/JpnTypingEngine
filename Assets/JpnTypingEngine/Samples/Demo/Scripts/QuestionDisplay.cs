using UnityEngine;
using UnityEngine.UI;

namespace JpnTypingEngine.Samples.Demo
{
    public class QuestionDisplay : MonoBehaviour
    {
        
        [SerializeField] private Text inputKeyText;

        [SerializeField] private Text hiraganaText;
        
        [SerializeField]
        Color32 inputedTextColor = new Color32(0, 0, 0, 255);
        
        public void SetQuestion(string hiragana,string inputKey)
        {
            inputKeyText.text = inputKey;
            hiraganaText.text = hiragana;
        }
        
        public void UpdateInputKey(string inputKey,int inputedLength,string hiragana,int inputtedHiragana)
        {
            var inputedText = inputKey.Substring(0, inputedLength);
            var notInputedText = inputKey.Substring(inputedLength);
            
            inputKeyText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(inputedTextColor)}>{inputedText}</color>{notInputedText}";
            
            
            var inputedHiraganaText = hiragana.Substring(0, inputtedHiragana);
            var notInputedHiraganaText = hiragana.Substring(inputtedHiragana);
            this.hiraganaText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(inputedTextColor)}>{inputedHiraganaText}</color>{notInputedHiraganaText}";
        }
    }
}