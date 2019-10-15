using Health_Consulting_And_eChanneling.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Health_Consulting_And_eChanneling.Models.ViewModels.News
{
    public class NewsViewModel
    {
        public NewsViewModel()
        {
        }
        public NewsViewModel(NewsDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Image = row.Image;
            Content = row.Content;
        }
        public int Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 5)]
        [AllowHtml]
        public string Content { get; set; }
    }
}