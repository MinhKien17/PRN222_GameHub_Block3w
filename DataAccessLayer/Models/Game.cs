using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string Title { get; set; }

    public decimal Price { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? DeveloperId { get; set; }

    public int? CategoryId { get; set; }

    public virtual GameCategory Category { get; set; }

    public virtual Developer Developer { get; set; }

    public virtual ICollection<Media> Media { get; set; } = new List<Media>();

    public virtual ICollection<PlayerLibrary> PlayerLibraries { get; set; } = new List<PlayerLibrary>();
}
