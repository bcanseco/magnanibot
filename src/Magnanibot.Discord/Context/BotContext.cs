﻿using Magnanibot.Context.Models;
using Microsoft.EntityFrameworkCore;

namespace Magnanibot.Context
{
    public class BotContext : DbContext
    {
        public DbSet<Memory> Memories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(BotTokens.MySql);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Memory>().ToTable(nameof(Memory));
        }
    }
}
