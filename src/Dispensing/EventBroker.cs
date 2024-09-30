using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing
{
    public class EventBroker : IEventBroker
    {
        private Dictionary<string, List<Delegate>> _subscribers = new Dictionary<string,List<Delegate>>();
        private object _padlock = new object();

        public void Subscribe(string id, Delegate method)
        {
            lock (_padlock)
            {
                List<Delegate> delegateList;
                if (_subscribers.ContainsKey(id))
                {
                    delegateList = _subscribers[id];
                }
                else
                {
                    delegateList = new List<Delegate>();
                    _subscribers[id] = delegateList;
                }
                delegateList.Add(method);
            }
        }

        public void Unsubscribe(string id, Delegate method)
        {
            lock (_padlock)
            {
                if (!_subscribers.ContainsKey(id))
                    return;
                List<Delegate> delegateList = _subscribers[id];
                delegateList.Remove(method);
            }
        }

        public void Publish(string id, EventArgs e)
        {
            List<Delegate> delegateList;
            lock (_padlock)
            {
                if (!_subscribers.ContainsKey(id))
                    return;
                delegateList = _subscribers[id];
            }
            foreach (var d in delegateList)
            {
                try
                {
                    d.DynamicInvoke(e);
                }
                catch
                {
                }
            }
        }

        public void Publish(string id)
        {
            List<Delegate> delegateList;
            lock (_padlock)
            {
                if (!_subscribers.ContainsKey(id))
                    return;
                delegateList = _subscribers[id];
            }
            foreach (var d in delegateList)
            {
                try
                {
                    d.DynamicInvoke();
                }
                catch
                {
                }
            }
        }
    }
}
