using System;
using System.IO;
using System.Net;
using System.Text;

namespace Nolte.Net
{
    public class Ftp
    {
        public static void Upload(string localFile, string serverName, string serverPath, string remoteFilename, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + serverName + "/" + serverPath.Trim(new char[] { '/' }) + "/" + remoteFilename);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(localFile))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
            }
            request.ContentLength = fileContents.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
            }
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
            response.Close();
        }
    }
}