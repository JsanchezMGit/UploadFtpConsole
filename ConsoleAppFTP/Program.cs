using System;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;


namespace ConsoleAppFTP
{
  class Program
  {
    static void Main(string[] args)
    {
      var ftpSettings = new
      {
        Host = ConfigurationManager.AppSettings["Zeus.Ftp.Host"],
        Port = ConfigurationManager.AppSettings["Zeus.Ftp.Port"],
        EnableSsl = bool.Parse(ConfigurationManager.AppSettings["Zeus.Ftp.EnableSsl"]),
        Password = Convert.FromBase64String(ConfigurationManager.AppSettings["Zeus.Ftp.Password"]),
        User = ConfigurationManager.AppSettings["Zeus.Ftp.User"],
        UseUntrustedCertificate = bool.Parse(ConfigurationManager.AppSettings["Zeus.Ftp.EnableSsl"])
      };

      // Get the object used to communicate with the server.
      FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString: $"ftp://{ftpSettings.Host}:{ftpSettings.Port}/{"MyFTPTestFile"}.atl");
      request.Method = WebRequestMethods.Ftp.UploadFile;

      request.EnableSsl = ftpSettings.EnableSsl;
      if (request.EnableSsl && ftpSettings.UseUntrustedCertificate) ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

      // This example assumes the FTP site uses anonymous logon.
      request.Credentials = new NetworkCredential(userName: ftpSettings.User, password: Encoding.UTF8.GetString(ftpSettings.Password));

      // Copy the contents of the file to the request stream.
      byte[] fileContents = Encoding.UTF8.GetBytes("Linea from Bytes");
      request.ContentLength = fileContents.Length;

      Stream requestStream = request.GetRequestStream();
      requestStream.Write(fileContents, 0, fileContents.Length);
      requestStream.Close();

      FtpWebResponse response = (FtpWebResponse)request.GetResponse();

      Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

      response.Close();
    }
  }
}
