using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common.Contract.Tests.Attributes;

public class CacheResultAttributeInterceptorTests
{
    private ITestValueProvider valueProviderMock;
    private ITestInterface testInstance;

    [SetUp]
    public void Setup()
    {
        this.valueProviderMock = Substitute.For<ITestValueProvider>();
        ContainerBuilder builder = new();
        builder.RegisterInstance(this.valueProviderMock);
        builder
            .RegisterType<TestClass>()
            .AsImplementedInterfaces()
            .EnableInterfaceInterceptors();
        builder
            .RegisterType<CacheResultAttributeInterceptor>()
            .AsSelf();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        builder.Populate(serviceCollection);

        var container = builder.Build();
        this.testInstance = container.Resolve<ITestInterface>();
    }

    [Test]
    public void SyncMethodValueProviderShouldOnlyBeCalledOnceForSameRequest()
    {
        this.valueProviderMock
            .GetValue()
            .Returns("Test");

        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(1));

        this.valueProviderMock
            .Received(1)
            .GetValue();
    }

    [Test]
    public void SyncMethodValueProviderShouldBeCalledTwiceForTwoDifferentRequests()
    {
        this.valueProviderMock
            .GetValue()
            .Returns("Test");

        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(2));

        this.valueProviderMock
            .Received(2)
            .GetValue();
    }

    [Test]
    public void SyncMethodNullResultsShouldNotBeCached()
    {
        this.valueProviderMock
            .GetValue()
            .Returns(null, "Test");

        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(1));

        this.valueProviderMock
            .Received(2)
            .GetValue();
    }

    [Test]
    public void SyncMethodWithoutReturnValuesShouldBeExecuted()
    {
        this.testInstance.MethodWithoutReturnValue(new Request(1));

        this.valueProviderMock
            .Received(1)
            .GetValue();
    }

    [Test]
    public async Task AsyncMethodValueProviderShouldOnlyBeCalledOnceForSameRequest()
    {
        this.valueProviderMock
            .GetValueAsync()
            .Returns("Test");

        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(1));

        await this.valueProviderMock
            .Received(1)
            .GetValueAsync();
    }

    [Test]
    public async Task AsyncMethodValueProviderShouldBeCalledTwiceForTwoDifferentRequests()
    {
        this.valueProviderMock
            .GetValueAsync()
            .Returns("Test");

        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(2));

        await this.valueProviderMock
            .Received(2)
            .GetValueAsync();
    }

    [Test]
    public async Task AsyncMethodNullResultsShouldNotBeCached()
    {
        this.valueProviderMock
            .GetValueAsync()
            .Returns(null, "Test");

        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(1));

        await this.valueProviderMock
            .Received(2)
            .GetValueAsync();
    }

    [Test]
    public async Task AsyncMethodWithoutReturnValueShouldNotBeCached()
    {
        await this.testInstance.MethodWithoutReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithoutReturnValueAsync(new Request(1));

        await this.valueProviderMock
            .Received(2)
            .GetValueAsync();
    }

#pragma warning disable SA1201
    public interface ITestInterface
    {
        string MethodWithReturnValue(Request request);

        void MethodWithoutReturnValue(Request request);

        Task<string> MethodWithReturnValueAsync(Request request, CancellationToken cancellationToken = default);

        Task MethodWithoutReturnValueAsync(Request request, CancellationToken cancellationToken = default);
    }

    public interface ITestValueProvider
    {
        string GetValue();

        Task<string> GetValueAsync();
    }

    [Intercept(typeof(CacheResultAttributeInterceptor))]
    public class TestClass : ITestInterface
    {
        private readonly ITestValueProvider valueProvider;

        public TestClass(ITestValueProvider valueProvider)
        {
            this.valueProvider = valueProvider;
        }

        [CacheResult]
        public string MethodWithReturnValue(Request request)
        {
            return this.valueProvider.GetValue();
        }

        [CacheResult]
        public void MethodWithoutReturnValue(Request request)
        {
            this.valueProvider.GetValue();
        }

        [CacheResult]
        public async Task MethodWithoutReturnValueAsync(Request request, CancellationToken cancellationToken = default)
        {
            await this.valueProvider.GetValueAsync();
        }

        [CacheResult]
        public Task<string> MethodWithReturnValueAsync(Request request, CancellationToken cancellationToken = default)
        {
            return this.valueProvider.GetValueAsync();
        }
    }

    public record Request(int Number);
#pragma warning restore SA1201
}