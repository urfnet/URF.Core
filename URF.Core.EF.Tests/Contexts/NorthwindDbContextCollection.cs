using Xunit;

namespace URF.Core.EF.Tests.Contexts
{
    [CollectionDefinition(nameof(NorthwindDbContext))]
    public class NorthwindDbContextCollection : ICollectionFixture<NorthwindDbContextFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
