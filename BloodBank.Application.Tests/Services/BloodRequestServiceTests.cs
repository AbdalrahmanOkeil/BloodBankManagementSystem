namespace BloodBank.Application.Tests.Services
{
    [TestFixture]
    public class BloodRequestServiceTests
    {
        private Mock<IBloodRequestRepository> _bloodRequestRepoMock;
        private Mock<IHospitalRepository> _hospitalRepoMock;
        private Mock<IBloodStockRepository> _bloodStockRepoMock;
        private Mock<ILogger<BloodRequestService>> _loggerMock;
        private BloodRequestService _service;


        [SetUp]
        public void Setup()
        {
            _bloodRequestRepoMock = new Mock<IBloodRequestRepository>();
            _hospitalRepoMock = new Mock<IHospitalRepository>();
            _bloodStockRepoMock = new Mock<IBloodStockRepository>();
            _loggerMock = new Mock<ILogger<BloodRequestService>>();

            _service = new BloodRequestService(
            _bloodRequestRepoMock.Object,
            _hospitalRepoMock.Object,
            _bloodStockRepoMock.Object,
            _loggerMock.Object
            );
        }

        [Test]
        public async Task CreateAsync_HospitalNotFound_ReturnsFailure()
        {
            //Arrange
            var userId = "user123";
            var dto = new CreateBloodRequestDTO
            {
                BloodTypeId = 1,
                Units = 2,
                UrgencyLevel = "High"
            };

            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((Hospital)null);

            //Act
            var result = await _service.CreateAsync(userId, dto);

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Hospital not found!", result.Error);
            _bloodRequestRepoMock.Verify(r => r.AddAsync(It.IsAny<BloodRequest>()), Times.Never);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task CreateAsync_NotEnoughStock_StatusPending()
        {
            var userId = "user123";
            var hospital = new Hospital { Id = Guid.NewGuid() };
            var dto = new CreateBloodRequestDTO
            {
                BloodTypeId = 1,
                Units = 5,
                UrgencyLevel = "High"
            };

            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(hospital);
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(dto.BloodTypeId))
                .ReturnsAsync(new BloodStock { BloodTypeId = dto.BloodTypeId, UnitsAvailable = 3 });
            BloodRequest request = null;
            _bloodRequestRepoMock.Setup(r => r.AddAsync(It.IsAny<BloodRequest>()))
                .Callback<BloodRequest>(r =>
                {
                    request = r;
                    request.BloodType = new BloodType { Name = "A+" };

                }).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(userId, dto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(RequestStatus.Pending.ToString(), result.Value.Status);
            Assert.AreEqual(RequestStatus.Pending, request.Status);
            _bloodRequestRepoMock.Verify(r => r.AddAsync(It.IsAny<BloodRequest>()), Times.Once());
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Test]
        public async Task CreateAsync_EnoughStock_AutoApprove()
        {
            //Arrange
            var userId = "user123";

            var hospital = new Hospital { Id = Guid.NewGuid() };
            var stock = new BloodStock
            {
                BloodTypeId = 1,
                UnitsAvailable = 10
            };

            var dto = new CreateBloodRequestDTO
            {
                BloodTypeId = 1,
                Units = 2,
                UrgencyLevel = "High"
            };

            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(hospital);
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(dto.BloodTypeId))
                .ReturnsAsync(stock);
            _bloodRequestRepoMock
                .Setup(r => r.AddAsync(It.IsAny<BloodRequest>()))
                .Callback<BloodRequest>(r =>
                {
                    r.BloodType = new BloodType
                    {
                        Id = 1,
                        Name = "A+"
                    };
                })
                .Returns(Task.CompletedTask);

            //Act
            var result = await _service.CreateAsync(userId, dto);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Approved", result.Value.Status);
            Assert.AreEqual(8, stock.UnitsAvailable);
            _bloodRequestRepoMock.Verify(r => r.AddAsync(It.IsAny<BloodRequest>()), Times.Once);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ApproveAsync_RequestNotFound_ReturnsFailure()
        {
            //Arrange
            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BloodRequest)null);

            //Act
            var result = await _service.ApproveAsync(Guid.NewGuid());

            //Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Blood request not found!", result.Error);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [TestCase(RequestStatus.Approved)]
        [TestCase(RequestStatus.Rejected)]
        public async Task ApproveAsync_RequestAlreadyProcessed_ReturnsFailure(RequestStatus status)
        {
            var requestId = Guid.NewGuid();
            var request = new BloodRequest { Id = requestId, Status = status };
            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(requestId))
                .ReturnsAsync(request);

            var result = await _service.ApproveAsync(requestId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Request already processed", result.Error);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ApproveAsync_InsufficientStock_ReturnsFailure()
        {
            var requestId = Guid.NewGuid();
            var request = new BloodRequest
            {
                Id = requestId,
                Status = RequestStatus.Pending,
                BloodTypeId = 1,
                UnitsRequested = 5
            };
            var stock = new BloodStock
            {
                BloodTypeId = request.BloodTypeId,
                UnitsAvailable = 3
            };

            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(requestId))
                .ReturnsAsync(request);
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(request.BloodTypeId))
                .ReturnsAsync(stock);

            var result = await _service.ApproveAsync(requestId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Not enough units in stock", result.Error);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ApproveAsync_SuccessfulApproval_UpdatesRequestAndStock()
        {
            var requestId = Guid.NewGuid();
            var request = new BloodRequest
            {
                Id = requestId,
                Status = RequestStatus.Pending,
                BloodTypeId = 1,
                UnitsRequested = 5
            };
            var stock = new BloodStock
            {
                BloodTypeId = request.BloodTypeId,
                UnitsAvailable = 10
            };

            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(requestId))
                .ReturnsAsync(request);
            _bloodStockRepoMock
                .Setup(r => r.GetByBloodTypeIdAsync(request.BloodTypeId))
                .ReturnsAsync(stock);

            var result = await _service.ApproveAsync(requestId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(RequestStatus.Approved, request.Status);
            Assert.AreEqual(5, stock.UnitsAvailable);
            Assert.IsNotNull(request.ApprovedAt);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RejectAsync_RequestNotFound_ReturnsFailure()
        {
            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BloodRequest)null);

            var result = await _service.RejectAsync(Guid.NewGuid());

            Assert.IsFalse(result.IsSuccess);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [TestCase(RequestStatus.Approved)]
        [TestCase(RequestStatus.Rejected)]
        public async Task RejectAsync_RequestAlreadyProcessed_ReturnsFailure(RequestStatus status)
        {
            var requestId = Guid.NewGuid();
            var request = new BloodRequest { Id = requestId, Status = status };
            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(requestId))
                .ReturnsAsync(request);

            var result = await _service.RejectAsync(requestId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Request already processed", result.Error);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RejectAsync_SuccessfulRejection_UpdatesRequestAndStock()
        {
            var requestId = Guid.NewGuid();
            var request = new BloodRequest
            {
                Id = requestId,
                Status = RequestStatus.Pending,
                BloodTypeId = 1,
                UnitsRequested = 5
            };

            _bloodRequestRepoMock
                .Setup(r => r.GetByIdAsync(requestId))
                .ReturnsAsync(request);

            var result = await _service.RejectAsync(requestId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(RequestStatus.Rejected, request.Status);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Test]
        public async Task GetMyRequestsAsync_HospitalNotFound_ReturnsFailure()
        {
            var userId = "user123";

            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((Hospital)null);

            var result = await _service.GetMyRequestsAsync(userId);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Hospital not found!", result.Error);
        }

        [Test]
        public async Task GetMyRequestsAsync_ShouldReturnEmptyListWhenNoRequests()
        {
            var userId = "user123";
            var hospitalId = Guid.NewGuid();

            var hospital = new Hospital
            {
                Id = hospitalId,
            };

            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(hospital);
            _bloodRequestRepoMock
                .Setup(r => r.GetByHospitalIdAsync(hospitalId)).ReturnsAsync(new List<BloodRequest>());

            var result = await _service.GetMyRequestsAsync(userId);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.Count);
        }

        [Test]
        public async Task GetMyRequestsAsync_ShouldReturnRequests()
        {
            var userId = "user123";
            var hospitalId = Guid.NewGuid();
            var hospital = new Hospital { Id = hospitalId };
            var bloodType = new BloodType { Name = "A+" };
            var requests = new List<BloodRequest>()
            {
                new BloodRequest
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalId,
                    UnitsRequested = 3,
                    Status = RequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    BloodType = bloodType,
                },
                new BloodRequest
                {
                    Id = Guid.NewGuid(),
                    HospitalId = hospitalId,
                    UnitsRequested = 2,
                    Status = RequestStatus.Approved,
                    CreatedAt = DateTime.UtcNow,
                    BloodType = bloodType,
                }
            };
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(hospital);
            _bloodRequestRepoMock
                .Setup(r => r.GetByHospitalIdAsync(hospitalId)).ReturnsAsync(requests);

            var result = await _service.GetMyRequestsAsync(userId);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual("A+", result.Value[0].BloodType);
            _bloodRequestRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
