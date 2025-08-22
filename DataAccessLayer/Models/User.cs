using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public DateTime JoinDate { get; set; }

    public string Role { get; set; }

    public virtual Player Player { get; set; }
}
