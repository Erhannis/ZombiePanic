using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jint;
using Jibu;
using System;

/*
Ideas for interface
Ugh, making this "safe" is gonna be hard - maybe infeasible?  Embrace it?

params?

move(dir) -> bool
get(dir) -> bool
put(dir) -> bool
canMove(dir) -> bool
feel(dir) -> ???
look(dir) -> ???
smell(???)??? -> ??? // See `pheromone`
inventory() -> ???
invCount() -> int
sleep(turns) -> bool?
getAge()??? -> int
getTime()??? -> int
speak(???)??? -> ??? //TODO Now, surely this must be "chitter" or "pheromone" or something.  :)
broadcast(???)??? -> ??? //TODO Ditto
listen(???)??? -> ??? //TODO Ditto
pheromone(???)??? -> ??? // This could be for marking tiles
home()??? -> pos3
die()???

debugging?
getError()
getOrders()
getState()
getLogs()
reset()
resorb() // decommission, kill, eat, etc.  ..."filicide" acheivement, horrible crunching noise???  0_0
carry()

*/

//TODO ...How the heck am I gonna save state?
class JintRunner : Async {
    private string program;
    private ChannelReader<int> syncA;
    private ChannelWriter<int> syncB;

    public JintRunner(ChannelReader<int> syncA, ChannelWriter<int> syncB, string program) {
        this.program = program;
        this.syncA = syncA;
        this.syncB = syncB;
    }

    public override void Run() {
        Debug.Log("run start");
        Engine engine = new Engine(); //TODO Use EnterExecutionContext?
        engine.SetValue("log", new Func<object, bool>(log));
        engine.SetValue("Pos3", new Func<long, long, long, object>((x, y, z) => new Pos3(x, y, z))); //TODO Autoimport?
        engine.SetValue("move", new Func<Pos3, bool>(move));
        Debug.Log("run exec...");
        try {
            engine.Execute(program);
        } catch (Exception e) {
            Debug.LogError("run error! " + e);
            syncA.Poison();
            syncB.Poison();
            //TODO Poison?
        }
        Debug.LogWarning("//TODO //!!! Don't forget that the program might not actually be done!  Could leave timers etc!");
        Debug.Log("run end");
    }

    private bool log(object o) {
        Debug.Log(o);
        //text.text = "" + o;
        return true;
    }

    private bool move(Pos3 dir) {
        Debug.Log("move 0");
        if (checkDead()) { //TODO ???
            return false;
        }
        Debug.Log("move start");
        int val = syncA.Read();
        //TODO Do something?
        Debug.Log("move " + dir);
        syncB.Write(val);
        Debug.Log("move end");
        return true;
    }

    private bool checkDead() { //TODO ???
        return false;
    }
}
