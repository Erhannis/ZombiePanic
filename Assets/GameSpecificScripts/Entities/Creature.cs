using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
public abstract class Creature : Entity
{
    public List<Entity> inventory = new List<Entity>();

    //TODO HP, etc.
    public Creature() {
    }
}
}