using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common.Contract.Tests.Attributes;

public class CacheResultAttributeInterceptorTests
{
    private Mock<ITestValueProvider> valueProviderMock;
    private ITestInterface testInstance;

    [SetUp]
    public void Setup()
    {
        this.valueProviderMock = new Mock<ITestValueProvider>();
        ContainerBuilder builder = new();
        builder.RegisterInstance(this.valueProviderMock.Object);
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
            .Setup(x => x.GetValue())
            .Returns("Test");

        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(1));

        this.valueProviderMock
            .Verify(x => x.GetValue(), Times.Once);
    }
    
    [Test]
    public void SyncMethodValueProviderShouldBeCalledTwiceForTwoDifferentRequests()
    {
        this.valueProviderMock
            .Setup(x => x.GetValue())
            .Returns("Test");

        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(2));

        this.valueProviderMock
            .Verify(x => x.GetValue(), Times.Exactly(2));
    }

    [Test]
    public void SyncMethodNullResultsShouldNotBeCached()
    {
        this.valueProviderMock
            .SetupSequence(x => x.GetValue())
            .Returns((string)null)
            .Returns("Test");
        
        this.testInstance.MethodWithReturnValue(new Request(1));
        this.testInstance.MethodWithReturnValue(new Request(1));
        
        this.valueProviderMock
            .Verify(x => x.GetValue(), Times.Exactly(2));
    }

    [Test]
    public void SyncMethodWithoutReturnValuesShouldBeExecuted()
    {
        this.testInstance.MethodWithoutReturnValue(new Request(1));
        
        this.valueProviderMock
            .Verify(x => x.GetValue(), Times.Once);
    }

    [Test]
    public async Task AsyncMethodValueProviderShouldOnlyBeCalledOnceForSameRequest()
    {
        this.valueProviderMock
            .Setup(x => x.GetValueAsync())
            .ReturnsAsync("Test");

        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(1));

        this.valueProviderMock
            .Verify(x => x.GetValueAsync(), Times.Once);
    }
    
    [Test]
    public async Task AsyncMethodValueProviderShouldBeCalledTwiceForTwoDifferentRequests()
    {
        this.valueProviderMock
            .Setup(x => x.GetValueAsync())
            .ReturnsAsync("Test");

        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(2));

        this.valueProviderMock
            .Verify(x => x.GetValueAsync(), Times.Exactly(2));
    }
    
    [Test]
    public async Task AsyncMethodNullResultsShouldNotBeCached()
    {
        this.valueProviderMock
            .SetupSequence(x => x.GetValueAsync())
            .ReturnsAsync((string)null)
            .ReturnsAsync("Test");
        
        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithReturnValueAsync(new Request(1));
        
        this.valueProviderMock
            .Verify(x => x.GetValueAsync(), Times.Exactly(2));
    }
    
    [Test]
    public async Task AsyncMethodWithoutReturnValueShouldNotBeCached()
    {
        await this.testInstance.MethodWithoutReturnValueAsync(new Request(1));
        await this.testInstance.MethodWithoutReturnValueAsync(new Request(1));
        
        this.valueProviderMock
            .Verify(x => x.GetValueAsync(), Times.Exactly(2));
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

    public record Request(int Number);
}