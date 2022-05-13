using System;
using System.Collections.Generic;
using System.Text;
using FinancialBot.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialBot.Domain.Contexts
{
    public sealed class BaseDBContext : IdentityDbContext<Users>
    {
        public BaseDBContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
