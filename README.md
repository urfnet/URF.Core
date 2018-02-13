# URF.Core
URF for .NET Standard and EF Core

## Status: URF.Core Alpha is Complete...!!!
URF.Core is feature complete and now has full parity with URF.NET (legacy .NET). URF.Core has gone through a complete rewrite with laser focus on Architecture, Design and Implementation as well as implementing top request for vNext.

## Lightweight, Nano-Footprint+
Staying faithful to (legacy) [URF.NET](https://github.com/urfnet/URF.NET) of having a small footprint. URF.Core [URF.Core](https://github.com/urfnet/URF.Core) (**7 total classes**) vs. [URF.NET](https://github.com/urfnet/URF.NET) (**12 total classes**).

## 1000% Extensible
 We've made every implementation virtual therefore overridable for whatever teams/projects/developer use-cases as well as edge-cases.

```csharp
IUnitOfWork
IRepository<TEntity>
IService<TEntity>
```

## IQuerable vs. IQuery (IEnumerable)
As as always, this is a religous debate between teams and the within the community. As with (legacy) URF.NET, we gave teams the option to opt into IQueryable or IEnumerable, and even both depending on your teams Architecture, Design & Implementation and style. As URF.NET and for teams that feel Repository Patterns that expose `IQueryable` as a leaky  abstraction, simple use URF's `IQuery` API, which will give you all the Fluet features of IQueryable, however will return pure Entity or IEnumerable<TEntity> vs. using IQueryable, again URF.Core & URF.NET both support, so teams have the total freedom of decieding which 3 paths/options that makes the most sense for their team/project.

## Performance
URF.Core has been completly re-written, and everything is now completely `task`, `async`, `await` right out of the box. This way, team's will automatically get the best thread management and utilize and max out on asyncronous perf improvements.

![alpha-unit-and-integration-tests](https://github.com/urfnet/URF.Core/blob/master/assets/2018-02-09_0-16-15.png?raw=true)