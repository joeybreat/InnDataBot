using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using innModel;

namespace InnDataBot
{
    public class FnsApi
    {
        static string FnsApiKey = ConfigurationSettings.AppSettings["FnsApiKey"];
        public string DirPath = $"{Directory.GetCurrentDirectory}/files";

        public string GetInnInfo(string inn)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                $"https://api-fns.ru/api/egr?req={inn}&key={FnsApiKey}");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string innData = reader.ReadToEnd();
            response.Close();

            InnJson innJson = JsonConvert.DeserializeObject<InnJson>(innData);
            try
            {
                return $"Наименование компании: {innJson.Items[0].jp.NameJp}\n" +
                    $"Адрес: {innJson.Items[0].jp.AddressJp.FullAddressJp}\n";
            }
            catch (Exception ex)
            {
                return "Введены некоректные данные";
            }
        }

        public void GetVypDoc(string inn)
        {
            Directory.CreateDirectory(DirPath);

            using (var client = new WebClient())
            {
                client.DownloadFile($"https://api-fns.ru/api/vyp?req={inn}&key={FnsApiKey}", $"{DirPath}/doc.pdf");
            }


        }
        public string GetIP()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://api.ipify.org/");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string ipData = reader.ReadToEnd();
            response.Close();

            return ipData;
        }
    }
}
