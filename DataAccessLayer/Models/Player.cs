using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public int? UserId { get; set; }

    public string Username { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<PlayerLibrary> PlayerLibraries { get; set; } = new List<PlayerLibrary>();

    public virtual User User { get; set; }
}
