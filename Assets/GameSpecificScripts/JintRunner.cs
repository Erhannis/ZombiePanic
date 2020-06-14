using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jint;
using Jibu;
using System;
using System.Security;
using UnityEngine.XR;

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
public class JintRunner : Async {
    private string program;

    public JintRunner(string program) {
        this.program = program;
    }

    public override void Run() {
        Debug.Log("run start");
        Engine engine = new Engine(); //TODO Use EnterExecutionContext?
        engine.SetValue("log", new Func<object, bool>(log));
        engine.SetValue("Pos3", new Func<long, long, long, object>((x, y, z) => new Pos3(x, y, z))); //TODO Autoimport?
        foreach ((string, Delegate) e in getFunctions()) {
            engine.SetValue(e.Item1, e.Item2);
        }
        Debug.Log("run exec...");
        try {
            engine.Execute(program);
        } catch (Exception e) {
            handleException(e);
        }
        Debug.LogWarning("//TODO //!!! Don't forget that the program might not actually be done!  Could leave timers etc!");
        Debug.Log("run end");
    }

    protected virtual (string, Delegate)[] getFunctions() { //TODO Eh...this is kinda gross.  Couldn't pass (string, Delegate)[] into constructor, so I'm putting this here, but bleh.
        return new (string, Delegate)[0];
    }

    protected virtual void handleException(Exception e) {

    }

    private bool log(object o) {
        Debug.Log(o);
        //text.text = "" + o;
        return true;
    }
}
