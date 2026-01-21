using BloodBank.Application.Interfaces.Services;

namespace BloodBank.Application.Tests.Services
{
    [TestFixture]
    public class HospitalServiceTests
    {
        private Mock<IHospitalRepository> _hospitalRepoMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<HospitalService>> _loggerMock;

        private HospitalService _service;

        [SetUp]
        public void Setup()
        {
            _hospitalRepoMock = new Mock<IHospitalRepository>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<HospitalService>>();

            _service = new HospitalService(
                _hospitalRepoMock.Object,
                _userServiceMock.Object,
                _loggerMock.Object
                );
        }

        [Test]
        public async Task CompleteProfileAsync_ProfileAlreadyExists_ReturnsFailure()
        {
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(new Hospital());

            var result = await _service.CompleteProfileAsync("user123", new CompleteHospitalProfileDTO());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Profile already exists!", result.Error);
            _hospitalRepoMock.Verify(r => r.AddAsync(It.IsAny<Hospital>()), Times.Never);
            _hospitalRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task CompleteProfileAsync_ValidRequest_ReturnsSuccess()
        {
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Hospital)null);
            _hospitalRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Hospital>())).Returns(Task.CompletedTask);
            _hospitalRepoMock
                .Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CompleteHospitalProfileDTO
            {
                Name = "Test Hospiatl",
                Address = "Luxor"
            };

            var result = await _service.CompleteProfileAsync("user123", dto);
            Assert.IsTrue(result.IsSuccess);
            _hospitalRepoMock.Verify(r => r.AddAsync(It.IsAny<Hospital>()), Times.Once);
            _hospitalRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetMyProfileAsync_HospitalNotFound_ReturnsFailure()
        {
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Hospital)null);

            var result = await _service.GetMyProfileAsync("user123");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Hospital not found!", result.Error);
        }

        [Test]
        public async Task GetMyProfileAsync_ValidHospital_ReturnsProfile()
        {
            var hospital = new Hospital
            {
                Id = Guid.NewGuid(),
                Name = "Test Hospiatl",
                Address = "Luxor"
            };
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(hospital);
            _userServiceMock
                .Setup(r => r.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserDTO { PhoneNumber = "0123456789" });

            var result = await _service.GetMyProfileAsync("user123");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test Hospiatl", result.Value.Name);
        }

        [Test]
        public async Task UpdateMyProfileAsync_HospitalNotFound_ReturnsFailure()
        {
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Hospital)null);

            var result = await _service.UpdateMyProfileAsync("user123", new UpdateHospitalDTO());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Hospital not found!", result.Error);
            _hospitalRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UpdateMyProfileAsync_ValidHospital_UpdatesSuccessfully()
        {
            var hospital = new Hospital
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Address = "Old Address"
            };
            _hospitalRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(hospital);
            _hospitalRepoMock
                .Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var user = new UserDTO { PhoneNumber = "0123456789" };
            _userServiceMock
                .Setup(r => r.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var dto = new UpdateHospitalDTO
            {
                Name = "New Name",
                Address = "New Address",
                PhoneNumber = "9821387126"
            };

            var result = await _service.UpdateMyProfileAsync("user123", dto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("New Name", hospital.Name);
            Assert.AreEqual("9821387126", user.PhoneNumber);
            _hospitalRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
