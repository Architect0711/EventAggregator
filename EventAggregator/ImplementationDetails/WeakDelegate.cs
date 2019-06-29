using System;
using System.Reflection;

namespace EventAggregator.ImplementationDetails
{
    internal sealed class WeakDelegate
    {
        readonly Type DelegateType;
        readonly MethodInfo DelegateFunction;
        readonly WeakReference DelegateTarget;

        public WeakDelegate(Type delegateType,
            MethodInfo delegateFunction,
            object delegateTarget)
        {
            DelegateType = typeof(Action<>).MakeGenericType(delegateType);
            DelegateFunction = delegateFunction;
            DelegateTarget = new WeakReference(delegateTarget);
        }

        public Delegate GetDelegate()
        {
            object target = DelegateTarget.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(DelegateType, target, DelegateFunction);
            }
            return null;
        }

        public bool IsAlive
        {
            get
            {
                return DelegateTarget.IsAlive;
            }
        }
    }
}
