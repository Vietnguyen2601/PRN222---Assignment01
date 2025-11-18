using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.DTOs.Cart;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVehicleService _vehicleService;
        private const string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor, IVehicleService vehicleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _vehicleService = vehicleService;
        }

        public async Task<CartDto> GetCartAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new CartDto();

            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartDto();
            }

            return JsonSerializer.Deserialize<CartDto>(cartJson) ?? new CartDto();
        }

        public async Task AddToCartAsync(int vehicleId, int quantity = 1, string imageName = "default.jpg")
        {
            var cart = await GetCartAsync();
            var vehicle = await _vehicleService.GetByIdAsync(vehicleId);

            if (vehicle != null)
            {
                var cartItem = new CartItemDto
                {
                    VehicleId = vehicle.VehicleId,
                    Model = vehicle.Model,
                    Type = vehicle.Type,
                    Color = vehicle.Color,
                    Price = vehicle.Price,
                    Quantity = quantity,
                    ImageName = imageName  // L?u tên ?nh vào cart item
                };

                cart.AddItem(cartItem);
                await SaveCartAsync(cart);
            }
        }

        public async Task RemoveFromCartAsync(int vehicleId)
        {
            var cart = await GetCartAsync();
            cart.RemoveItem(vehicleId);
            await SaveCartAsync(cart);
        }

        public async Task UpdateQuantityAsync(int vehicleId, int quantity)
        {
            var cart = await GetCartAsync();
            cart.UpdateQuantity(vehicleId, quantity);
            await SaveCartAsync(cart);
        }

        public async Task ClearCartAsync()
        {
            var cart = new CartDto();
            await SaveCartAsync(cart);
        }

        private Task SaveCartAsync(CartDto cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var cartJson = JsonSerializer.Serialize(cart);
                session.SetString(CartSessionKey, cartJson);
            }
            return Task.CompletedTask;
        }
    }
}
