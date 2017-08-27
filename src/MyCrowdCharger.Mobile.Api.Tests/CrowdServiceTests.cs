using System;
using MyCrowdCharger.Mobile.Api.Interfaces;
using FluentAssertions;
using Moq;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Services;
using Xunit;
using Xunit.Abstractions;

namespace MyCrowdCharger.Mobile.Api.Tests
{
    public class CrowdServiceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private readonly ICrowdService _crowdService;

        private readonly Mock<ILog> _mockLog;
        
        public CrowdServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _mockLog = new Mock<ILog>();
            _crowdService = new CrowdService(_mockLog.Object);

            var device1 = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] {51.649829, -0.188136},
                Name = "Spyros",
                Nickname = "Spyros"
            };

            var device2 = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] { 51.651047, -0.186956 },
                Name = "Fenia",
                Nickname = "Fenia"
            };
            _crowdService.RegisterDevice(device1);
            _crowdService.RegisterDevice(device2);
        }

        public void Dispose()
        {
            _crowdService.DeleteDeviceByName("Spyros");
            _crowdService.DeleteDeviceByName("Fenia");
        }

        [Fact]
        public void Ping_CalledForValidService_ReturnsTrue()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var result = crowdService.Ping();

            //Assert
            mockLog.Verify(x=>x.Debug(It.IsAny<string>()), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public void GetAllDevices_WhenDevicesExist_ReturnsSomeDevices()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var devices = crowdService.GetAllDevices();

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));
            devices.Should().NotBeNull();
            devices.Count.Should().BeGreaterThan(1);
        }
    }
}
