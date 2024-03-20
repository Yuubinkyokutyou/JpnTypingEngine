using System.Collections.Generic;

namespace JpnTypingEngine
{
    public static class ListExtensions
    {
        public static void InitializeOrClear<T>(this List<T> list)
        {
            if (list == null)
            {
                list = new List<T>();
            }
            else
            {
                list.Clear();
            }
        }
    }
}