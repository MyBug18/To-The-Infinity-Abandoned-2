using System;
using System.Collections.Generic;
using System.Linq;

namespace Infinity
{
    /// <summary>
    /// Super simple propositional logic
    /// </summary>
    public interface IPropositionalLogic<T>
    {
        bool Evaluate(T input);

        void SetRestExpression(IPropositionalLogic<T> e1);
    }

    /// <summary>
    /// Binary comparison will use another parser.
    /// </summary>
    public struct BinaryComparisonHolder
    {
        public string Name;

        /// <summary>
        /// -1 if less, 0 if equal, 1 if more
        /// </summary>
        public int Operator;

        public int RightValue;
    }

    /// <summary>
    /// Super simple condition parser with custom grammar
    /// </summary>
    public static class ConditionParser<T>
    {
        public static IPropositionalLogic<T> ParseCondition(string condition, Func<string, T, bool> conditionChecker) =>
            ParseConditionInternal(TokenizeConditionString(condition), conditionChecker);

        private static List<string> TokenizeConditionString(string condition)
        {
            var result = new List<string>();
            var currentWordToken = new List<char>();

            foreach (var c in condition)
            {
                if ('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || '0' <= c && c <= '9')
                {
                    currentWordToken.Add(c);
                    continue;
                }

                if (!IsValidSpecialCharacter(c))
                    throw new InvalidOperationException($"Unexpected special character : {c}.");

                if (currentWordToken.Count > 0)
                {
                    result.Add(new string(currentWordToken.ToArray()));
                    currentWordToken.Clear();
                }

                if (c != ' ' && c != '\n')
                    result.Add(c.ToString());
            }

            if (currentWordToken.Count > 0)
                result.Add(new string(currentWordToken.ToArray()));

            for (var i = 0; i < result.Count; i++)
            {
                if (result[i] != "<" && result[i] != "=" && result[i] != ">") continue;

                if (i < 1 || i > result.Count - 2)
                    throw new ArgumentOutOfRangeException();

                var binaryComparisionString = $"{result[i - 1]} {result[i]} {result[i + 1]}";

                result[i] = binaryComparisionString;

                result.RemoveAt(i + 1);
                result.RemoveAt(i - 1);
            }

            return result;
        }

        private static bool IsValidSpecialCharacter(char c) => " !&|()=<>\n".Any(validChar => validChar == c);

        private static IPropositionalLogic<T> ParseConditionInternal(IReadOnlyList<string> condition, Func<string, T, bool> conditionChecker)
        {
            var walker = 0;

            // Will collect not fully constructed logic (ex: (e1 & ), (! )
            var incompleteLogic = new Stack<IPropositionalLogic<T>>();

            // Will point the top node of expression
            IPropositionalLogic<T> topExpression = null;

            // Will store just-evaluated complete expression
            IPropositionalLogic<T> justFullExpression = null;

            while (walker < condition.Count)
            {
                var current = condition[walker];
                walker++;

                switch (current)
                {
                    case "(":
                        var insideString = GetParenthesisSurrounding(walker, condition, out var endIdx);
                        var inside = ParseConditionInternal(insideString, conditionChecker);
                        justFullExpression = inside;
                        walker = endIdx + 1;
                        break;
                    case ")":
                        throw new InvalidOperationException("Parenthesis mismatch!");
                    case "!":
                        incompleteLogic.Push(new NotLogic<T>());
                        break;
                    case "|":
                        incompleteLogic.Push(new OrLogic<T>(topExpression));
                        break;
                    case "&":
                        incompleteLogic.Push(new AndLogic<T>(topExpression));
                        break;
                    default:
                        var stringValueLogic = new ValueLogic<T>(t => conditionChecker(current, t));
                        justFullExpression = stringValueLogic;
                        break;
                }
                if (justFullExpression == null) continue;

                // If a full expression has appeared
                if (incompleteLogic.Count > 0)
                {
                    topExpression = incompleteLogic.Pop();
                    topExpression.SetRestExpression(justFullExpression);
                }
                else
                    topExpression = justFullExpression;

                // Should connect all incomplete logic
                while (incompleteLogic.Count > 0)
                {
                    var temp = incompleteLogic.Pop();
                    temp.SetRestExpression(topExpression);
                    topExpression = temp;
                }

                justFullExpression = null;
            }

            return topExpression;
        }

        private static List<string> GetParenthesisSurrounding(int startIdx, IReadOnlyList<string> condition, out int endIdx)
        {
            var currentParenthesisCount = 1;
            var walker = startIdx;

            var insideParenthesisHolder = new List<string>();

            // get expression surrounded by current parenthesis
            while (currentParenthesisCount != 0)
            {
                if (walker > condition.Count)
                    throw new InvalidOperationException("Parenthesis mismatch!");

                var s = condition[walker];

                if (s == "(")
                    currentParenthesisCount++;
                if (s == ")")
                    currentParenthesisCount--;

                insideParenthesisHolder.Add(s);
                walker++;
            }
            insideParenthesisHolder.RemoveAt(insideParenthesisHolder.Count - 1);

            endIdx = walker;
            return insideParenthesisHolder;
        }
    }

