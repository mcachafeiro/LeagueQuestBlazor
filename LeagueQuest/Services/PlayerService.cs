﻿using LeagueQuest.Data;
using LeagueQuest.DTO;
using LeagueQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueQuest.Services
{
    public class PlayerService
    {
        private readonly ApplicationDbContext _context;

        public PlayerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetYesterdaysPlayerOTD()
        {
            var yesterday = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

            var playerName = await _context.Playersotds.Where(p => p.Date == yesterday).Select(p => p.IdNavigation.Name).FirstOrDefaultAsync();

            playerName = playerName ?? string.Empty;

            return playerName;
        }

        private async Task<Player> GetPlayerOTD()
        {
            Player? playerOTD = null;
            var today = DateOnly.FromDateTime(DateTime.Now);

            int? playerOTDId = await _context.Playersotds.Where(m => m.Date == today).Select(p => p.Id).FirstOrDefaultAsync();

            if (playerOTDId == 0)
            {
                Random rnd = new Random();
                int maxId = await _context.Players.MaxAsync(p => p.Id);
                do
                {
                    playerOTDId = rnd.Next(maxId);
                } while (!await _context.Players.AnyAsync(p => p.Id == playerOTDId) && await _context.Playersotds.AnyAsync(p => p.Id == playerOTDId));
                // mirar concurrencias (si dos entran a la vez), mirar background service para que ejecute por detras
                if(playerOTDId != null)
                {
                    await _context.Playersotds.AddAsync(new Playersotd { Id = playerOTDId.Value, Date = today });
                }
            }

            playerOTD = await _context.Players.FirstOrDefaultAsync(p => p.Id == playerOTDId);


            if (playerOTD == null)
            {
                throw new Exception("No player found");
            }

            await _context.SaveChangesAsync();

            return playerOTD;
        }

        public async Task<List<Player>> SearchPlayers(string filter)
        {
            var players = new List<Player>();

            if (!String.IsNullOrWhiteSpace(filter))
            {
                players = await _context.Players
                    .Where(p => p.Name.ToLower().Contains(filter.ToLower()))
                    .OrderBy(p => p.Name)
                    .Take(7)
                    .ToListAsync();
            }

            return players;
        }

        public async Task<TryDTO> Guess(int playerId)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
            if (player == null)
            {
                throw new Exception("No player found");
            }

            var playerOTD = await GetPlayerOTD();

            var result = new TryDTO
            {
                IsPlayer = playerId == playerOTD.Id,
                Player = player,
                Age = new ClueDTO { Guessed = true },
                Country = new ClueDTO { Guessed = true },
                Number = new ClueDTO { Guessed = true },
                Team = new ClueDTO { Guessed = true },
                Position = new ClueDTO { Guessed = true }
            };

            if (!result.IsPlayer)
            {
                result.Age.Guessed = GetAge(player.Date) == GetAge(playerOTD.Date);
                result.Age.NumberClue = result.Age.Guessed ? null : player.Date > playerOTD.Date ? NumberClue.Higher : NumberClue.Lower;

                result.Country.Guessed = player.Country == playerOTD.Country;

                result.Number.Guessed = player.Number == playerOTD.Number;
                result.Number.NumberClue = result.Number.Guessed ? null : player.Number > playerOTD.Number ? NumberClue.Lower : NumberClue.Higher;

                result.Team.Guessed = player.Team == playerOTD.Team;

                var playerPositions = player.PositionsHashSet;
                var playerOTDPositions = playerOTD.PositionsHashSet;

                result.Position.Guessed = playerPositions.SetEquals(playerOTDPositions);

                if (!result.Position.Guessed)
                {
                    result.Position.PositionClue = playerPositions.Overlaps(playerOTDPositions) ? PositionClue.Medium : PositionClue.Wrong;
                }
            }

            return result;
        }
        private int GetAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
