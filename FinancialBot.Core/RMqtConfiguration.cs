using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.Core
{
    public class RMqtConfiguration
    {
        public string HostName { get; set; }

        public string QueueName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ListenToQueueName { get; set; }
    }
}
