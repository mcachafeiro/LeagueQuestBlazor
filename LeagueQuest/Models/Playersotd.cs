using System;
using System.Collections.Generic;

namespace LeagueQuest.Models;

public partial class Playersotd
{
    public int Id { get; set; }

    public string Date { get; set; } = null!;

    public virtual Player IdNavigation { get; set; } = null!;
}
