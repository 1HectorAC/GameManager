namespace GameManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Game
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public int? Price { get; set; }

        [Required]
        [StringLength(128)]
        public string SystemName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfPurchase { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DatePlayed { get; set; }

        public bool Borrowed { get; set; }

        public bool Physical { get; set; }

        public bool Replayed { get; set; }

        public virtual GameSystem GameSystem { get; set; }

        public virtual GameUser GameUser { get; set; }
    }
}
