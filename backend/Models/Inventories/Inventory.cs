using System.Collections.Generic;

namespace PokemonBattle.Inventories {
    
    public class Inventory<T> {

        private List<T> items;
        private int backpackSize;

        public Inventory(int backpackSize = 20) {
            items = new List<T>();
            this.backpackSize = backpackSize;
        }

        public void AddItem(T item) {
            items.Add(item);
        }

        public bool RemoveItem(T item) {
            return items.Remove(item);
        }

        public List<T> GetItems() {
            return new List<T>(items);
        }

        public int Count => items.Count;

        public bool Contains(T item) {
            return items.Contains(item);
        }

        public void Clear() {
            items.Clear();
        }
    }

}
