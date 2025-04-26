using System.Collections.Generic;
using System.Text;

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// タイピングゲームの状態を管理するクラス
    /// </summary>
    public class TypingState
    {
        // 予測の入力セクションを、入力順に格納
        public List<HiraganaSection> SuggestHiraganaSections { get; } = new();

        // 現在のセクションであるHiraganaSection[0]のどのキーが選択されているか
        public string CurrentSectionSelectKey { get; set; }

        // 現在のセクションの入力済みキー
        public StringBuilder CurrentSectionInputtedKey { get; } = new();

        // 入力されたひらがな
        public StringBuilder InputtedHiragana { get; } = new();

        // 入力済みのキー（セクションが完了したときにアップデート）
        public StringBuilder InputtedSectionKeys { get; } = new();

        public void ClearCurrentSectionInputtedKey() => CurrentSectionInputtedKey.Clear();
        public void AppendToCurrentSectionInputtedKey(char key) => CurrentSectionInputtedKey.Append(key);
        public void RemoveLastFromCurrentSectionInputtedKey() => CurrentSectionInputtedKey.Remove(CurrentSectionInputtedKey.Length - 1, 1);

        public void AppendToInputtedSectionKeys() => InputtedSectionKeys.Append(CurrentSectionInputtedKey);

        public void AppendToInputtedHiragana(string hiragana) => InputtedHiragana.Append(hiragana);

        public void ClearSuggestSections() => SuggestHiraganaSections.Clear();
        public void AddSuggestSection(HiraganaSection section) => SuggestHiraganaSections.Add(section);
        public void RemoveFirstSuggestSection() => SuggestHiraganaSections.RemoveAt(0);

        public void Reset()
        {
            SuggestHiraganaSections.Clear();
            CurrentSectionSelectKey = null;
            CurrentSectionInputtedKey.Clear();
            InputtedHiragana.Clear();
            InputtedSectionKeys.Clear();
        }
    }
}
