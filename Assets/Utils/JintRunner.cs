using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jint;
using Jibu;
using System;
using System.Security;
using UnityEngine.XR;

//TODO ...How the heck am I gonna save state?
public class JintRunner : Async {
    public readonly string program;

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
