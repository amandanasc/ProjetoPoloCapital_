using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace ExpectativaMensal.Models
{
    internal class AppDbContext : DbContext
    {
        public System.Data.Entity.DbSet<ExpectativaMercado> ExpectativaMercadoMensal { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ExpectativaMercadoMensal;Trusted_Connection=True;");
            }
        }
    }
}
