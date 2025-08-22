using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Media
{
    public int MediaId { get; set; }

    public int? GameId { get; set; }

    public byte[]? Data { get; set; }

    public string Type { get; set; }

    public DateTime UploadedAt { get; set; }

    public int UploadById { get; set; }

    public int? UserId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public virtual Game Game { get; set; }
}
