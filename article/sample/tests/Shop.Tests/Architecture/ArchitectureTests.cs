using NetArchTest.Rules;
using Shop.Features.Orders;

namespace Shop.Tests.Architecture;

/// <summary>
/// The project's structure, enforced rather than described.
/// If one of these fails, move the code — don't change the rule.
/// </summary>
public sealed class ArchitectureTests
{
    private static Types ShopTypes => Types.InAssembly(typeof(OrderStore).Assembly);

    [Fact]
    public void TypeNames_SayWhatTheTypeDoes() => AssertPasses(ShopTypes
        .ShouldNot().HaveNameMatching("(Manager|Helper|Helpers|Util|Utils)$")
        .GetResult());

    [Fact]
    public void Shared_DoesNotDependOnAnyFeature() => AssertPasses(ShopTypes
        .That().ResideInNamespace("Shop.Shared")
        .ShouldNot().HaveDependencyOn("Shop.Features")
        .GetResult());

    [Fact]
    public void Handlers_DoNotKnowAboutHttp() => AssertPasses(ShopTypes
        .That().HaveNameEndingWith("Handler")
        .ShouldNot().HaveDependencyOn("Microsoft.AspNetCore")
        .GetResult());

    [Fact]
    public void Handlers_LiveInTheSameNamespaceAsTheirRequest()
    {
        var handlers = ShopTypes.That().HaveNameEndingWith("Handler").GetTypes();

        var orphans = handlers
            .Where(handler => handler.Assembly.GetType($"{handler.Namespace}.{handler.Name[..^"Handler".Length]}") is null)
            .Select(handler => handler.FullName);

        Assert.Empty(orphans);
    }

    private static void AssertPasses(NetArchTest.Rules.TestResult result) =>
        Assert.True(result.IsSuccessful, $"Violated by: {string.Join(", ", result.FailingTypeNames ?? [])}");
}
