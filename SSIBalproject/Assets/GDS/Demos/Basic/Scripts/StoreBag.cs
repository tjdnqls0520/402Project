using System.Collections.Generic;
using UnityEngine;
using GDS.Core;
using GDS.Core.Events;
using GDS.Common.Scripts;

namespace GDS.Demos.Basic {
    /// <summary>
    /// Store Bag reference. Clicking the object will open the associated bag.
    /// </summary>
    public class StoreBag : BagBase {
        public string BagId;
        public List<Bag> Bags => Store.Instance.Bags;
        Bag Bag;

        void Awake() => Bag = Bags.Find(b => b.Id == BagId) ?? Bags[0];
        void OnMouseUp() => Store.Bus.Publish(new OpenWindow(Bag));
    }
}
