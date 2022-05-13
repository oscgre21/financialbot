using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.BL.DTOs
{
    public class UsersMessages
    {
        public string UserName { get; set; }

        public DateTime SendedOnUtc { get; set; }

        public string Message { get; set; }
    }
}
