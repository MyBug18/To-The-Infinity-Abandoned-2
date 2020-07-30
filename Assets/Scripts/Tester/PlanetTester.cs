﻿using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Infinity;
using Infinity.HexTileMap;
using Infinity.PlanetPop;
using Infinity.PlanetPop.BuildingCore;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    public class B
    {
        [JsonProperty]
        public int asdf { get; }
    }

    public class PlanetTester : MonoBehaviour
    {
        [SerializeField]
        private PlanetWrapper planetPrefab;

        private void Start()
        {
            TestJson();
        }

        private void PlayDynamicLinq()
        {
            var args = new Dictionary<string, object> {{"myInt", "40"}, {"myBool", true}};

            var condition = "fieldA > int.Parse(myInt) && fieldB == myBool";

            var expr = DynamicExpressionParser.ParseLambda(new ParameterExpression[] { Expression.Parameter(typeof(A)) },
                typeof(bool),
                condition, args);

            var func = expr.Compile();
            Debug.Log((bool)func.DynamicInvoke(new A()));
        }

        private void TestJson()
        {
            var j = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Buildings", "TestBuilding.json"));
            var p = new BuildingPrototype(j);
        }
    }
}