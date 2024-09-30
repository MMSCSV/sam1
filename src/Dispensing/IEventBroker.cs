using System;

namespace CareFusion.Dispensing
{
    public interface IEventBroker
    {
        void Subscribe(string id, Delegate method);

        void Unsubscribe(string id, Delegate method);

        void Publish(string id);

        void Publish(string id, EventArgs e);
    }
}
