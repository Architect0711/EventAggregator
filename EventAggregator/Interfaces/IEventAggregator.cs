using System;

namespace EventAggregator.Interfaces
{
    /// <summary>
    /// Interface for the Nonstatic EventAggregator Implementations for Dependency Injection
    /// </summary>
    public interface IEventAggregator
    {
        void Subscribe<T>(Action<T> callback) where T : IEvent;

        void Publish<T>(T args) where T : IEvent;
    }
}
