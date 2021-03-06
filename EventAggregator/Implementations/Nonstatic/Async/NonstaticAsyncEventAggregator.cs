﻿using EventAggregator.ImplementationDetails;
using EventAggregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace EventAggregator.Implementations.Nonstatic.Async
{
    /// <summary>
    /// Asynchronous Nonstatic Implementation of the EventAggregator Pattern
    /// Stores Weak References to the Callbacks to prevent Memory Leaks 
    /// Meant to be used with Dependency Injection
    /// Register as Singleton
    /// Runs the Callbacks asynchronously
    /// </summary>
    public sealed class NonstaticAsyncEventAggregator : IEventAggregator
    {
        private static WeakDelegateTable callbacks;

        /// <summary>
        /// Pass an IEvent Implementation to this Function to notify all Subscribers of an Event
        /// Runs the Callbacks asynchronously
        /// </summary>
        /// <typeparam name="T">The Type of the IEvent Implementation</typeparam>
        /// <param name="args">The IEvent Implementation</param>
        public void Publish<T>(T args) where T : IEvent
        {
            if (callbacks != null)
            {
                var callbacksByEvent = callbacks.GetCallbacksByEventType(args.GetType());
                if (callbacksByEvent != null)
                {
                    foreach (var callback in callbacksByEvent)
                    {
                        Task.Run(() => (callback.DynamicInvoke(args)));
                    }
                }
            }
        }

        /// <summary>
        /// Register a Method that takes an IEvent Implementation as (only) Parameter
        /// </summary>
        /// <typeparam name="T">The Type of the IEvent Implementation</typeparam>
        /// <param name="callback">The Function that processes the IEvents</param>
        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            if (callbacks == null)
            {
                callbacks = new WeakDelegateTable();
            }
            callbacks.RegisterCallback(typeof(T), callback.Target, callback.Method);
        }
    }
}
