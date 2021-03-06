﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Jibu;

public class JibuTest
{
    static ChannelWriter<string> loggingOut;

    /************************************************** 
     * The barber task.
     **************************************************/
    class Barber : Async
    {
        private ChannelReader<Customer> customers;

        public Barber(ChannelReader<Customer> customers)
        {
            this.customers = customers;
        }

        private void CutHair(Customer customer)
        {
            //Console.WriteLine("Barber: Cutting " + customer.Name);
            loggingOut.Write("Barber: Cutting " + customer.Name);
            Timer.SleepFor(300);
        }

        public override void Run()
        {
            try
            {
                while (true)
                {
                    Customer customer = customers.Read();
                    CutHair(customer);
                }
            }
            catch (PoisonException)
            {
                // if the channel is poisoned
                // we close the shop
                //Console.WriteLine("Barber shutting down after a busy day.");
                loggingOut.Write("Barber shutting down after a busy day.");
            }
        }

    }

    /************************************************** 
     * The Customer task.
     **************************************************/
    class Customer : Async
    {
        private ChannelWriter<Customer> customers;
        private String name;

        public Customer(String name, ChannelWriter<Customer> customers)
        {
            this.name = name;
            this.customers = customers;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        private void LeaveShop()
        {
            //Console.WriteLine(name + ": No seats! ");
            loggingOut.Write(name + ": No seats! ");
        }



        public override void Run()
        {
            try
            {
                // we use a Choice to listen 
                // on the Channel.
                Choice choice = new Choice(customers);

                switch (choice.TryFairSelect())
                {
                    case 0:
                        // We got a seat => sit down
                        customers.Write(this);
                        break;
                    case -1:
                        // no seats => leave shop
                        LeaveShop();
                        break;
                }

            }
            catch (PoisonException)
            {
                // if the channel has been
                // poisoned the shop must be closed
                //Console.WriteLine(name + ": Too late! Shop is closed.");
                loggingOut.Write(name + ": Too late! Shop is closed.");
                //loggingOut.Write("");
            }
        }

    }


    class Logger : Async
    {
        private ChannelReader<string> logs;
        private StringBuilder sb;

        public Logger(StringBuilder sb, ChannelReader<string> logs)
        {
            this.sb = sb;
            this.logs = logs;
        }

        public override void Run()
        {
            try
            {
                    string log = logs.Read();
                    sb.Append(log);
                    UnityEngine.Debug.Log(log);
            }
            catch (PoisonException)
            {
                // if the channel has been
                // poisoned the shop must be closed
                //Console.WriteLine(name + ": Too late! Shop is closed.");
                sb.Append("Logger poisoned");
                UnityEngine.Debug.Log("Logger poisoned");
            }
        }

    }

    StringBuilder logSb = new StringBuilder();

    public JibuTest()
    {
        Channel<string> loggingChannel = new Channel<string>(JibuBuffer.Infinite);
        loggingOut = loggingChannel.ChannelWriter; //TODO Bad
        Logger logger = new Logger(logSb, loggingChannel.ChannelReader);
        logger.Start();
    }

    /************************************************** 
     * With the CSP like channels it is possible to 
     * solve the sleeping barber problem with just one
     * Channel as synchronization device.
     * For a definition of the sleeping barber problem 
     * visit: 
     * http://en.wikipedia.org/wiki/Sleeping_barber 
     **************************************************/
    public string Main()
    {
        int numChairs = 4;

        // Create a channel with room for numChairs customers
        Channel<Customer> customers = new Channel<Customer>(JibuBuffer.Fifo, numChairs);


        // Start the barber task
        Async barber = new Barber(customers.ChannelReader).Start();

        Random rand = new Random();

        // Create 100 customers
        for (int i = 0; i < 100; i++)
        {
            Timer.SleepFor(rand.Next(300));
            new Customer("Mr. " + i, customers.ChannelWriter).Start();
        }

        // We poison the channel to shut the barber down.
        // Note that if there are any more customers
        // in the channel when it is poisoned,
        // the barber will cut them anyway.
        customers.Poison();
        barber.WaitFor();

        return logSb.ToString();
    }
}
