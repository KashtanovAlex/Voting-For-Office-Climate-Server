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

            //запись в базу
            GetDBConnection();



            return OutputWeather.Temperature;
        }

        public static SqlConnection
        GetDBConnection()
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345
            //
            string datasource = @"PROGRAM";

            string connString = @"Data Source=" + datasource + "; Initial Catalog=WeatherStory; User ID=sa ; Password=Fibertrade2";

            SqlConnection connection = new SqlConnection(connString);
            connection.Open();
            //Console.WriteLine("Подключение открыто");
            //Console.WriteLine("Свойства подключения:");
            //Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
            //Console.WriteLine("\tБаза данных: {0}", connection.Database);
            //Console.WriteLine("\tСервер: {0}", connection.DataSource);
            //Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
            //Console.WriteLine("\tСостояние: {0}", connection.State);
            //Console.WriteLine("\tWorkstationld: {0}", connection.WorkstationId);
            var DBTemp = Convert.ToString(Convert.ToInt32(Convert.ToDouble(OutputWeather.Temperature)));
            var DBPressure = Convert.ToString(Convert.ToInt32(Convert.ToDouble(OutputWeather.Pressure)));
            var DBTempMin = Convert.ToString(Convert.ToInt32(Convert.ToDouble(OutputWeather.Temp_min)));
            DateTime dt = DateTime.Now;
            string DateNoFormat = dt.ToShortDateString();
            string DateDB = DateNoFormat[0]+""+ DateNoFormat[1]+"-" + DateNoFormat[3] + "" + DateNoFormat[4] + "-" + DateNoFormat[6] + DateNoFormat[7] + DateNoFormat[8] + DateNoFormat[9];
            //DateDB += " 00:00:10";
            //DateDB = "22-12-2008 10:37:22";
            //var cmd = new SqlCommand("SELECT City FROM Table_Weather WHERE Temp = -50", connection);20-10-2008 10:37:22'
            //var cmd = new SqlCommand("INSERT INTO SavedWeather VALUES('" + DBTemp + "','" + DBPressure + "', '" + DBTempMin + "', '20-10-2008 10:37:22','Novosibirsk')", connection);
            var cmd = new SqlCommand("INSERT INTO SavedWeather VALUES('" + DBTemp + "','" + DBPressure + "', '" + DBTempMin + "','" + DateDB + "','Nsk')", connection);
            SqlDataReader read = cmd.ExecuteReader();


            if (read.HasRows) // если есть данные
            {
                while (read.Read())
                {
                    Console.WriteLine(read.GetName(0) + " " + Convert.ToString(read.GetSqlValue(0)));
                }
            }
            return connection;
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
