using LeagueQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace LeagueQuest.Services
{
    public class PlayerService
    {
        private readonly LeagueQuestContext _context;

        public PlayerService(LeagueQuestContext context)
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
    }
}
