using FluentAssertions;
using MyCrowdCharger.Mobile.Api.Interfaces;
using MyCrowdCharger.Mobile.Api.Services;
using NUnit.Framework;

namespace MyCrowdCharger.Mobile.Api.Tests
{
    [TestFixture]
    public class CrowdServiceTests
    {
        private readonly ICrowdService _crowdService;

        public CrowdServiceTests()
        {
            _crowdService = new CrowdService();
        }

        [Test]
        public void Ping_CalledForValidService_ReturnsTrue()
        {
            var result = _crowdService.Ping();
            result.Should().BeTrue();
        }
    }
}
