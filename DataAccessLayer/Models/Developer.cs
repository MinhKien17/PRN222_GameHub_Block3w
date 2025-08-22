using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Developer
{
    public int DeveloperId { get; set; }

    public string DeveloperName { get; set; }

    public string Website { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
