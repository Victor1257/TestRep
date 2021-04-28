using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace ClientFileStorage
{
    public class Загрузитьфайл1
    {
        public string Link;
        public string IdUser;
        HubConnection _connection;
        public HttpClient client;

        public Загрузитьфайл1( HttpClient httpClient)
        {
            client = httpClient;
        }

        public async void загрузитьФайлToolStripMenuItem_Click1(string FileNamePath,bool IsFile,int id,string MustBeEx)
        {
            var fileName = new DirectoryInfo(FileNamePath).Name;
            long Size = new System.IO.FileInfo(FileNamePath).Length;
            try
            {
                using (client)
                {
                    using (HttpResponseMessage response = await client.GetAsync(client.BaseAddress.ToString(), HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            string fileToWriteTo = Path.GetTempFileName();
                            using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}

