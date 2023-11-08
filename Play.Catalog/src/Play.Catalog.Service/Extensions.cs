
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item Item)
        {
            return new ItemDto(Item.Id,Item.Name,Item.Description,Item.Price,Item.CreateDate);
        }
    }
}