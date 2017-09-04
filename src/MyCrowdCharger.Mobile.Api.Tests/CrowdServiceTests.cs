using System;
using System.Linq;
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
                Location = new[] { -0.187961, 51.649825 },
                Name = "Spyros",
                Nickname = "Spyros"
            };

            var device2 = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] { -0.186663, 51.650477 },
                Name = "Fenia",
                Nickname = "Fenia"
            };

            // create a distant device 
            // devices next to each other in an address in London, distant in university of Piraeus
            var oneMileDistantDevice = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] { -0.201888, 51.655295 },
                Name = "UnipiDevice",
                Nickname = "UnipiDevice"
            };
            _crowdService.RegisterDevice(oneMileDistantDevice);
            _crowdService.RegisterDevice(device1);
            _crowdService.RegisterDevice(device2);
        }

        public void Dispose()
        {
            _crowdService.DeleteDeviceByName("UnipiDevice");
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

        [Fact]
        public void GetDevice_WhenDeviceExist_ReturnDevice()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var device = crowdService.GetDeviceByName("Fenia");

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));
            device.Should().NotBeNull();
            device.Name.Should().Be("Fenia");
            device.Nickname.Should().Be("Fenia");
            device.BatteryLevel.Should().BePositive();
            device.Contributions.Should().BePositive();
        }

        [Fact]
        public void GetDevice_WhenDeviceDoesNotExist_ReturnNull()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var device = crowdService.GetDeviceByName("Morgana");

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(1));
            mockLog.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Exactly(1));
            device.Should().BeNull();
        }

        [Fact]
        public void RegisterDevice_WhenDeviceExists_ReturnNull()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            var device = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] {51.649829, -0.188136},
                Name = "Spyros",
                Nickname = "Spyros"
            };

            //Act
            var responseDevice = crowdService.RegisterDevice(device);

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(1));
            responseDevice.Should().BeNull();
        }

        [Fact]
        public void GetNearestDevicesToDeviceLocation_WhenADeviceIsNear_ReturnThisDevice()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var nearestDevices = crowdService.GetNearestDevicesToDeviceLocation("Spyros");

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));
            nearestDevices.Count.Should().Be(1);
            var nearestDevice = nearestDevices.FirstOrDefault(x=>x.Name == "Fenia");
            var unipiDevice = nearestDevices.FirstOrDefault(x => x.Name == "UnipiDevice");
            unipiDevice.Should().BeNull();
            nearestDevice.Name.Should().Be("Fenia");
            nearestDevice.Location[0].Should().Be(-0.186663);
            nearestDevice.Location[1].Should().Be(51.650477);
        }

        [Fact]
        public void GetNearestDevicesToDeviceLocation_WhenNoDevicesAreNear_ReturnNoDevices()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            //Act
            var nearestDevices = crowdService.GetNearestDevicesToDeviceLocation("UnipiDevice");

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));
            nearestDevices.Count.Should().Be(0);
        }

        [Fact]
        public void RefreshDevice_WhenBatteryOrLocationChanges_RefreshDeviceInDataContext()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);
            var device = crowdService.GetDeviceByName("Spyros");

            var newBatteryLevel = 65;
            var newLong = 51.651892;
            var newLat = -0.186819;

            device.BatteryLevel = newBatteryLevel;
            device.Location[0] = newLong;
            device.Location[1] = newLat;
            //Act
            var refreshedDevice = crowdService.RefreshDevice(device);

            //Assert
            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(5));
            refreshedDevice.Should().NotBeNull();
            refreshedDevice.Name.Should().Be("Spyros");
            refreshedDevice.BatteryLevel = 65;
            refreshedDevice.Location[0].Should().Be(newLong);
            refreshedDevice.Location[1].Should().Be(newLat);
        }

        [Fact]
        public void RefreshDevice_WhenDeviceDoesNotExist_ReturnNull()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            var device = new Device
            {
                BatteryLevel = 80,
                Contributions = 1,
                Location = new[] { 51.651047, -0.186956 },
                Name = "Morgana",
                Nickname = "Morgana"
            };

            //Act
            var refreshedDevice = crowdService.RefreshDevice(device);

            //Assert
            mockLog.Verify(x => x.Warning(It.IsAny<string>()), Times.Exactly(1));
            refreshedDevice.Should().BeNull();
        }

        [Fact]
        public void SendBattery_RecipientSufficientLevels_SuccesfullySendBattery()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            var batterySendInfo = new BatterySend
            {
                SenderUser = new BatterySend.Sender() {Name = "Spyros", Battery = 15},
                RecipientUser = new BatterySend.Recipient() {Name = "Fenia" }
            };

            //Act
            var hasSendBattery = crowdService.SendBattery(batterySendInfo);

            //Assert
            var refreshedSender = crowdService.GetDeviceByName("Spyros");
            var refreshedRecipient = crowdService.GetDeviceByName("Fenia");

            mockLog.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(7));
            hasSendBattery.Should().BeTrue();
            refreshedSender.BatteryLevel.Should().Be(65);
            refreshedRecipient.BatteryLevel.Should().Be(95);
        }

        [Fact]
        public void SendBattery_SenderInsufficientLevels_DoNotSendBattery()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            mockLog.Setup(x => x.Warning(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            var batterySendInfo = new BatterySend
            {
                SenderUser = new BatterySend.Sender() { Name = "Spyros", Battery = 81 },
                RecipientUser = new BatterySend.Recipient() { Name = "Fenia" }
            };

            //Act
            var hasSendBattery = crowdService.SendBattery(batterySendInfo);

            //Assert
            var refreshedSender = crowdService.GetDeviceByName("Spyros");
            var refreshedRecipient = crowdService.GetDeviceByName("Fenia");

            mockLog.Verify(x => x.Warning(It.IsAny<string>()), Times.Exactly(1));
            hasSendBattery.Should().BeFalse();
            refreshedSender.BatteryLevel.Should().Be(80);
            refreshedRecipient.BatteryLevel.Should().Be(80);
        }

        [Fact]
        public void SendBattery_RecipientDoesNotExist_DoNotSendBattery()
        {
            //Arrange
            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            mockLog.Setup(x => x.Warning(It.IsAny<string>())).Callback<string>(s => _output.WriteLine(s));
            var crowdService = new CrowdService(mockLog.Object);

            var batterySendInfo = new BatterySend
            {
                SenderUser = new BatterySend.Sender() { Name = "Spyros", Battery = 81 },
                RecipientUser = new BatterySend.Recipient() { Name = "Morgana" }
            };

            //Act
            var hasSendBattery = crowdService.SendBattery(batterySendInfo);

            //Assert
            var refreshedSender = crowdService.GetDeviceByName("Spyros");
            var refreshedRecipient = crowdService.GetDeviceByName("Fenia");

            mockLog.Verify(x => x.Warning(It.IsAny<string>()), Times.Exactly(1));
            hasSendBattery.Should().BeFalse();
            refreshedSender.BatteryLevel.Should().Be(80);
            refreshedRecipient.BatteryLevel.Should().Be(80);
        }
    }
}
