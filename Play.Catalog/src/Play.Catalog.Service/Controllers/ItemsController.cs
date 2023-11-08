using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;
using MassTransit;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
[Route("Items")]
    public class ItemController : ControllerBase
    {
       private readonly IRepository<Item> itemsRepository;

       private readonly IPublishEndpoint publishEndpoint;

       public ItemController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
       {
        this.itemsRepository=itemsRepository;
        this.publishEndpoint=publishEndpoint;
       }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {   
         

            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item=await itemsRepository.GetAsync(id);

            if(item == null){
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {

            var item =new Item
            {
                Name =createItemDto.Name,
                Description =createItemDto.Description,
                Price=createItemDto.Price,
                CreateDate=DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id,item.Name,item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id} , item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDto>> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {   
            var existingitem = await itemsRepository.GetAsync(id);

            if(existingitem == null)
            {
                return NotFound();
            }
           
           existingitem.Name = updateItemDto.Name;
           existingitem.Description = updateItemDto.Description;
           existingitem.Price = updateItemDto.Price;

           await itemsRepository.UpdateAsync(existingitem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingitem.Id,existingitem.Name,existingitem.Description));

           return NoContent();
        }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ItemDto>> DeleteAsync(Guid id){

        var item =await itemsRepository.GetAsync(id);

        if(item == null)
        {
            return NotFound();
        }

        await itemsRepository.RemoveAsync(item.Id);

        await publishEndpoint.Publish(new CatalogItemDeleted(id));


        return NoContent();

    }
        
}

    public class InterfaceItemRepository
    {
    }
}


