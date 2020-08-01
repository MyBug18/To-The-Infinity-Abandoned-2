using System;
using System.Collections.Generic;

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
    /// Super simple condition parser with custon grammar
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
                if (result[i] == "<" || result[i] == "=" || result[i] == ">")
                {
                    if (i < 1 || i > result.Count - 2)
                        throw new ArgumentOutOfRangeException();

                    var binaryComparisionString = $"{result[i - 1]} {result[i]} {result[i + 1]}";

                    result[i] = binaryComparisionString;

                    result.RemoveAt(i + 1);
                    result.RemoveAt(i - 1);
                }
            }

            return result;
        }

        private static bool IsValidSpecialCharacter(char c)
        {
            var set = " !&|()=<>\n";
            foreach (var validChar in set)
                if (validChar == c) return true;

            return false;
        }

        private static IPropositionalLogic<T> ParseConditionInternal(List<string> condition, Func<string, T, bool> _conditionChecker)
        {
            var walker = 0;

            // Will collect not fully constructed logics (ex: (e1 & ), (! )
            var _incompleteLogics = new Stack<IPropositionalLogic<T>>();

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
                        var inside = ParseConditionInternal(insideString, _conditionChecker);
                        justFullExpression = inside;
                        walker = endIdx + 1;
                        break;
                    case ")":
                        throw new InvalidOperationException("Parenthesis mismatch!");
                    case "!":
                        _incompleteLogics.Push(new NotLogic<T>());
                        break;
                    case "|":
                        _incompleteLogics.Push(new OrLogic<T>(topExpression));
                        break;
                    case "&":
                        _incompleteLogics.Push(new AndLogic<T>(topExpression));
                        break;
                    default:
                        var stringValueLogic = new ValueLogic<T>(t => _conditionChecker(current, t));
                        justFullExpression = stringValueLogic;
                        break;
                }

                // If a full expression has appeared
                if (justFullExpression != null)
                {
                    if (_incompleteLogics.Count > 0)
                    {
                        topExpression = _incompleteLogics.Pop();
                        topExpression.SetRestExpression(justFullExpression);
                    }
                    else
                        topExpression = justFullExpression;

                    // Should connect all incomplete logics
                    while (_incompleteLogics.Count > 0)
                    {
                        var temp = _incompleteLogics.Pop();
                        temp.SetRestExpression(topExpression);
                        topExpression = temp;
                    }

                    justFullExpression = null;
                }
            }

            return topExpression;
        }

        private static List<string> GetParenthesisSurrounding(int startIdx, List<string> condition, out int endIdx)
        {
            var currentParenthesisCount = 1;
            var walker = startIdx;

            var insideParenthesisHolder = new List<string>();

            // get expression surrouded by current parenthesis
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

    /// <summary>
    /// Evaluates VALUE operation
    /// </summary>
    public class ValueLogic<T> : IPropositionalLogic<T>
    {
        private Func<T, bool> _valueChecker;

        public ValueLogic(Func<T, bool> valueChecker)
        {
            _valueChecker = valueChecker;
        }

        public bool Evaluate(T input) => _valueChecker(input);

        public void SetRestExpression(IPropositionalLogic<T> _) => new InvalidOperationException("Operator expected!");
    }

    public static class ConditionParser
    {
        public static BinaryComparisonHolder ParseBinaryComparison(string s)
        {
            var tokens = s.Split(' ');
            if (tokens.Length != 3)
                throw new InvalidOperationException();

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
        private IPropositionalLogic<T> _e1, _e2;

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
        private IPropositionalLogic<T> _e1, _e2;

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