using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Interfaces;
using Newtonsoft.Json;

namespace MyCrowdCharger.Mobile.Api.Services
{
    public class CrowdService : ICrowdService
    {
        private const string ServiceUrl = "http://spyros.hopto.org:8000";

        private readonly ILog _log;

        public CrowdService(ILog log)
        {
            _log = log;
        }

        public bool Ping()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetAsync($"{ServiceUrl}/ping").Result;
            _log.Debug(response.Content.ReadAsStringAsync().Result);
            return response.IsSuccessStatusCode;
        }

        public List<Device> GetAllDevices()
        {
            _log.Debug($"GET: {ServiceUrl}/device");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            
            var response = client.GetStringAsync($"{ServiceUrl}/device").Result;
            var responseDevices = JsonConvert.DeserializeObject<DevicesResult>(response);
            _log.Debug($"GET: {ServiceUrl}/device | success: {responseDevices}");
            return responseDevices.Result;
        }

        public Device GetDeviceByName(string name)
        {
            _log.Debug($"GET: {ServiceUrl}/device/{name}");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            try
            {
                var response = client.GetStringAsync($"{ServiceUrl}/device/{name}").Result;
                var responseDevice = JsonConvert.DeserializeObject<DeviceResult>(response);
                _log.Debug($"GET: {ServiceUrl}/device | success: {responseDevice}");
                return responseDevice.Result;
            }
            catch (Exception exception)
            {
                _log.Error("", exception);
                return null;
            }
        }

        public bool DeleteDeviceByName(string name)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            
            var response = client.DeleteAsync($"{ServiceUrl}/device/{name}").Result;
            _log.Debug(response.Content.ReadAsStringAsync().Result);
            return response.IsSuccessStatusCode;
        }

        public Device RegisterDevice(Device newDevice)
        {
            _log.Debug($"POST: {ServiceUrl}/register | payload: {newDevice}");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            
            var buffer = System.Text.Encoding.UTF8.GetBytes(newDevice.ToString());
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync($"{ServiceUrl}/register", byteContent).Result;
            if (response.Content.ReadAsStringAsync().Result.Contains("already registered"))
            {
                return null;
            }
            _log.Debug(response.Content.ReadAsStringAsync().Result);
            if (!response.IsSuccessStatusCode) return null;
            var responseDevice = JsonConvert.DeserializeObject<DeviceResult>(response.Content.ReadAsStringAsync().Result);
            _log.Debug($"POST: {ServiceUrl}/register | success: {responseDevice}");
            return responseDevice.Result;
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