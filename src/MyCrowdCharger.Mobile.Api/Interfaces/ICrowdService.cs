using System.Collections.Generic;
using MyCrowdCharger.Mobile.Api.Dtos;

namespace MyCrowdCharger.Mobile.Api.Interfaces
{
    public interface ICrowdService
    {
        bool Ping();

        List<Device> GetAllDevices();

        Device GetDeviceByName(string name);

        bool DeleteDeviceByName(string name);

        Device RegisterDevice(Device newDevice);

        Device RefreshDevice(Device device);

        bool SendBattery(BatterySend batterySendInfo);

        List<Device> GetNearestDevicesToDeviceLocation(string deviceName);
    }
}
