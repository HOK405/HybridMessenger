using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using Moq;

namespace HybridMessenger.Tests.Infrastructure.UnitOfWork
{
    public class UnitOfWorkTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly HybridMessenger.Infrastructure.UnitOfWork.UnitOfWork _unitOfWork;
        private class DummyEntity { }

        public UnitOfWorkTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _unitOfWork = new HybridMessenger.Infrastructure.UnitOfWork.UnitOfWork(_mockServiceProvider.Object);
        }

        [Fact]
        public void GetRepository_RepositoryRegistered_ReturnsRepository()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<User>>(); 
            _mockServiceProvider.Setup(x => x.GetService(typeof(IRepository<User>)))
                                .Returns(mockRepository.Object);

            // Act
            var repository = _unitOfWork.GetRepository<IRepository<User>>();

            // Assert
            Assert.NotNull(repository);
            Assert.IsAssignableFrom<IRepository<User>>(repository);
        }


        [Fact]
        public void GetRepository_RepositoryNotRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockServiceProvider.Setup(x => x.GetService(typeof(IRepository<DummyEntity>)))
                                .Returns(null);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(_unitOfWork.GetRepository<IRepository<DummyEntity>>);
            Assert.StartsWith("Repository not registered", exception.Message);
        }
    }
}
