﻿using System;
using UnityEngine;

namespace Infinity.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        [SerializeField]
        private new Camera camera;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out var hit)) return;

                var ic = hit.transform.GetComponent<IClickable>();

                ic?.OnClick();
            }
        }
    }
}
