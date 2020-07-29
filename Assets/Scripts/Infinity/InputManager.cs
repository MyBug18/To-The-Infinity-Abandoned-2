using UnityEngine;

namespace Infinity
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
            // should be changed to new input system
            // if (Input.GetMouseButtonDown(0))
            // {
            //     var ray = camera.ScreenPointToRay(Input.mousePosition);
            //
            //     if (!Physics.Raycast(ray, out var hit)) return;
            //
            //     var ic = hit.transform.GetComponent<IClickable>();
            //
            //     ic?.OnClick();
            // }
        }
    }
}
