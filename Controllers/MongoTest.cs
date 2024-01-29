using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace APIdi.Controllers
{
    [ApiController]
[Route("api/collation")]
public class collationController : ControllerBase
{
    private readonly MongoDbContext _context;

    public collationController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var collation = _context.Collations.Find(_ => true).ToList();
        return Ok(collation);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(string id)
    {
        var collation = _context.Collations.Find(p => p.Id == ObjectId.Parse(id)).FirstOrDefault();
        if (collation == null)
            return NotFound();

        return Ok(collation);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] Collation collation)
    {
        _context.Collations.InsertOne(collation);
        return CreatedAtAction(nameof(GetProduct), new { id = collation.Id }, collation);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(string id, [FromBody] Collation collation)
    {
        var filter = Builders<Collation>.Filter.Eq(p => p.Id, ObjectId.Parse(id));
        var update = Builders<Collation>.Update
            .Set(p => p.Name, collation.Name)
            .Set(p => p.Price, collation.Price);

        var result = _context.Collations.UpdateOne(filter, update);

        if (result.ModifiedCount == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(string id)
    {
        var result = _context.Collations.DeleteOne(p => p.Id == ObjectId.Parse(id));

        if (result.DeletedCount == 0)
            return NotFound();

        return NoContent();
    }
}


}


