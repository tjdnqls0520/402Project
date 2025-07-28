using System;
using UnityEngine;
using GDS.Core.Events;
using GDS.Demos.Basic.Events;

namespace GDS.Demos.Basic {
    // Publishes events on key press
    // Events are listened by the Store
    public class InventoryInput : MonoBehaviour {

        Action<CustomEvent> Publish = Store.Bus.Publish;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Publish(new HotbarUse(0));
            if (Input.GetKeyDown(KeyCode.Alpha2)) Publish(new HotbarUse(1));
            if (Input.GetKeyDown(KeyCode.Alpha3)) Publish(new HotbarUse(2));
            if (Input.GetKeyDown(KeyCode.Alpha4)) Publish(new HotbarUse(3));
            if (Input.GetKeyDown(KeyCode.Alpha5)) Publish(new HotbarUse(4));

            if (Input.GetKeyDown(KeyCode.C)) Publish(new ToggleCharacterSheet());
            if (Input.GetKeyDown(KeyCode.I)) Publish(new ToggleInventory());
            if (Input.GetKeyDown(KeyCode.Tab)) Publish(new ToggleInventory());
            if (Input.GetKeyDown(KeyCode.Escape)) Publish(new CloseInventory());
        }
    }
}