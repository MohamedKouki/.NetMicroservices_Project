namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid UserId, Guid CatalogId, int Quantity);

    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description,int Quantity, DateTimeOffset AcquiredDate);

        public record CatalogItemDto(Guid Id, string Name, string Description);


}