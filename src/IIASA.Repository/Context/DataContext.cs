using System;
using System.Collections.Generic;
using System.Text;
using IIASA.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace IIASA.Repository.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}
