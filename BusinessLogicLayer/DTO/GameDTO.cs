using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.DTO
{
    public class MediaDTO
    {
        public int? MediaId { get; set; }
        public string FilePath { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
    public class GameDTO
    {
        public int? GameId { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        // Category
        [Required]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        // Developer
        [Required]
        public int DeveloperId { get; set; }
        public string? DeveloperName { get; set; }
        public string? Website { get; set; }
        // Img and Video
        public List<MediaDTO>? Images { get; set; } = new();
        public List<MediaDTO>? Videos { get; set; } = new();
        public IFormFileCollection? ImageFiles { get; set; }
        public IFormFileCollection? VideoFiles { get; set; }
        // Upload Info
        [Required]
        public int UploadById { get; set; }
        // Update
        public List<int>? RemoveMediaIds { get; set; } = new();
    }
}
