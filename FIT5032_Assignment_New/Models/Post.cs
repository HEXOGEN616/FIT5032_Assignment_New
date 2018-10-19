using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace FIT5032_Assignment_New.Models
{
    public class Post
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }

        [Display(Name = "Post Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PostDate { get; set; }
        public string AuthorId { get; set; }

        [DataType(dataType: DataType.MultilineText)]
        [Required]
        [AllowHtml]
        public string Content { get; set; }
    }

    public class PostDBContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
    }
}