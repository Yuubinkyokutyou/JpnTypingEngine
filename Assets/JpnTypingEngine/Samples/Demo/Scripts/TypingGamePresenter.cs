﻿using System.Collections.Generic;
using UnityEngine;

namespace JpnTypingEngine.InGame
{
    public class TypingGamePresenter : MonoBehaviour
    {
        [SerializeField] private QuestionDisplay questionDisplay;
        [SerializeField] InputKeyProvider inputKeyProvider;
        [SerializeField] List<string> hiraganaQuestionList;

        private TypingGameSystem _typingGameSystem;
        
        int _questionIndex = 0;
        
        private void Start()
        {
            _typingGameSystem= new TypingGameSystem();
            inputKeyProvider.OnInputKey += OnInputKey;
            
            SetQuestion(hiraganaQuestionList[0]);
        }

        private void SetQuestion(string hiraganaQuestion)
        {
            var result=_typingGameSystem.SetQuestion(hiraganaQuestion);
            UpdateView(result);
        }

        private void OnInputKey(char key)
        {
            var result = _typingGameSystem.InputKey(key);

            if (result.IsFinished)
            {
                _questionIndex++;
                SetQuestion(hiraganaQuestionList[_questionIndex%hiraganaQuestionList.Count]);
                return;
            }
            
            UpdateView(result);
        }

        private void UpdateView(TypingInputResult typingInputResult)
        {
            //jsonにしてログ
            Debug.Log(
                typingInputResult.InputtedSectionKeys
                );
            questionDisplay.UpdateInputKey(
                typingInputResult.ViewInputKeys.ToString(),
                typingInputResult.InputtedKeyLength,
                hiraganaQuestionList[_questionIndex],
                typingInputResult.InputtedHiragana.Length);
        }
        
        
        private void OnDestroy()
        {
            inputKeyProvider.OnInputKey -= OnInputKey;
            _typingGameSystem?.Dispose();
        }
    }

}