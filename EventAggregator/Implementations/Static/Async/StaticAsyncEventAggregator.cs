﻿using EventAggregator.ImplementationDetails;
using EventAggregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace EventAggregator.Implementations.Static.Async
{
    /// <summary>
    /// Asynchronous Static Implementation of the EventAggregator Pattern
    /// Stores Weak References to the Callbacks to prevent Memory Leaks 
    /// Runs the Callbacks asynchronously
    /// </summary>
    public static class StaticAsyncEventAggregator
    {
        private static WeakDelegateTable callbacks;

        /// <summary>
        /// Pass an IEvent Implementation to this Function to notify all Subscribers of an Event
        /// Runs the Callbacks asynchronously
        /// </summary>
        /// <typeparam name="T">The Type of the IEvent Implementation</typeparam>
        /// <param name="args">The IEvent Implementation</param>
        public static void Publish<T>(T args) where T : IEvent
        {
            if (callbacks != null)
            {
                var callbacksByEvent = callbacks.GetCallbacksByEventType(args.GetType());
                foreach (var callback in callbacksByEvent)
                {
                    Task.Run(() => (callback.DynamicInvoke(args)));
                }
            }
        }

        /// <summary>
        /// Register a Method that takes an IEvent Implementation as (only) Parameter
        /// </summary>
        /// <typeparam name="T">The Type of the IEvent Implementation</typeparam>
        /// <param name="callback">The Function that processes the IEvents</param>
        public static void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            if (callbacks == null)
            {
                callbacks = new WeakDelegateTable();
            }
            callbacks.RegisterCallback(typeof(T), callback.Target, callback.Method);
        }
    }
}
