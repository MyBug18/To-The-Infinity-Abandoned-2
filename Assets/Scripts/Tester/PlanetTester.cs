using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Infinity.GameData;
using Infinity.PlanetPop;
using UnityEngine;
using Newtonsoft.Json;

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

            var expr = DynamicExpressionParser.ParseLambda(new[] { Expression.Parameter(typeof(A)) },
                typeof(bool),
                condition, args);

            var func = expr.Compile();
            Debug.Log((bool)func.DynamicInvoke(new A()));
        }

        private void TestJson()
        {
            var l = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "gamedata/buildingdata"));

            if (l == null)
            {
                Debug.Log("ASDFASDFASDF");
                return;
            }

            var j = l.LoadAsset<TextAsset>("TestBuilding.json");
            var p = new BuildingPrototype(j.text);

            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "test.json"), JsonConvert.SerializeObject(p, Formatting.Indented));
        }
    }
}