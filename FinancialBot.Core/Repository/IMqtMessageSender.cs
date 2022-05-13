using FinancialBot.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.Core.Repository
{
    public interface IMqtMessageSender
    {
        void SendMessage(UsersMessages message);
    }
}
