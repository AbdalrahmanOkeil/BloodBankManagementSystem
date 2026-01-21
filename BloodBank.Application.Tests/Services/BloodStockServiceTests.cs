namespace BloodBank.Application.Tests.Services
{
    [TestFixture]
    public class BloodStockServiceTests
    {
        private Mock<IBloodStockRepository> _bloodStockRepoMock;
        private Mock<ILogger<BloodStockService>> _loggerMock;
        private BloodStockService _service;

        [SetUp]
        public void Setup()
        {
            _bloodStockRepoMock = new Mock<IBloodStockRepository>();
            _loggerMock = new Mock<ILogger<BloodStockService>>();
            _service = new BloodStockService(_bloodStockRepoMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBloodStocks()
        {
            var stock = new List<BloodStock>()
            {
                new BloodStock
                {
                    Id = 1,
                    BloodTypeId = 1,
                    UnitsAvailable = 5,
                    BloodType = new BloodType{Name="A+"}
                },
                new BloodStock
                {
                    Id = 2,
                    BloodTypeId = 2,
                    UnitsAvailable = 3,
                    BloodType = new BloodType{Name="A"}
                },
            };
            _bloodStockRepoMock
                .Setup(r => r.GetAllAsync()).ReturnsAsync(stock);

            var result = await _service.GetAllAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("A", result.Value[1].BloodType);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoStocksExist()
        {
            _bloodStockRepoMock
                .Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BloodStock>());

            var result = await _service.GetAllAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Value.Count);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetByBloodTypeIdAsync_ShouldFail_WhenStockNotFound()
        {
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(It.IsAny<int>())).ReturnsAsync((BloodStock)null);

            var result = await _service.GetByBloodTypeIdAsync(It.IsAny<int>());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Blood stock not found!", result.Error);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetByBloodTypeIdAsync_ShouldReturnStock_WhenFound()
        {
            var stock = new BloodStock
            {
                Id = 1,
                BloodType =new BloodType { Name="A+"},
                UnitsAvailable = 0,
            };
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(It.IsAny<int>())).ReturnsAsync(stock);

            var result = await _service.GetByBloodTypeIdAsync(It.IsAny<int>());

            Assert.IsTrue(result.IsSuccess);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task IncreaseAsync_ShouldIncreaseStockSuccessfully()
        {
            var bloodTypeId = 1;
            var units = 5;
            _bloodStockRepoMock
                .Setup(r => r.IncreaseStockAsync(bloodTypeId, units)).Returns(Task.CompletedTask);
            _bloodStockRepoMock
                .Setup(r=>r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.IncreaseAsync(bloodTypeId, units);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
            _bloodStockRepoMock.Verify(r => r.IncreaseStockAsync(bloodTypeId, units), Times.Once);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DecreaseAsync_ShouldFail_WhenStockNotFound()
        {
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(It.IsAny<int>())).ReturnsAsync((BloodStock)null);
            var bloodTypeId = 1;
            var units = 5;
            _bloodStockRepoMock
                .Setup(r => r.DecreseStockAsync(bloodTypeId, units)).Returns(Task.CompletedTask);

            var result = await _service.DecreaseAsync(bloodTypeId, units);

            Assert.IsFalse(result.IsSuccess);
            _bloodStockRepoMock.Verify(r => r.DecreseStockAsync(bloodTypeId, units), Times.Never);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DecreaseAsync_ShouldFail_WhenInsufficientStock()
        {
            var bloodTypeId = 1;
            var units = 5;
            var stock = new BloodStock
            {
                Id = bloodTypeId,
                BloodType = new BloodType { Name = "A+" },
                UnitsAvailable = 0

            };
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(bloodTypeId)).ReturnsAsync(stock);
            _bloodStockRepoMock
                .Setup(r => r.IncreaseStockAsync(bloodTypeId, units)).Returns(Task.CompletedTask);

            var result = await _service.DecreaseAsync(bloodTypeId, units);

            Assert.IsFalse(result.IsSuccess);
            _bloodStockRepoMock.Verify(r => r.DecreseStockAsync(bloodTypeId, units), Times.Never);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DecreaseAsync_ShouldDecreaseStockSuccessfully()
        {
            var bloodTypeId = 1;
            var units = 5;
            var stock = new BloodStock
            {
                Id = bloodTypeId,
                BloodType = new BloodType { Name = "A+" },
                UnitsAvailable = 10
            };
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(bloodTypeId)).ReturnsAsync(stock);
            _bloodStockRepoMock
                .Setup(r => r.IncreaseStockAsync(bloodTypeId, units)).Returns(Task.CompletedTask);

            var result = await _service.DecreaseAsync(bloodTypeId, units);

            Assert.IsTrue(result.IsSuccess);
            _bloodStockRepoMock.Verify(r => r.DecreseStockAsync(bloodTypeId, units), Times.Once);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
