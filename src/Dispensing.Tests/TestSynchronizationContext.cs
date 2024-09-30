using System;
using System.Collections.Generic;
using System.Threading;

namespace CareFusion.Dispensing.Test
{
    public class TestSynchronizationContext : SynchronizationContext
    {
        readonly Queue<Action> _messagesToProcess = new Queue<Action>();
        readonly object _syncHandle = new object();
        bool _isRunning = true;

        public override void Send(SendOrPostCallback codeToRun, object state)
        {
            lock (_syncHandle)
            {
                codeToRun(state);
            }
        }

        public override void Post(SendOrPostCallback codeToRun, object state)
        {
            lock (_syncHandle)
            {
                _messagesToProcess.Enqueue(() => codeToRun(state));
                SignalContinue();
            }
        }

        public void RunMessagePump()
        {
            while (CanContinue())
            {
                Action nextToRun = GrabItem();
                nextToRun();
            }
        }

        private Action GrabItem()
        {
            lock (_syncHandle)
            {
                while (CanContinue() && _messagesToProcess.Count == 0)
                {
                    Monitor.Wait(_syncHandle);
                }
                return _messagesToProcess.Dequeue();
            }
        }

        private bool CanContinue()
        {
            lock (_syncHandle)
            {
                return _isRunning;
            }
        }

        public void Cancel()
        {
            lock (_syncHandle)
            {
                _isRunning = false;
                SignalContinue();
            }
        }

        private void SignalContinue()
        {
            Monitor.Pulse(_syncHandle);
        }

    }
}
