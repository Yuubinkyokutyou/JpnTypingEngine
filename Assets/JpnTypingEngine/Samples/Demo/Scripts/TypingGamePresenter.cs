#region

using System.Collections.Generic;
using JpnTypingEngine.TypingGameSystem;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace JpnTypingEngine.Samples.Demo
{
    public class TypingGamePresenter : MonoBehaviour
    {
        [SerializeField] private QuestionDisplay questionDisplay;
        [SerializeField] private InputKeyProvider inputKeyProvider;
        [SerializeField] private Text allCombinationText;
        [SerializeField] private List<string> hiraganaQuestionList;
        private TypingGameManager _typingGameManager;

        private int _questionIndex;

        private void Start()
        {
            _typingGameManager = new TypingGameManager();
            inputKeyProvider.OnInputKey += OnInputKey;

            SetQuestion(hiraganaQuestionList[0]);
        }

        private void SetQuestion(string hiraganaQuestion)
        {
            var result = _typingGameManager.SetQuestion(hiraganaQuestion);
            UpdateView();

            // allCombinationText.text = _typingGameSystem.GetAllCombinations()[1];
            //改行ですべて表示
            Debug.Log(_typingGameManager.GetAllCombinations().Count);
            allCombinationText.text = string.Join("\n", _typingGameManager.GetAllCombinations());
        }

        private void OnInputKey(char key)
        {
            var result = _typingGameManager.InputKey(key);

            if (result.IsFinished)
            {
                _questionIndex++;
                SetQuestion(hiraganaQuestionList[_questionIndex % hiraganaQuestionList.Count]);
                return;
            }

            Debug.Log(result.IsMiss);
            //ミスの時は、文字を揺らす
            if (result.IsMiss)
            {
                questionDisplay.TypingMiss();
            }

            UpdateView();
        }

        private void UpdateView()
        {
            var viewText = _typingGameManager.GetTypingViewText();
            Debug.Log(viewText.InputtedKeys);
            questionDisplay.UpdateInputKey(
                viewText.InputtedKeys,
                viewText.InputtedKeys.Length,
                hiraganaQuestionList[_questionIndex % hiraganaQuestionList.Count],
                viewText.InputtedHiragana.Length);
        }


        private void OnDestroy()
        {
            inputKeyProvider.OnInputKey -= OnInputKey;
            _typingGameManager?.Dispose();
        }
    }
}