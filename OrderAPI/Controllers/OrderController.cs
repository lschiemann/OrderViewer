using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OrderAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class OrderController : ControllerBase
  {
    private readonly Context _context;

    public OrderController(Context context)
    {
      _context = context;
    }

    [HttpGet("openOrders")]
    public IEnumerable<Order> GetOpenOrders()
    {
      var orders = _context.Orders.Where(o => !o.IsTransfered);
      if (orders.Count() != 0)
      {
        return orders;
      }

      var lastId = _context.Orders.Select(o => o.Id).Max();
      var data = DataGenerator.Create(lastId);
      _context.OrderFiles.AddRange(data.Files);
      _context.Orders.AddRange(data.Orders);
      _context.SaveChanges();

      return data.Orders.Where(o => !o.IsTransfered);
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
      var orderFile = _context.OrderFiles.Find(id);
      if (orderFile == null)
      {
        return NotFound();
      }

      var filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Data", orderFile.FileName));

      if (!System.IO.File.Exists(filePath))
        return NotFound();

      var memory = new MemoryStream();
      await using (var stream = new FileStream(filePath, FileMode.Open))
      {
        await stream.CopyToAsync(memory);
      }
      memory.Position = 0;

      return File(memory, GetContentType(filePath), filePath);
    }

    [HttpPost("ship")]
    public async Task<IActionResult> SetShipped([FromBody] int id)
    {
      var order = await _context.Orders.FindAsync(id);
      if (order == null)
      {
        return NotFound();
      }

      order.IsShipped = true;

      await _context.SaveChangesAsync();

      return Ok();
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> SetTransfered([FromBody] int id)
    {
      var order = await _context.Orders.FindAsync(id);
      if (order == null)
      {
        return NotFound();
      }

      order.IsTransfered = true;

      await _context.SaveChangesAsync();

      return Ok();
    }

    private string GetContentType(string path)
    {
      var provider = new FileExtensionContentTypeProvider();

      if (!provider.TryGetContentType(path, out string contentType))
      {
        contentType = "application/octet-stream";
      }

      return contentType;
    }
  }
}