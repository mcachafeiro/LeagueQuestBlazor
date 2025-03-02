using System;
using System.Collections.Generic;

namespace LeagueQuest.Models;

public partial class Playersotd
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public virtual Player IdNavigation { get; set; } = null!;
}
