using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class GameCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
