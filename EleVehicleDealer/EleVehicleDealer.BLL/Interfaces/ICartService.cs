using EleVehicleDealer.Domain.DTOs.Cart;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();
        Task AddToCartAsync(int vehicleId, int quantity = 1, string imageName = "default.jpg");
        Task RemoveFromCartAsync(int vehicleId);
        Task UpdateQuantityAsync(int vehicleId, int quantity);
        Task ClearCartAsync();
    }
}
