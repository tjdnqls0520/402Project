using System;

namespace GDS.Core {
    public static class Filters {
        public static readonly Predicate<Item> Everything = _ => true;
        public static readonly Predicate<Item> Nothing = _ => false;
    }
}