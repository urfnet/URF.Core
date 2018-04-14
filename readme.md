# URF.Core &nbsp;&nbsp; <span style="text-align: right">[![Build Status](https://travis-ci.org/urfnet/URF.Core.svg?branch=master)](https://travis-ci.org/urfnet/URF.Core) [![NuGet Badge](https://buildstats.info/nuget/URF.Core.EF.Trackable?includePreReleases=true)](https://www.nuget.org/packages?q=urf.core)</span> #

<sup>Unit-of-Work & Repository Framework | _Official_ [URF](https://github.com/urfnet), [Trackable Entities](https://github.com/TrackableEntities) & Design Factory Team</sup>

[![Build history](https://buildstats.info/travisci/chart/urfnet/URF.Core)](https://travis-ci.org/urfnet/URF.Core/builds)

##### Docs: [URF.Core.Sample](https://goo.gl/MgC4tG) | Subscribe URF Updates: [@lelong37](http://twitter.com/lelong37) | NuGet: [goo.gl/WEn7Jm](https://goo.gl/WEn7Jm) #####

### Sample & Live Demo w/ Source Code: [URF.Core.Sample](https://goo.gl/MgC4tG) ###

#### URF.Core RC2 is Complete
URF.Core is feature complete and now has full parity with URF.NET (legacy .NET). URF.Core has gone through a complete rewrite with laser focus on Architecture, Design and Implementation as well as implementing top request for vNext, you can take a look at our [URF.Core.Sample](https://github.com/urfnet/URF.Core.Sample) w/ [ASP.NET Core Web API](https://github.com/aspnet/Home), [OData](https://github.com/OData/WebApi), with full CRUD samples with [Angular](https://angular.io/) and [Kendo UI](https://www.telerik.com/kendo-angular-ui/components/).

#### Lightweight, Nano-Footprint
Staying faithful to (legacy) [URF.NET](https://github.com/urfnet/URF.NET) of having a small footprint. URF.Core [URF.Core](https://github.com/urfnet/URF.Core) (**7 total classes**) vs. [URF.NET](https://github.com/urfnet/URF.NET) (**12 total classes**).

#### 100% Extensible
 We've made every implementation virtual therefore overridable for whatever teams/projects/developer use-cases as well as edge-cases.

#### IQuerable vs. IEnumerable
As as always, this is a religous debate between teams and the within the community. As with (legacy) URF.NET, we gave teams the option to opt into IQueryable or IEnumerable, and even both depending on your teams Architecture, Design & Implementation and style. As URF.NET and for teams that feel Repository Patterns that expose `IQueryable` as a leaky  abstraction, simple use URF's `IQuery` API, which will give you all the Fluet features of IQueryable, however will return pure Entity or IEnumerable<TEntity> vs. using IQueryable, again URF.Core & URF.NET both support, so teams have the total freedom of decieding which 3 paths/options that makes the most sense for their team/project.

#### URF.Core sample and usage in ASP.NET Core Web API & OData
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

    // e.g. GET odata/Products?$skip=2&$top=10
    [EnableQuery]
    public IQueryable<Products> Get() => _productService.Queryable();

    // e.g.  GET odata/Products(37)
    public async Task<IActionResult> Get([FromODataUri] int key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.FindAsync(key);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // e.g. PUT odata/Products(37)
    public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Products products)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (key != products.ProductId)
            return BadRequest();

        _productService.Update(products);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _productService.ExistsAsync(key))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // e.g. PUT odata/Products
    public async Task<IActionResult> Post([FromBody] Products products)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _productService.Insert(products);
        await _unitOfWork.SaveChangesAsync();

        return Created(products);
    }

    // e.g. PATCH, MERGE odata/Products(37)
    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Products> product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _productService.FindAsync(key);
        if (entity == null)
            return NotFound();

        product.Patch(entity);
        _productService.Update(entity);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _productService.ExistsAsync(key))
                return NotFound();
            throw;
        }
        return Updated(entity);
    }

    // e.g. DELETE odata/Products(37)
    public async Task<IActionResult> Delete([FromODataUri] int key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.DeleteAsync(key);

        if (!result)
            return NotFound();

        await _unitOfWork.SaveChangesAsync();

        return StatusCode((int) HttpStatusCode.NoContent);
    }
}
```
#### Performance
URF.Core has been completly re-written, and everything is now completely `task`, `async`, `await` right out of the box. This way, team's will automatically get the best thread management and utilize and max out on asyncronous perf improvements.

#### URF Powered & Sponsered by:
<table border="0" style="border:0px;none">
  <tr>
    <td>    
      <img src="https://user-images.githubusercontent.com/4691404/36338938-0a7b6f7e-1380-11e8-94d1-6c308989aa6c.png" width="500px">
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/4691404/36340352-cce7fe0e-13a0-11e8-9bbb-3cf85d6d8104.png" width="450px">
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/4691404/36340407-da64cd0e-13a1-11e8-8dd9-5bb634ca0511.png" width="375px">    
    </td>
    <td>
      <img src="https://user-images.githubusercontent.com/4691404/36340442-a686c8a6-13a2-11e8-93c9-2c7db4392c52.png" width="375px">    
    </td>
  </tr>
</table>

&copy; 2017 [URF.NET](https://github.com/urfnet) All rights reserved.
