using System.Text;

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// タイピングゲームの表示用データを生成するクラス
    /// </summary>
    public class TypingViewDataBuilder
    {
        // 表示key
        public StringBuilder ViewInputKeys { get; } = new();

        // 表示、未入力のキー
        public StringBuilder ViewNotInputKeys { get; } = new();

        // 表示、入力済みのキー
        public StringBuilder ViewInputtedKeys { get; } = new();

        public void UpdateViewKeys(TypingState state)
        {
            // １つ目は_currentSectionSelectKey
            ViewInputKeys.Clear();
            ViewInputKeys.Append(state.InputtedSectionKeys);
            ViewInputKeys.Append(state.CurrentSectionSelectKey);
            for (int i = 1; i < state.SuggestHiraganaSections.Count; i++)
            {
                // 候補がない場合は最初の候補を使う
                if (state.SuggestHiraganaSections[i].InputPairs.Count > 0)
                {
                    ViewInputKeys.Append(state.SuggestHiraganaSections[i].InputPairs[0]);
                }
            }

            ViewInputtedKeys.Clear();
            ViewInputtedKeys.Append(state.InputtedSectionKeys);
            ViewInputtedKeys.Append(state.CurrentSectionInputtedKey);

            ViewNotInputKeys.Clear();
            ViewNotInputKeys.Append(ViewInputKeys);
            if (ViewInputtedKeys.Length <= ViewNotInputKeys.Length)
            {
                 ViewNotInputKeys.Remove(0, ViewInputtedKeys.Length);
            }
            else
            {
                // 入力済みキーが表示キーより長くなるケース（タイプミスなど）を考慮
                ViewNotInputKeys.Clear();
            }
        }

        public void Clear()
        {
            ViewInputKeys.Clear();
            ViewNotInputKeys.Clear();
            ViewInputtedKeys.Clear();
        }
    }
}
