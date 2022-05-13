using System.Collections.Generic;

namespace FinancialBot.BL.DTOs
{
    public class RegisterResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
