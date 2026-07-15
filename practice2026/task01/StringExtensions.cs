namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            string normalized = input.ToLower();
            string filtered = string.Empty;

            foreach (char ch in normalized)
            {
                if (!char.IsWhiteSpace(ch) && !char.IsPunctuation(ch))
                {
                    filtered += ch;
                }
            }

            for (int i = 0; i < filtered.Length / 2; i++)
            {
                if (filtered[i] != filtered[filtered.Length - 1 - i])
                    return false;
            }

            return true;
        }
    }
}