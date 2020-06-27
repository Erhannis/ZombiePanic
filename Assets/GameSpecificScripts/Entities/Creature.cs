using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
    public abstract class Creature : Entity, Inventoried {
        public List<Entity> inventory = new List<Entity>();
        public bool alive = true;

        //TODO HP, etc.
        public Creature(Inventoried parent) : base(parent) {            
        }

        public void addItem(Entity entity) {
            inventory.Add(entity);
            entity.parent = this;
        }

        public IEnumerable<Entity> getInventory() {
            return inventory; //TODO Should not allow modification?
        }

        public bool removeItem(Entity entity) {
            entity.parent = null;
            return inventory.Remove(entity);
        }
    }
}