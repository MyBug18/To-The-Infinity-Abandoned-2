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
                if ('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z')
                {
                    currentWordToken.Add(c);
                    continue;
                }

                if (!IsValidSpecialCharacter(c))
                    throw new InvalidOperationException($"Unexpected character : {c}.");

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

            return result;
        }

        private static bool IsValidSpecialCharacter(char c)
        {
            var set = " !&|()=\n";
            foreach (var validChar in set)
                if (validChar == c) return true;

            return false;
        }

        private static IPropositionalLogic<T> ParseConditionInternal(List<string> condition, Func<string, T, bool> _conditionChecker)
        {
            var walker = 0;

            var _incompleteLogics = new Stack<IPropositionalLogic<T>>();

            IPropositionalLogic<T> currentTree = null;

            while (walker < condition.Count)
            {
                var current = condition[walker];
                var wasEvaluation = false;
                walker++;

                switch (current)
                {
                    case "(":
                        var insideString = GetParenthesisSurrounding(walker, condition, out var endIdx);
                        var inside = ParseConditionInternal(insideString, _conditionChecker);

                        if (_incompleteLogics.Count > 0)
                        {
                            currentTree = _incompleteLogics.Pop();
                            currentTree.SetRestExpression(inside);
                        }
                        else
                            currentTree = inside;

                        wasEvaluation = true;
                        walker = endIdx + 1;
                        break;
                    case ")":
                        throw new InvalidOperationException("Parenthesis mismatch!");
                    case "!":
                        _incompleteLogics.Push(new NotLogic<T>());
                        break;
                    case "|":
                        _incompleteLogics.Push(new OrLogic<T>(currentTree));
                        break;
                    case "&":
                        _incompleteLogics.Push(new AndLogic<T>(currentTree));
                        break;
                    default:
                        var stringValueLogic = new StringValueLogic<T>(t => _conditionChecker(current, t));

                        if (_incompleteLogics.Count > 0)
                        {
                            currentTree = _incompleteLogics.Pop();
                            currentTree.SetRestExpression(stringValueLogic);
                        }
                        else
                            currentTree = stringValueLogic;

                        wasEvaluation = true;
                        break;
                }

                if (wasEvaluation)
                {
                    while (_incompleteLogics.Count > 0)
                    {
                        var temp = _incompleteLogics.Pop();
                        temp.SetRestExpression(currentTree);
                        currentTree = temp;
                    }
                }
            }

            return currentTree;
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
    public class StringValueLogic<T> : IPropositionalLogic<T>
    {
        private Func<T, bool> _valueChecker;

        public StringValueLogic(Func<T, bool> valueChecker)
        {
            _valueChecker = valueChecker;
        }

        public bool Evaluate(T input) => _valueChecker(input);

        public void SetRestExpression(IPropositionalLogic<T> _) => new InvalidOperationException("Operator expected!");
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