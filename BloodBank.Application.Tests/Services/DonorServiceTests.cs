using BloodBank.Application.Interfaces.Services;

namespace BloodBank.Application.Tests.Services
{
    [TestFixture]
    public class DonorServiceTests
    {
        private Mock<IDonorRepository> _donorRepoMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<DonorService>> _loggerMock;

        private DonorService _service;

        [SetUp]
        public void Setup()
        {
            _donorRepoMock = new Mock<IDonorRepository>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<DonorService>>();

            _service = new DonorService(
                _donorRepoMock.Object,
                _userServiceMock.Object,
                _loggerMock.Object
                );
        }

        [Test]
        public async Task CreateProfileAsync_ProfileAlreadyExists_ReturnsFailure()
        {
            _donorRepoMock
                .Setup(r => r.ExistsByUserIdAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _service.CreateProfileAsync("user123", new CreateDonorDTO());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donor profile already exists!", result.Error);
        }

        [Test]
        public async Task CreateProfileAsync_ValidRequest_ReturnsSuccess()
        {
            _donorRepoMock
                .Setup(r => r.ExistsByUserIdAsync(It.IsAny<string>())).ReturnsAsync(false);
            _donorRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Donor>())).Returns(Task.CompletedTask);
            _donorRepoMock
                .Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CreateDonorDTO
            {
                BloodTypeId = 1,
                DateOfBirth = DateTime.UtcNow.AddDays(-25),
                HealthStatus = "Alhamdoullah"
            };

            var result = await _service.CreateProfileAsync("user123", dto);

            Assert.IsTrue(result.IsSuccess);
            _donorRepoMock.Verify(r => r.AddAsync(It.IsAny<Donor>()), Times.Once);
            _donorRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetProfileAsync_DonorNotFound_ReturnsFailure()
        {
            _donorRepoMock
                .Setup(r => r.GetWithDetailsByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Donor)null);

            var result = await _service.GetProfileAsync("user123");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donor not found!", result.Error);
            _donorRepoMock.Verify(r => r.AddAsync(It.IsAny<Donor>()), Times.Never);
            _donorRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetProfileAsync_ValidDonor_ReturnsProfile()
        {
            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                TotalDonations = 3,
                LastDonationDate = DateTime.UtcNow.AddDays(-15),
                BloodType = new BloodType { Name = "A+" },
                HealthStatus = "الحمدلله"
            };
            _donorRepoMock
                .Setup(r => r.GetWithDetailsByUserIdAsync(It.IsAny<string>())).ReturnsAsync(donor);
            _userServiceMock
                .Setup(r => r.GetUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserDTO
                {
                    FullName = "Test",
                    Email = "Test@test.com",
                });

            var result = await _service.GetProfileAsync("user123");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("A+", result.Value.BloodType);
        }

        [Test]
        public async Task UpdateDonationInfoAsync_DonorNotFound_ReturnsFailure()
        {
            _donorRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Donor)null);

            var result = await _service.UpdateDonationInfoAsync(Guid.NewGuid());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donor not found!", result.Error);
            _donorRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UpdateDonationInfoAsync_ValidDonor_UpdatesSuccessfully()
        {
            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                TotalDonations = 1,
            };
            _donorRepoMock
                .Setup(r => r.GetByIdAsync(donor.Id)).ReturnsAsync(donor);
            _donorRepoMock
                .Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateDonationInfoAsync(donor.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, donor.TotalDonations);
            Assert.IsNotNull(donor.LastDonationDate);
            _donorRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
