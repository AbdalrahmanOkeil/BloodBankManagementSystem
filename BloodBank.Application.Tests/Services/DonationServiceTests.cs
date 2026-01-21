namespace BloodBank.Application.Tests.Services
{
    [TestFixture]
    public class DonationServiceTests
    {
        private Mock<IDonationRepository> _donationRepoMock;
        private Mock<IDonorRepository> _donorRepoMock;
        private Mock<IBloodStockRepository> _bloodStockRepoMock;
        private Mock<ILogger<DonationService>> _loggerMock;
        private DonationService _service;

        [SetUp]
        public void Setup()
        {
            _donationRepoMock = new Mock<IDonationRepository>();
            _donorRepoMock = new Mock<IDonorRepository>();
            _bloodStockRepoMock = new Mock<IBloodStockRepository>();
            _loggerMock = new Mock<ILogger<DonationService>>();

            _service = new DonationService(
                _donationRepoMock.Object,
                _donorRepoMock.Object,
                _bloodStockRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task CreateDonationAsync_DonorNotFound_ReturnsFailure()
        {
            _donorRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Donor)null);

            var result = await _service.CreateDonationAsync("user123", new CreateDonationDTO());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donor not found!", result.Error);
            _donationRepoMock.Verify(r => r.AddAsync(It.IsAny<Donation>()), Times.Never);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task CreateDonationAsync_ValidRequest_ReturnsSuccess()
        {
            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                LastDonationDate = DateTime.UtcNow.AddDays(-100)
            };
            _donorRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(donor);
            _donationRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Donation>())).Returns(Task.CompletedTask);
            _donationRepoMock
                .Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CreateDonationDTO
            {
                BloodTypeId = 1,
                Units = 3
            };

            var result = await _service.CreateDonationAsync("user123", dto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, result.Value.Units);
            Assert.AreEqual(RequestStatus.Pending.ToString(), result.Value.Status);
            _donationRepoMock.Verify(r => r.AddAsync(It.IsAny<Donation>()), Times.Once);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateDonationAsync_DonorNotEligible_ReturnsFailure()
        {
            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                LastDonationDate = DateTime.UtcNow.AddDays(10)
            };
            _donorRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(donor);

            var result = await _service.CreateDonationAsync("user123", new CreateDonationDTO());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Dononr is not eligible yet!", result.Error);
            _donationRepoMock.Verify(r => r.AddAsync(It.IsAny<Donation>()), Times.Never);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ApproveDonationAsync_DonationNotFound_ReturnsFailure()
        {
            _donationRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Donation)null);

            var result = await _service.ApproveDonationAsync(It.IsAny<Guid>());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donation not found!", result.Error);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ApproveDonationAsync_ValidDonation_ApprovesSuccessfully()
        {
            var donor = new Donor
            {
                Id = Guid.NewGuid(),
                LastDonationDate = DateTime.UtcNow.AddDays(-100),
                TotalDonations = 0,
            };
            var donation = new Donation
            {
                Id = Guid.NewGuid(),
                Donor = donor,
                UnitsDonated = 2,
                BloodTypeId = 1,
                Status = DonationStatus.Pending,
            };
            _donationRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(donation);
            _bloodStockRepoMock
                .Setup(r => r.IncreaseStockAsync(donation.BloodTypeId, donation.UnitsDonated));
            _bloodStockRepoMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.ApproveDonationAsync(donation.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(DonationStatus.Approved, donation.Status);
            Assert.AreEqual(1, donor.TotalDonations);
            _bloodStockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RejectDonationAsync_DonationNotFound_ReturnsFailure()
        {
            _donationRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Donation)null);

            var result = await _service.RejectDonationAsync(It.IsAny<Guid>());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donation not found!", result.Error);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RejectDonationAsync_ValidDonation_RejectedSuccessfully()
        {
            var donation = new Donation
            {
                Id = Guid.NewGuid(),
                Status = DonationStatus.Pending,
            };
            _donationRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(donation);
            _donationRepoMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.RejectDonationAsync(donation.Id);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(DonationStatus.Rejected, donation.Status);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetDonationsAsync_DonorNotFound_ReturnsFailure()
        {
            _donorRepoMock
                .Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Donor)null);

            var result = await _service.GetDonationsAsync(It.IsAny<string>());

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Donor not found!", result.Error);
            _donationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    }
}
