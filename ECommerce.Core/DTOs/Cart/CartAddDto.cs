using ECommerce.Core.DTOs.CartLine;

namespace ECommerce.Core.DTOs.Cart
{
    public class CartAddDto
    {
        public ICollection<CartLineListDto> CartLines { get; set; } = new List<CartLineListDto>();
        public Guid AppUserId { get; set; }
    }
}
