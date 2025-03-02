using LeagueQuest.Data;
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

        public async Task<Player> GetRandomPlayer()
        {
            var player = await _context.Players.FirstOrDefaultAsync();
            if (player == null)
            {
                throw new Exception("No player found");
            }
            return player;
        }

        private async Task<Player> GetPlayerOTD()
        {
            Player? playerOTD = null;
            var today = DateOnly.FromDateTime(DateTime.Now);

            int? playerOTDId = await _context.Playersotds.Where(m => m.Date == today).Select(p => p.Id).FirstOrDefaultAsync();

            if (playerOTDId == null)
            {
                Random rnd = new Random();

                do
                {
                    playerOTDId = rnd.Next();
                } while (await _context.Players.AnyAsync(p => p.Id == playerOTDId));

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

            return playerOTD;
        }

        public async Task<List<Player>> SearchPlayers(string filter)
        {
            var players = new List<Player>();

            if (String.IsNullOrEmpty(filter))
            {
                players = await _context.Players
                    .Where(p => p.Name.ToLower().Contains(filter.ToLower()))
                    .OrderBy(p => p.Name)
                    .Take(10)
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
                result.Age.Guessed = player.Date == playerOTD.Date;
                result.Age.NumberClue = result.Age.Guessed ? null : player.Date > playerOTD.Date ? NumberClue.Lower : NumberClue.Lower;

                result.Country.Guessed = player.Country == playerOTD.Country;

                result.Number.Guessed = player.Number == playerOTD.Number;
                result.Number.NumberClue = result.Number.Guessed ? null : player.Number > playerOTD.Number ? NumberClue.Higher : NumberClue.Lower;

                result.Team.Guessed = player.Team == playerOTD.Team;

                var playerPositions = new HashSet<string>(player.Position.Split(",").Select(p => p.Trim()));
                var playerOTDPositions = new HashSet<string>(playerOTD.Position.Split(",").Select(p => p.Trim()));

                result.Position.Guessed = playerPositions.SetEquals(playerOTDPositions);

                if (!result.Position.Guessed)
                {
                    result.Position.PositionClue = playerPositions.Overlaps(playerOTDPositions) ? PositionClue.Medium : PositionClue.Wrong;
                }
            }

            return result;
        }
    }
}
