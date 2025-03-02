using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

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

    [NotMapped]
    public HashSet<string> PositionsHashSet { get => new HashSet<string>(Position.Split("/").Select(p => p.Trim())); }

    public virtual ICollection<Playersotd> Playersotds { get; set; } = new List<Playersotd>();
}
