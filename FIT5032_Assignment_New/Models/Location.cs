namespace FIT5032_Assignment_New.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Location")]
    public partial class Location
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Location()
        {
            Invitees = new HashSet<Invitee>();
        }

        public int Id { get; set; }

        //[Required]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        //[Required]
        [Display(Name = "Event Name")]
        public string Description { get; set; }

        //[Required]
        [Column(TypeName = "numeric")]
        public decimal Latitude { get; set; }

       // [Required]
        [Column(TypeName = "numeric")]
        public decimal Longitude { get; set; }

        //[Required]
        [Display(Name = "Event Date")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string InviterId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invitee> Invitees { get; set; }
    }
}
