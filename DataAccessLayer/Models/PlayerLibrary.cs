using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class PlayerLibrary
{
    public int PlayerLibraryId { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public DateTime PurchasedAt { get; set; }

    public string Status { get; set; }

    public int TotalPlayMinutes { get; set; }

    public DateTime? LastPlayedAt { get; set; }

    public bool IsFavorite { get; set; }

    public string Notes { get; set; }

    public virtual Game Game { get; set; }

    public virtual Player Player { get; set; }
}
