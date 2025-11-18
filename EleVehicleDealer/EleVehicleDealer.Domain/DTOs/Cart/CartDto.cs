using System.Collections.Generic;
using System.Linq;

namespace EleVehicleDealer.Domain.DTOs.Cart
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        
        public int TotalItems => Items.Sum(x => x.Quantity);
        
        public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
        
        public void AddItem(CartItemDto item)
        {
            var existingItem = Items.FirstOrDefault(x => x.VehicleId == item.VehicleId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        
        public void RemoveItem(int vehicleId)
        {
            var item = Items.FirstOrDefault(x => x.VehicleId == vehicleId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
        
        public void UpdateQuantity(int vehicleId, int quantity)
        {
            var item = Items.FirstOrDefault(x => x.VehicleId == vehicleId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }
        
        public void Clear()
        {
            Items.Clear();
        }
    }
}
