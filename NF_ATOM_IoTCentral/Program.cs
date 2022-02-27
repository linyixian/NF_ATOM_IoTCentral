using System;
using System.Diagnostics;
using System.Threading;

//�ǉ�
using System.Device.WiFi;
using nanoFramework.Networking;
using System.Security.Cryptography.X509Certificates;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Azure.Devices.Provisioning.Client;
using nanoFramework.M2Mqtt.Messages;
using System.Device.I2c;
using Iot.Device.Sht3x;
using nanoFramework.Hardware.Esp32;

namespace NF_ATOM_IoTCentral
{
    public class Program
    {
        const string ssid = "YOUR SSID";
        const string password = "PASSWORD";

        const string dspaddress = "global.azure-devices-provisioning.net";
        const string idscope = "IDSCOPE";
        const string registrationid = "DEVICEID";
        const string saskey = "SASKEY";

        public static void Main()
        {
            //Wifi�ڑ�
            if (!ConnectWifi())
            {
                Debug.WriteLine("Wifi Connection failed...");
                return;
            }
            else
            {
                Debug.WriteLine("Wifi Connected...");
            }

            //ROOT CA �ؖ���
            X509Certificate certificate = new (Resource.GetBytes(Resource.BinaryResources.BaltimoreCyberTrustRoot));

            //DPS�ݒ�
            var provisioning = ProvisioningDeviceClient.Create(dspaddress, idscope, registrationid, saskey, certificate);

            //DPS������擾
            var myDevice = provisioning.Register(null, new CancellationTokenSource(30000).Token);

            if (myDevice.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                Debug.WriteLine($"Registration is not assigned: {myDevice.Status}, error message: {myDevice.ErrorMessage}");
                return;
            }

            Debug.WriteLine($"Device successfully assigned:");

            //IoTCentoral�ɐڑ�
            var device = new DeviceClient(myDevice.AssignedHub, registrationid, saskey,MqttQoSLevel.AtMostOnce,certificate);
            var res = device.Open();

            if (!res)
            {
                Debug.WriteLine("can't open the device");
                return;
            }

            //I2C Setting
            Configuration.SetPinFunction(26, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(32, DeviceFunction.I2C1_CLOCK);

            //Sensor Address
            I2cConnectionSettings settings = new I2cConnectionSettings(1, 0x44);

            //�Z���T�[
            using I2cDevice i2cdevice = I2cDevice.Create(settings);
            using Sht3x sensor = new(i2cdevice);

            while (true)
            {
                //�f�[�^���M
                device.SendMessage($"{{\"temperature\":{sensor.Temperature.DegreesCelsius.ToString("F2")},\"humidity\":{sensor.Humidity.Percent.ToString("F2")}}}",new CancellationTokenSource(2000).Token);

                Debug.WriteLine($"{{\"temperature\":{sensor.Temperature.DegreesCelsius.ToString("F2")},\"humidity\":{sensor.Humidity.Percent.ToString("F2")}}}");

                Thread.Sleep(60000);
            }
        }

        private static bool ConnectWifi()
        {
            Debug.WriteLine("Connecting WiFi");

            var success = WiFiNetworkHelper.ConnectDhcp(ssid, password, reconnectionKind: WiFiReconnectionKind.Automatic, requiresDateTime: true, token: new CancellationTokenSource(60000).Token);

            if (!success)
            {
                Debug.WriteLine($"Can't connect to the network, error: {WiFiNetworkHelper.Status}");
                if (WiFiNetworkHelper.HelperException != null)
                {
                    Debug.WriteLine($"ex: {WiFiNetworkHelper.HelperException}");
                }
            }

            Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");

            return success;
        }
    }
}
