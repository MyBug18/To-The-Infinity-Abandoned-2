using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Infinity.PlanetPop;
using UnityEngine;

namespace Tester
{
    public class A
    {
        public int fieldA
        {
            get
            {
                Debug.Log("AAAAA");
                return 54;
            }
        }
        public bool fieldB = true;
    }

    public class PlanetTester : MonoBehaviour
    {
        public static bool TestCondition(A obj, string condition, IDictionary<string, object> args)
        {
            var expr = DynamicExpressionParser.ParseLambda(new ParameterExpression[] { Expression.Parameter(typeof(A)) },
                typeof(bool),
                condition,
                new object[] { args });

            var func = expr.Compile();
            return (bool)func.DynamicInvoke(obj);
        }

        [SerializeField]
        private PlanetWrapper planetPrefab;

        private void Start()
        {
            var args = new Dictionary<string, object> {{"myInt", "40"}, {"myBool", true}};

            var condition = "fieldA > int.Parse(myInt) && fieldB == myBool";

            var expr = DynamicExpressionParser.ParseLambda(new ParameterExpression[] { Expression.Parameter(typeof(A)) },
                typeof(bool),
                condition, args);

            var func = expr.Compile();
            Debug.Log((bool)func.DynamicInvoke(new A()));
        }
    }
}