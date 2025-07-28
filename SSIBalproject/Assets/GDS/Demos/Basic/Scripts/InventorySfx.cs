using GDS.Core;
using GDS.Core.Events;
using GDS.Demos.Basic.Events;
using UnityEngine;


namespace GDS.Demos.Basic {
    // Plays sounds on certain events
    [RequireComponent(typeof(AudioSource))]
    public class InventorySfx : MonoBehaviour {

        AudioSource audioSource;

        public SoundList Sounds;

        [System.Serializable]
        public class SoundList {
            public AudioClip Fail;
            public AudioClip Pick;
            public AudioClip Place;
            public AudioClip Move;
            public AudioClip Buy;
            public AudioClip Sell;
            public AudioClip Craft;
        }

        void OnEnable() {
            audioSource = GetComponent<AudioSource>();
            var bus = Store.Bus;

            bus.On<Fail>(PlayClip);
            bus.On<BagFull>(PlayClip);
            bus.On<CantAfford>(PlayClip);
            bus.On<CantFitAll>(PlayClip);
            bus.On<Restricted<Bag>>(PlayClip);
            bus.On<Restricted<Slot>>(PlayClip);

            bus.On<PickItemSuccess>(PlayClip);
            bus.On<PlaceItemSuccess>(PlayClip);
            bus.On<MoveItemSuccess>(PlayClip);
            bus.On<CraftItemSuccess>(PlayClip);
            bus.On<BuyItemSuccess>(PlayClip);
            bus.On<SellItemSuccess>(PlayClip);
            bus.On<DestroyItemSuccess>(PlayClip);
            bus.On<DiscardItemSuccess>(PlayClip);
            bus.On<LootItemSuccess>(PlayClip);
            bus.On<CollectAll>(PlayClip);
        }

        void OnDisable() {
            var bus = Store.Bus;
            bus.Off<Fail>(PlayClip);
            bus.Off<BagFull>(PlayClip);
            bus.Off<CantAfford>(PlayClip);
            bus.Off<CantFitAll>(PlayClip);
            bus.Off<Restricted<Bag>>(PlayClip);
            bus.Off<Restricted<Slot>>(PlayClip);

            bus.Off<PickItemSuccess>(PlayClip);
            bus.Off<PlaceItemSuccess>(PlayClip);
            bus.Off<MoveItemSuccess>(PlayClip);
            bus.Off<CraftItemSuccess>(PlayClip);
            bus.Off<BuyItemSuccess>(PlayClip);
            bus.Off<SellItemSuccess>(PlayClip);
            bus.Off<DestroyItemSuccess>(PlayClip);
            bus.Off<DiscardItemSuccess>(PlayClip);
            bus.Off<LootItemSuccess>(PlayClip);
            bus.Off<CollectAll>(PlayClip);
        }

        // TODO: this should be a dictionary
        AudioClip EventClip(CustomEvent e) => e switch {
            Fail => Sounds.Fail,
            PickItemSuccess => Sounds.Pick,
            PlaceItemSuccess => Sounds.Place,
            MoveItemSuccess => Sounds.Move,
            CraftItemSuccess => Sounds.Craft,
            BuyItemSuccess => Sounds.Buy,
            SellItemSuccess => Sounds.Sell,
            DestroyItemSuccess => Sounds.Place,
            DiscardItemSuccess => Sounds.Place,
            LootItemSuccess => Sounds.Place,
            CollectAll => Sounds.Place,
            _ => null
        };

        void PlayClip(CustomEvent e) {
            audioSource.pitch = Random.Range(0.85f, 1.05f);
            audioSource.PlayOneShot(EventClip(e));
        }

    }

}
