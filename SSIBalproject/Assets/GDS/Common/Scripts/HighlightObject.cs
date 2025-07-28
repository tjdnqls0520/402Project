using UnityEngine;

namespace GDS.Common.Scripts {

    /// <summary>
    /// Highlights an object on mouse over (changes object material).
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class HighlightObject : MonoBehaviour, IHighlight {
        Renderer Renderer;
        Material Initial;
        [SerializeField] Material HighlightMaterial;

        void Awake() {
            Renderer = GetComponent<Renderer>();
            Initial = Renderer.material;
        }

        void OnMouseEnter() => Highlight();
        void OnMouseExit() => Unhighlight();

        public void Highlight() => Renderer.material = HighlightMaterial;
        public void Unhighlight() => Renderer.material = Initial;


    }
}