using System;

namespace Infinity
{
    /// <summary>
    /// Super simple propositional logic
    /// </summary>
    public interface IPropositionalLogic<T>
    {
        bool Evaluate(T input);
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
            

        public bool Evaluate(T input)
        {
            return _valueChecker(input);
        }
    }

    /// <summary>
    /// Evaluates NOT operation
    /// </summary>
    public class NotLogic<T> : IPropositionalLogic<T>
    {
        private IPropositionalLogic<T> _e1;

        public NotLogic(IPropositionalLogic<T> e1)
        {
            _e1 = e1;
        }

        public bool Evaluate(T input)
        {
            return !_e1.Evaluate(input);
        }
    }

    /// <summary>
    /// Evaluates AND operation
    /// </summary>
    public class AndLogic<T> : IPropositionalLogic<T>
    {
        private IPropositionalLogic<T> _e1, _e2;

        public AndLogic(IPropositionalLogic<T> e1, IPropositionalLogic<T> e2)
        {
            _e1 = e1;
            _e2 = e2;
        }

        public bool Evaluate(T input)
        {
            return _e1.Evaluate(input) && _e1.Evaluate(input);
        }
    }

    /// <summary>
    /// Evaluates OR operation
    /// </summary>
    public class OrLogic<T> : IPropositionalLogic<T>
    {
        private IPropositionalLogic<T> _e1, _e2;

        public OrLogic(IPropositionalLogic<T> e1, IPropositionalLogic<T> e2)
        {
            _e1 = e1;
            _e2 = e2;
        }

        public bool Evaluate(T input)
        {
            return _e1.Evaluate(input) || _e1.Evaluate(input);
        }
    }
}
