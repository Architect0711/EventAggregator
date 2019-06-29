using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventAggregator.ImplementationDetails
{
    /// <summary>
    /// This class holds a List<WeakDelegate> for every registered Event Type that gets registered with the EventAggregator,  
    /// so all Subscribers get notified in case of an Event.
    /// </summary>
    internal class WeakDelegateTable
    {
        private readonly Dictionary<Type, List<WeakDelegate>> delegateTable = new Dictionary<Type, List<WeakDelegate>>();

        internal void RegisterCallback(Type eventType, object targetClass, MethodInfo delegateMethod)
        {
            lock (delegateTable)
            {
                if (!delegateTable.ContainsKey(eventType))
                {
                    delegateTable[eventType] = new List<WeakDelegate>();
                }
                delegateTable[eventType].Add(new WeakDelegate(eventType, delegateMethod, targetClass));
            }
        }

        internal List<Delegate> GetCallbacksByEventType(Type eventType)
        {
            List<Delegate> callbacks;
            lock (delegateTable)
            {
                if (!delegateTable.ContainsKey(eventType))
                {
                    return null;
                }

                List<WeakDelegate> weakReferences = delegateTable[eventType];

                callbacks = new List<Delegate>(weakReferences.Count);
                for (int i = weakReferences.Count - 1; i > -1; --i)
                {
                    WeakDelegate weakReference = weakReferences[i];
                    if (weakReference.IsAlive)
                    {
                        callbacks.Add(weakReference.GetDelegate());
                    }
                    else
                    {
                        weakReferences.RemoveAt(i);
                    }
                }

                if (weakReferences.Count == 0)
                {
                    delegateTable.Remove(eventType);
                }
            }
            return callbacks;
        }
    }
}
