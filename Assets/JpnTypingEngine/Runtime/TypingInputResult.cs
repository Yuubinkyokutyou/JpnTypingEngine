using System;
using System.Text;

namespace JpnTypingEngine
{
    public struct TypingInputResult
    {
        //入力が完了したかどうか
        public bool IsFinished;

        public bool IsMiss;
        
        //入力成功
        public bool IsSuccess;

        //入力キーが変更されたかどうか
        public bool InputKeyChange;
        
        //現在のセクションの入力済みキー
        public StringBuilder CurrentSectionInputtedKey;

        //入力されたひらがな
        public StringBuilder InputtedHiragana;
        
        //入力済みのキー（セクションが完了したときにアップデート）
        public StringBuilder InputtedSectionKeys;
        
        //表示用入力キー
        public StringBuilder ViewInputKeys;
        
        // 表示、入力済みのキー
        public StringBuilder ViewInputtedKeys;

        //表示、未入力のキー
        public StringBuilder ViewNotInputKeys;
        
        public int InputtedKeyLength => InputtedSectionKeys.Length + CurrentSectionInputtedKey.Length;
        
        public TypingInputResult(
            StringBuilder currentSectionInputtedKey, 
            StringBuilder inputtedHiragana, 
            StringBuilder inputtedSectionKeys,
            StringBuilder viewInputKeys,
            StringBuilder viewInputtedKeys,
            StringBuilder viewNotInputKeys
            )
        {
            CurrentSectionInputtedKey = currentSectionInputtedKey;
            InputtedHiragana = inputtedHiragana;
            InputtedSectionKeys = inputtedSectionKeys;
            ViewInputKeys = viewInputKeys;
            ViewInputtedKeys = viewInputtedKeys;
            ViewNotInputKeys = viewNotInputKeys;
            
            IsFinished = false;
            IsMiss = false;
            IsSuccess = false;
            InputKeyChange = false;
        }

    }
}