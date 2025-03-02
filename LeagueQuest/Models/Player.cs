using System;
using System.Collections.Generic;

namespace LeagueQuest.Models;

public partial class Player
{
    public string Name { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string Country { get; set; } = null!;

    public int Number { get; set; }

    public string Position { get; set; } = null!;

    public string Team { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<Playersotd> Playersotds { get; set; } = new List<Playersotd>();
}
