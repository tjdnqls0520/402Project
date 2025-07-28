using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core.Views {
    public class DropTargetView : VisualElement {
        public DropTargetView(Observable<Item> dragged, Action<Item> callback) {
            this.WithClass("drop-target drop-target-visible").Hide();
            this.Observe(dragged, item => {
                // Debug.Log($"Droptargetview:: should do something about the item {item}");
                // this.ToggleClass("drop-target-visible", item is not NoItem);
                this.SetVisible(item is not NoItem);
            });
            RegisterCallback<PointerUpEvent>(e => {

                // Debug.Log($"DroptargetView - should dispatch event");
                if (e.button != 0) return;
                if (dragged.Value is NoItem) return;
                callback(dragged.Value);
            });

        }
    }
}
