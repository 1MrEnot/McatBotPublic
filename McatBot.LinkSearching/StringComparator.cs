namespace McatBot.LinkSearching
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class StringComparator
    {
        private const int MinWordLength = 3;
        private const int SubTokenLength = 2;
        private const double ThresholdWord = 0.45;

        /// <summary>
        /// Вычисляет значение нечеткого сравнения предложений.
        /// </summary>
        /// <param name="first">Первое предложение.</param>
        /// <param name="second">Второе предложение.</param>
        /// <returns>Результат нечеткого сравнения предложений.</returns>
        public static double CalculateFuzzyEqualValue(string first, string second) {
            if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second)) {
                return 1.0;
            }

            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second)) {
                return 0.0;
            }

            var normalizedFirst = NormalizeSentence(first);
            var normalizedSecond = NormalizeSentence(second);

            var tokensFirst = GetTokens(normalizedFirst);
            var tokensSecond = GetTokens(normalizedSecond);

            var fuzzyEqualsTokens = GetFuzzyEqualsTokens(tokensFirst, tokensSecond);

            var equalsCount = fuzzyEqualsTokens.Length;
            var firstCount = tokensFirst.Length;
            var secondCount = tokensSecond.Length;

            var resultValue = 1.0 * equalsCount / (firstCount + secondCount - equalsCount);

            return resultValue;
        }

        /// <summary>
        /// Возвращает эквивалентные слова из двух наборов.
        /// </summary>
        /// <param name="tokensFirst">Слова из первого предложения.</param>
        /// <param name="tokensSecond">Слова из второго набора предложений.</param>
        /// <returns>Набор эквивалентных слов.</returns>
        private static string[] GetFuzzyEqualsTokens(string[] tokensFirst, string[] tokensSecond) {
            var equalsTokens = new List<string>();
            var usedToken = new bool[tokensSecond.Length];

            for (var i = 0; i < tokensFirst.Length; ++i) {
                for (var j = 0; j < tokensSecond.Length; ++j) {
                    if (!usedToken[j]) {
                        if (IsTokensFuzzyEqual(tokensFirst[i], tokensSecond[j])) {
                            equalsTokens.Add(tokensFirst[i]);
                            usedToken[j] = true;
                            break;
                        }
                    }
                }
            }

            return equalsTokens.ToArray();
        }

        /// <summary>
        /// Приводит предложение к нормальному виду:
        /// - в нижнем регистре
        /// - удалены не буквы и не цифры
        /// </summary>
        /// <param name="sentence">Предложение.</param>
        /// <returns>Нормализованное предложение.</returns>
        private static string NormalizeSentence(string sentence) {
            var resultContainer = new StringBuilder(100);
            var lowerSentence = sentence.ToLower();
            foreach (var c in lowerSentence.Where(IsNormalChar))
            {
                resultContainer.Append(c);
            }

            return resultContainer.ToString();
        }

        /// <summary>
        /// Возвращает признак подходящего символа.
        /// </summary>
        /// <param name="c">Символ.</param>
        /// <returns>True - если символ буква или цифра или пробел, False - иначе.</returns>
        private static bool IsNormalChar(char c) {
            return char.IsLetterOrDigit(c) || c == ' ';
        }

        /// <summary>
        /// Разбивает предложение на слова.
        /// </summary>
        /// <param name="sentence">Предложение.</param>
        /// <returns>Набор слов.</returns>
        private static string[] GetTokens(string sentence) {
            var words = sentence.Split(' ');

            return words.Where(word => word.Length >= MinWordLength).ToArray();
        }

        /// <summary>
        /// Возвращает результат нечеткого сравнения слов.
        /// </summary>
        /// <param name="firstToken">Первое слово.</param>
        /// <param name="secondToken">Второе слово.</param>
        /// <returns>Результат нечеткого сравения слов.</returns>
        public static bool IsTokensFuzzyEqual(string firstToken, string secondToken) {
            var equalSubTokensCount = 0;
            var usedTokens = new bool[secondToken.Length - SubTokenLength + 1];
            for (var i = 0; i < firstToken.Length - SubTokenLength + 1; ++i) {
                var subtokenFirst = firstToken.Substring(i, SubTokenLength);
                for (var j = 0; j < secondToken.Length - SubTokenLength + 1; ++j) {
                    if (!usedTokens[j]) {
                        var subtokenSecond = secondToken.Substring(j, SubTokenLength);
                        if (subtokenFirst.Equals(subtokenSecond)) {
                            equalSubTokensCount++;
                            usedTokens[j] = true;
                            break;
                        }
                    }
                }
            }

            var subTokenFirstCount = firstToken.Length - SubTokenLength + 1;
            var subTokenSecondCount = secondToken.Length - SubTokenLength + 1;

            var tanimoto = 1.0 * equalSubTokensCount /
                           (subTokenFirstCount + subTokenSecondCount - equalSubTokensCount);

            return ThresholdWord <= tanimoto;
        }
    }
}