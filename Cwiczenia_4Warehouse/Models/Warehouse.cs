using System.ComponentModel.DataAnnotations;

namespace Cwiczenia_4Warehouse.Models;

public class Warehouse
{
    
        [Required]
        public int IdProduct { get; set; }
        [Required]
        public int IdWarehouse { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Wartość nie może być mniejsza niż 1")]
        public int Amount { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
}

