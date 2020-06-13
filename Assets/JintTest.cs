﻿using Jint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Jibu;

public class JintTest : MonoBehaviour
{
    public Text text;

//    ObjectWithEvent m_ObjectWithEvent = new ObjectWithEvent();

    private void Start()
    {
        //text.text = new JibuTest().Main();
        new JintRunner(
@"for (int i = 0; i < 10; i++) {
    move(Pos3(1,0,0));
}"
        ).Start();
    }

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

    class JintRunner : Async {
        private string program;
        private ChannelReader<int> sync;

        public JintRunner(ChannelReader<int> sync, string program) {
            this.program = program;
            this.sync = sync;
        }

        public override void Run() {
            Engine engine = new Engine(); //TODO Use EnterExecutionContext?
            engine.SetValue("log", new Func<object, bool>(log));
            engine.SetValue("Pos3", new Func<long, long, long, object>((x,y,z) => new Pos3(x,y,z))); //TODO Autoimport?
            engine.SetValue("move", new Func<Pos3, bool>(move));
            engine.Execute(program);
            Debug.LogWarning("//TODO //!!! Don't forget that the program might not actually be done!  Could leave timers etc!");
        }

        private bool log(object o) {
            Debug.Log(o);
            //text.text = "" + o;
            return true;
        }

        private bool move(Pos3 dir) {
            if (checkDead()) { //TODO ???
                return false;
            }
            int val = sync.Read();
            return true;
        }

        private bool checkDead() { //TODO ???
            return false;
        }
    }

/*    private void AsyncStuff()
    {

    }

    private async void doSomething()
    {
        Debug.Log("1");
        int result = await Task.Run(() => subSomething());

    }

    private int subSomething()
    {
        return 5;
    }

    private void EngineStuff()
    {
        Engine engine = new Engine();

        engine.SetValue("log", new Func<object, bool>(log));
        engine.SetValue("objectWithEvent", m_ObjectWithEvent);

        engine.SetValue("p", new Pos3(1, 2, 3));
        engine.Execute("log(p.x + '/' + p.y + '/' + p.z)");

        engine.Execute(
          @"objectWithEvent.add_TestEvent(testEvent);
            function testEvent(sender, eventArgs) {
              log('The testEvent was fired!');
            }");

        // Wait a couple of seconds, then raise the test event.
        Invoke(nameof(RaiseTestEvent), 2.0f);
    }

    private void RaiseTestEvent()
    {
        m_ObjectWithEvent.RaiseTestEvent();
    }

    private bool log(object o) {
        Debug.Log(o);
        text.text = "" + o;
        return true;
    }

    static IEnumerator<int> Feedthrough0()
    {
        return Feedthrough1(0);
    }

    static IEnumerator<int> Feedthrough1(int i)
    {
        yield return i;
    }


    static IEnumerator<int> Fibonacci()
    {
        int Fkm2 = 1;
        int Fkm1 = 1;
        yield return 1; // The first two values are 1
        yield return 1;
        // Now, each time we continue execution, generate the next entry.
        while (Fkm1 + Fkm2 < int.MaxValue)
        {
            int Fk = Fkm2 + Fkm1;
            Fkm2 = Fkm1;
            Fkm1 = Fk;
            yield return Fk;
        }
    }

    static void Start2()
    {
        // Call the Fibonacci function, which 
        // immediately returns an IEnumerator
        // (No code in Fibonnacci is run)
        IEnumerator<int> fib = Fibonacci();

        // Generate the first 10 Fibonacci numbers
        for (int i = 0; i < 10; i++)
        {
            // MoveNext runs the Fibonacci function
            // with the stored stack frame in the Ienumerator
            if (!fib.MoveNext())
                break;
            // Current returns the value that was yielded
            Debug.Log((int)fib.Current);
        }
    }
*/}

/*
public class ObjectWithEvent
{
    public event EventHandler TestEvent;

    public void RaiseTestEvent()
    {
        TestEvent?.Invoke(this, EventArgs.Empty);
    }
}
*/