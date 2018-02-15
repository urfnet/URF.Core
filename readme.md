# URF.Core #
**_<sup>Unit-of-Work & Repository Framework | Official URF Team & [Trackable Entities](https://github.com/TrackableEntities) Team</sup>_**

[![Build Status](https://travis-ci.org/urfnet/URF.Core.svg?branch=master)](https://travis-ci.org/urfnet/URF.Core)
### Docs: [comming soon](https://goo.gl/6zh9zp) | Subscribe URF Updates: [@lelong37](http://twitter.com/lelong37) | NuGet: [goo.gl/WEn7Jm](https://goo.gl/WEn7Jm) ###

Unit of Work and Repository Framework for .NET Core, NET Standard & EntityFramework Core

## URF.Core Beta is Complete...!
URF.Core is feature complete and now has full parity with URF.NET (legacy .NET). URF.Core has gone through a complete rewrite with laser focus on Architecture, Design and Implementation as well as implementing top request for vNext.

## Lightweight, Nano-Footprint
Staying faithful to (legacy) [URF.NET](https://github.com/urfnet/URF.NET) of having a small footprint. URF.Core [URF.Core](https://github.com/urfnet/URF.Core) (**7 total classes**) vs. [URF.NET](https://github.com/urfnet/URF.NET) (**12 total classes**).

## 100% Extensible
 We've made every implementation virtual therefore overridable for whatever teams/projects/developer use-cases as well as edge-cases.


## IQuerable vs. IEnumerable
As as always, this is a religous debate between teams and the within the community. As with (legacy) URF.NET, we gave teams the option to opt into IQueryable or IEnumerable, and even both depending on your teams Architecture, Design & Implementation and style. As URF.NET and for teams that feel Repository Patterns that expose `IQueryable` as a leaky  abstraction, simple use URF's `IQuery` API, which will give you all the Fluet features of IQueryable, however will return pure Entity or IEnumerable<TEntity> vs. using IQueryable, again URF.Core & URF.NET both support, so teams have the total freedom of decieding which 3 paths/options that makes the most sense for their team/project.

### URF sample and usage in ASP.NET Core Web API & OData
```csharp
public class ProductsController : ODataController
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(
        IProductService productService,
        IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    [EnableQuery]
    public IQueryable<Products> Get()
    {
        return _productService.Queryable();
    }

    public async Task<IActionResult> Get([FromODataUri] int key)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var products = await _productService.Queryable().SingleOrDefaultAsync(m => m.ProductId == key);

        if (products == null)
        {
            return NotFound();
        }

        return Ok(products);
    }

    public async Task<IActionResult> Put(int key, [FromBody] Products products)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (key != products.ProductId)
        {
            return BadRequest();
        }

        products.TrackingState = TrackingState.Modified;

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductsExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    public async Task<IActionResult> Post(Products products)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _productService.Insert(products);
        await _unitOfWork.SaveChangesAsync();

        return Created(products);
    }

    public async Task<IActionResult> Delete(int key)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var products = await _productService.Queryable().SingleOrDefaultAsync(m => m.ProductId == key);

        if (products == null)
        {
            return NotFound();
        }

        _productService.Delete(products);
        await _unitOfWork.SaveChangesAsync();

        return StatusCode((int) HttpStatusCode.NoContent);
    }

    private bool ProductsExists(int id)
    {
        return _productService.Queryable().Any(e => e.ProductId == id);
    }
}
```
## Performance
URF.Core has been completly re-written, and everything is now completely `task`, `async`, `await` right out of the box. This way, team's will automatically get the best thread management and utilize and max out on asyncronous perf improvements.

![alpha-unit-and-integration-tests](https://user-images.githubusercontent.com/2836367/36233036-c501125a-11a9-11e8-972f-8c673534760a.png)
