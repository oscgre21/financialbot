using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.BL.DTOs
{
    public class UsersMessages
    {
        public string username { get; set; }

        public DateTime sendedDateUtc { get; set; }

        public string Message { get; set; }

        public string group { get; set; } = "Global";
    }
}
