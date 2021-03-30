using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ClientFileStorage
{
    public class Загрузитьфайл
    {
        public string Link;
        public string IdUser;
        HubConnection _connection;

        public Загрузитьфайл(string Link1, string IdUser1)
        {
            Link = Link1;
            IdUser = IdUser1;
        }

        public async void SetCon()
        {
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(Link)
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .Build();
                _connection.ServerTimeout = TimeSpan.FromMinutes(10000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public async void загрузитьФайлToolStripMenuItem_Click1(string FileNamePath)
        {
            SetCon();
            var fileName = new DirectoryInfo(FileNamePath).Name;
            //fileName = fileName.Remove(fileName.Length - 7, 3);
            long READBUFFER_SIZE = 1048576;
            int offset = 0;
            long Pos = 0;
            try
            {
                using (FileStream FS = new FileStream(FileNamePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long n = FS.Length;
                    while (FS.Position < FS.Length)
                    {
                        if (n <= READBUFFER_SIZE)
                        {
                            byte[] FSBuffer = new byte[n];
                            FS.Read(FSBuffer, offset, (int)n);
                            await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, n, IdUser);
                        }
                        else
                        {
                            byte[] FSBuffer = new byte[READBUFFER_SIZE];
                            FS.Read(FSBuffer, offset, (int)READBUFFER_SIZE);
                            await _connection.InvokeAsync("UploadFile", FSBuffer, fileName, Pos, FS.Length, READBUFFER_SIZE, IdUser);
                            n -= READBUFFER_SIZE;
                        }
                        Pos = FS.Position;
                        GC.Collect();
                    }
                    await _connection.InvokeAsync("WriteToDataBase", fileName, IdUser);

                }
                await _connection.StopAsync();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}

