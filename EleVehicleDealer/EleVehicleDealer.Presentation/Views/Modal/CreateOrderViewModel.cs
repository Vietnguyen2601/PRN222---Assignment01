using System.ComponentModel.DataAnnotations;

namespace EleVehicleDealer.Presentation.Modal
{
    public class CreateOrderViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string StationName { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int StaffId { get; set; }

        public int? PromotionId { get; set; }
    }
}
