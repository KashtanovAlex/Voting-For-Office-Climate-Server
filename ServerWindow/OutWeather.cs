using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerWindow
{
    class OutWeather
    {
        public string GetData()
        {
            WebRequest req = WebRequest.Create(@"http://api.openweathermap.org/data/2.5/weather?q=Novosibirsk&APPID=7bf7d29effd3cd324b3154ff7de827d0");
            req.Method = "POST";
            //Вот тут пришлось погуглить так как не редко тип у каждого свой, в итоге нашел в одном из проектов на github.
            req.ContentType = "application/x-www-urlencoded";

            string str = ""; //!!!Новая сторока
            openweather openweather;//!!!Новая сторока

            WebResponse response = req.GetResponse();
            using (Stream s = response.GetResponseStream()) //Пишем в поток.
            {
                using (StreamReader r = new StreamReader(s)) //Читаем из потока.
                {
                    str = r.ReadToEnd(); //!!!Изменения
                }
            }
            response.Close(); //Закрываем поток
            openweather = JsonConvert.DeserializeObject<openweather>(str); //Ключевой момент, загоняем ответ сервера в нашу переменную типа openweather.
            OutputWeather.Temperature = (openweather.main.temp - 273.15).ToString();
            OutputWeather.Pressure = openweather.main.pressure.ToString(); //Давление.
            OutputWeather.Temp_min = (openweather.main.temp_min - 273.15).ToString(); //Тепмпература  min на сегодня. 
            OutputWeather.Date = DateTime.Now.ToString();
            string[] numbers = { OutputWeather.Temperature, OutputWeather.Pressure, OutputWeather.Temp_min, OutputWeather.Date };

            return OutputWeather.Temperature;
        }

        class mainWeather
        {
            public double temp { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
        }
        class openweather
        {
            [JsonProperty("base")] //так как слово base является ключевым для C# необходимо воспользоваться параметром, он как бы подменяет имя.
            public string base1 { get; set; }
            public mainWeather main { get; set; }
            public int visibility { get; set; }

            public long dt { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }
    }
    public class OutputWeather
    {
        public static string Temperature { get; set; }
        public static string Pressure;
        public static string Temp_min;
        public static string Date;
    }
}
