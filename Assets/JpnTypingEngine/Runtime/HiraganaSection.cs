using System.Collections.Generic;

namespace JpnTypingEngine
{
    /// <summary>
    /// ひらがなの区切りとその入力組み合わせ
    /// </summary>
    public class HiraganaSection
    {
        public string Hiragana { get; private set; }

        /// <summary>
        /// 文章のひらがなの開始位置。「あいうえお」の「あ」だったら０
        /// </summary>
        public int StartIndex { get; private set; }
        
        /// <summary>
        ///　Hiraganaが「い」の場合、入力組み合わせは「i」「yi」
        /// </summary>
        public List<string> InputPairs { get; private set; }
        
        public HiraganaSection(string hiragana, int startIndex, List<string> inputPairs)
        {
            Hiragana = hiragana;
            StartIndex = startIndex;
            InputPairs = inputPairs;
        }
    }
}