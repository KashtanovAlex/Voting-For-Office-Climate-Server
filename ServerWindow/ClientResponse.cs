using System;
using System.Net;

namespace ServerWindow
{
    internal class ClientResponse
    {
        public string IpAddress { get; set; }
        public int Temperature { get; set; }
        public DateTime Time { get; set; }
        public bool ClientIdentificator { get; set; }
        //ClientIdentificator  false - telegram      true - PC user

        public ClientResponse(string ipAddress, int temperature, DateTime time, bool clientIdentificator)
        {
           /* if (!IPAddress.TryParse(ipAddress, out _))
            {
                throw new ArgumentException(nameof(ipAddress));
            }*/

            if (temperature < 0 || temperature > 10)
            {
                throw new ArgumentException(nameof(temperature));
            }

            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }


            IpAddress = ipAddress;
            Temperature = temperature;
            Time = time;
            ClientIdentificator = clientIdentificator;
        }
    }
}
