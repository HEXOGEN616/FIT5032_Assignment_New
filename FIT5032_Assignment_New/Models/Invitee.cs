namespace FIT5032_Assignment_New.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invitee")]
    public partial class Invitee
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Status { get; set; }

        public int InvitationId { get; set; }

        public virtual Location Location { get; set; }
    }

    //public enum StatusList
    //{
    //    accepted,
    //    pending,
    //    declined
    //}
}
