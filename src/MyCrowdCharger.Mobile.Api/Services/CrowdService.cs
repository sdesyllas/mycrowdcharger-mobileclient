using System;
using System.Collections.Generic;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Interfaces;
using RestSharp;

namespace MyCrowdCharger.Mobile.Api.Services
{
    public class CrowdService : ICrowdService
    {
        public bool Ping()
        {
            var client = new RestClient("http://spyros.hopto.org");
            var request = new RestRequest("ping", Method.GET);

            // execute the request
            IRestResponse response = client.Execute(request);
            return response.ResponseStatus == ResponseStatus.Completed;
        }

        public List<Device> GetAllDevices()
        {
            throw new NotImplementedException();
        }

        public Device GetDeviceByName(string name)
        {
            throw new NotImplementedException();
        }

        public Device RegisterDevice(Device newDevice)
        {
            throw new NotImplementedException();
        }

        public Device RefreshDevice(Device device)
        {
            throw new NotImplementedException();
        }

        public bool SendBattery(BatterySend batterySendInfo)
        {
            throw new NotImplementedException();
        }

        public List<Device> GetNearestDevicesToDeviceLocation(Device device)
        {
            throw new NotImplementedException();
        }
    }
}