    public static class ConditionParser
    {
        public static BinaryComparisonHolder ParseBinaryComparison(string s)
        {
            if (!IsBinaryComparison(s))
                throw new InvalidOperationException();

            var tokens = s.Split(' ');

            if (!int.TryParse(tokens[2], out var value)) throw new InvalidOperationException();

            switch (tokens[1])
            {
                case "<":
                    return new BinaryComparisonHolder { Name = tokens[0], Operator = -1, RightValue = value };
                case "=":
                    return new BinaryComparisonHolder { Name = tokens[0], Operator = 0, RightValue = value };
                case ">":
                    return new BinaryComparisonHolder { Name = tokens[0], Operator = 1, RightValue = value };
            }

            throw new InvalidOperationException();
        }

        public static bool IsBinaryComparison(string s) => s.Contains('<') || s.Contains('=') || s.Contains('>');
    }

    /// <summary>
    /// Evaluates VALUE operation
    /// </summary>
    public class ValueLogic<T> : IPropositionalLogic<T>
    {
        private readonly Func<T, bool> _valueChecker;

        public ValueLogic(Func<T, bool> valueChecker)
        {
            _valueChecker = valueChecker;
        }

        public bool Evaluate(T input) => _valueChecker(input);

        public void SetRestExpression(IPropositionalLogic<T> _) => throw new InvalidOperationException("Operator expected!");
    }

    /// <summary>
    /// Evaluates NOT operation
    /// </summary>
    public class NotLogic<T> : IPropositionalLogic<T>
    {
        private IPropositionalLogic<T> _e1;

        public bool Evaluate(T input) => !_e1.Evaluate(input);

        public void SetRestExpression(IPropositionalLogic<T> e1)
        {
            _e1 = e1;
        }
    }

    /// <summary>
    /// Evaluates AND operation
    /// </summary>
    public class AndLogic<T> : IPropositionalLogic<T>
    {
        private readonly IPropositionalLogic<T> _e1;
        private IPropositionalLogic<T> _e2;

        public AndLogic(IPropositionalLogic<T> e1)
        {
            _e1 = e1;
        }

        public bool Evaluate(T input) => _e1.Evaluate(input) && _e2.Evaluate(input);

        public void SetRestExpression(IPropositionalLogic<T> e2)
        {
            _e2 = e2;
        }
    }

    /// <summary>
    /// Evaluates OR operation
    /// </summary>
    public class OrLogic<T> : IPropositionalLogic<T>
    {
        private readonly IPropositionalLogic<T> _e1;
        private IPropositionalLogic<T> _e2;

        public OrLogic(IPropositionalLogic<T> e1)
        {
            _e1 = e1;
        }

        public bool Evaluate(T input) => _e1.Evaluate(input) || _e2.Evaluate(input);

        public void SetRestExpression(IPropositionalLogic<T> e2)
        {
            _e2 = e2;
        }
    }
}
