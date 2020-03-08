using Microsoft.EntityFrameworkCore;
using SquadEvent.SquadGameInfos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SquadEvent.Entities;

namespace SquadEvent.Entities
{
    public class SquadEventContext : DbContext
    {
        public SquadEventContext(DbContextOptions<SquadEventContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MatchUser> MatchUsers { get; set; }
        public DbSet<MatchSide> MatchSides { get; set; }
        public DbSet<Match> Matchs { get; set; }
        public DbSet<GameLayout> Layouts { get; set; }
        public DbSet<GameMap> Maps { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<RoundSide> RoundSides { get; set; }
        public DbSet<RoundSquad> RoundSquads { get; set; }
        public DbSet<RoundSlot> RoundSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<GameLayout>().ToTable("GameMap");

            modelBuilder.Entity<Match>().ToTable("Match");
            modelBuilder.Entity<MatchSide>().ToTable("MatchSide");
            modelBuilder.Entity<MatchUser>().ToTable("MatchUser");
            modelBuilder.Entity<Round>().ToTable("Round");
            modelBuilder.Entity<RoundSide>().ToTable("RoundSide");
            modelBuilder.Entity<RoundSquad>().ToTable("RoundSquad");
            modelBuilder.Entity<RoundSlot>().ToTable("RoundSlot").HasOne(t => t.Squad).WithMany(s => s.Slots).OnDelete(DeleteBehavior.Cascade); ;
        }

        internal void InitBaseData()
        {
            if (!Maps.Any())
            {
                var lines = File.ReadAllLines("SquadMaps.csv").Skip(1);
                foreach (var line in lines)
                {
                    var items = line.Split(';');
                    Maps.Add(new GameMap()
                    {
                        Name = items[0],
                        Region = Enum.Parse<GameMapRegion>(items[1]),
                    });
                }
                SaveChanges();
            }
            if (!Layouts.Any())
            {
                var lines = File.ReadAllLines("SquadLayouts.csv").Skip(1);
                foreach (var line in lines)
                {
                    var items = line.Split(';');
                    var name = items[0];
                    Layouts.Add(new GameLayout()
                    {
                        Name = name,
                        Left = !string.IsNullOrEmpty(items[1]) ? (Faction?)Enum.Parse<Faction>(items[1]) : null,
                        Right = !string.IsNullOrEmpty(items[2]) ? (Faction?)Enum.Parse<Faction>(items[2]) : null,
                        Thumbnail = items[3],
                        MapFull = items[4],
                        GameMap = Maps.FirstOrDefault(m => name.Contains(m.Name)) ?? Maps.FirstOrDefault(m => m.Region == GameMapRegion.Training)
                    });
                }
                SaveChanges();
            }

            if (!Users.Any())
            {
                for(int i=1;i<=80;++i)
                {
                    Users.Add(new User() { Name = $"User {i}", SteamId = "XXXXXXXXX" });
                    SaveChanges();
                }
            }
        }

    }
}
