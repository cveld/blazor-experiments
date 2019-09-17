using BlazorServerSide.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Services
{
    public class CounterService
    {
        private int _value;
        public int Value { 
            get { return _value; }
            set
            {
                _value = value;
                this.OnDataUpdated?.Invoke(this, value);
            }
        }

        public CounterService(IQueueManager queueManager)
        {
            queueManager.MessageReceived += QueueManager_MessageReceived;
        }

        public event EventHandler<int> OnDataUpdated;

        private void QueueManager_MessageReceived(object sender, MessageReceivedArgs e)
        {
            Value++;
        }
    }
}
