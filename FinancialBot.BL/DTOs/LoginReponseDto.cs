﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialBot.BL.DTOs
{
    public class LoginReponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}
