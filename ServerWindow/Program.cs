using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWindow
{
    class Program
    {
        public static List<ClientResponse> _clientResponses = new List<ClientResponse>();

        static List<string> ClientIP = new List<string>();
        static int ActiveUsers = 0;
        static void Main(string[] args)
        {
            /*
            DateTime[] arrString = new DateTime[2];
            arrString[0] = DateTime(9,0);
            arrString[1] = "1,00";
            arrString[2] = "6:00";
            */

            Console.WriteLine("Connecting...");
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("10.0.10.254");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
            var average = 0;
            var lastSum = 0;
            var averagecounter = 2;

            int[] ventTimeMass = new int[3];

            ventTimeMass[0] = 9;

            ventTimeMass[1] = 1;

            ventTimeMass[2] = 6;


            //     string fstVent = "1:00";

            //  var cultureInfo = new CultureInfo("de-DE");

            //  var ventTime = DateTime.Parse(fstVent, cultureInfo, DateTimeStyles.NoCurrentDateDefault);

            //   var z = DateTime.Now.ToString("hh:mm", CultureInfo.InvariantCulture);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                int countUser = 0;
                // Начинаем слушать соединения


                var tg = new TGmodel();
                tg.GetMessageFromTG();


                while (true)
                {
                   // CheckClient();

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    //  var b = sListener.RemoteEndPoint.AddressFamily;
                    //var clientAdress = ((IPEndPoint)sListener.LocalEndPoint).Address.ToString();

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    IPEndPoint clientep;
                    //Socket client;
                    clientep = (IPEndPoint)handler.RemoteEndPoint;
                    //string strConnect = "Connected with " + clientep.Address;

                    var isIPinList = _clientResponses.Any(x => x.IpAddress == Convert.ToString(clientep.Address));

                    var indexIp = _clientResponses.FindIndex(x => x.IpAddress == Convert.ToString(clientep.Address));

                    //var ipAddress = clientep.Address.ToString();
                    if (!string.IsNullOrEmpty(data))
                    {
                        if (!isIPinList)
                        {
                            _clientResponses.Add(new ClientResponse(clientep.Address.ToString(), Convert.ToInt32(data), DateTime.Now, true));
                        }
                        else
                        {

                            _clientResponses[indexIp] = new ClientResponse(clientep.Address.ToString(), Convert.ToInt32(data), DateTime.Now, true);

                        }
                    }


                    //ClientIP.Add(Convert.ToString(clientep.Address));

                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "  от " + clientep + "\n");

                    Console.Write("Всего пользователей подключено: " + _clientResponses.Count + "\n");

                    try
                    {
                        var timeNow = DateTime.Now;
                        //var maxDifference = TimeSpan.FromMinutes(15);
                        var maxDifference = TimeSpan.FromSeconds(30);


                        for (var i = 0; i < _clientResponses.Count; i++)
                        {
                            if ((timeNow - _clientResponses[i].Time) > maxDifference)
                            {
                                _clientResponses.RemoveAt(i);
                            }
                        }

                        var sum = 0;
                        for (var i = 0; i < _clientResponses.Count; i++)
                        {
                            sum += _clientResponses[i].Temperature;
                         

                        }

                        average = sum / _clientResponses.Count;
                        /*
                        if (countUser != 0)
                        {
                            lastSum = lastSum + Convert.ToInt32(data);
                            countUser--;
                        }
                        else
                        {
                            countUser = _clientResponses.Count;
                            average = lastSum / _clientResponses.Count;
                            lastSum = 0;
                        }*/
                    }
                    catch
                    {
                        //Console.WriteLine("Пришло сообщение с ошибкой!");
                    }

                    int hoursNow = DateTime.Now.Hour;
                    int minuteNow = DateTime.Now.Minute;



                    string reply = "В " + Convert.ToString(ventTimeMass[1]) + ":00" + " будет проводиться проветривание.";

                    if (average < 3)
                    {
                        reply = "Окно будет скоро закрыто";
                    }
                    if (average > 7)
                    {
                        reply = "Окно будет скоро открыто";
                    }

                    if (hoursNow == ventTimeMass[0] && minuteNow < 15 || hoursNow == ventTimeMass[1] && minuteNow < 15 || hoursNow == ventTimeMass[2] && minuteNow < 15)
                        reply = "Идет проветривание, осталось: " + Convert.ToString(15 - minuteNow) + " минут";
                    else
                    {
                        if (hoursNow < 6)
                        {
                            hoursNow += 12;
                        }
                        int nearVentTime = 0;
                        for (int i = 0; i < ventTimeMass.Length; i++)
                        {
                            if (ventTimeMass[i] - hoursNow >= 0)
                            {
                                nearVentTime = ventTimeMass[i];
                                reply = "В " + Convert.ToString(nearVentTime) + ":00" + " будет проводиться проветривание.";
                                break;
                            }
                            else
                            { }
                        }
                    }
                   // reply = "Идет проветривание, осталось: " + Convert.ToString(15 - minuteNow) + " минут";//ДЛЯ ТЕСТА

                    reply = average + " " + reply;

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);
                    // Отправляем ответ клиенту\
                    Console.Write("Среднее число сейчас: " + average + "\n\n");
                    //byte[] msg = Encoding.UTF8.GetBytes(reply);
                    //  handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }






        
        static async void WorkWithTGClietn()
        {
            await Task.Run(() => CheckList());
        }
        static void CheckList()
        {
                var timeNow = DateTime.Now;
                //var maxDifference = TimeSpan.FromMinutes(15);
                var maxDifference = TimeSpan.FromSeconds(30);

                for (var i = 0; i < _clientResponses.Count; i++)
                {
                    if ((timeNow - _clientResponses[i].Time) > maxDifference)
                    {
                                 _clientResponses.RemoveAt(i);
                    }
                }
         }
    }
}
