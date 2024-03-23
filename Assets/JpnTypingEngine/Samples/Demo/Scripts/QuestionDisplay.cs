using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace JpnTypingEngine.Samples.Demo
{
    public class QuestionDisplay : MonoBehaviour
    {
        
        [SerializeField] private Text inputKeyText;

        [SerializeField] private Text hiraganaText;
        
        [SerializeField]
        Color32 inputedTextColor = new Color32(0, 0, 0, 255);
        
        
        // 揺らす強度
        [SerializeField] private float shakeAmount;
        // 揺らす時間
        [SerializeField] private float shakeDuration;
        
        private Vector3 _initialPosition;

        private void Start()
        {
            _initialPosition = hiraganaText.transform.localPosition;
        }

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
        
        public void TypingMiss()
        {
            StartCoroutine(Shake());
        }
        
        private IEnumerator Shake()
        {
            float elapsedTime = 0f;

            while (elapsedTime < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeAmount;
            
                hiraganaText.transform.localPosition = _initialPosition + new Vector3(x, 0, 0);

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            hiraganaText.transform.localPosition = _initialPosition;
        }
    }
}