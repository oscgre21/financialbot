using FinancialBot.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.Core.Repository
{
    public interface IMqtBot
    {
        StockModel GetStockQuote(UsersMessages message);
    }
}
