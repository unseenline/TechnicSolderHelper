﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using TechnicSolderHelper.Confighandler;
using TechnicSolderHelper.cryptography;
using TechnicSolderHelper.Properties;

namespace TechnicSolderHelper.FileUpload
{
    public class Ftp
    {
        private String _userName;
        private String _password;
        private String _url;

        public Ftp()
        {
            Crypto crypto = new Crypto();
            ConfigHandler ch = new ConfigHandler();
            _url = ch.GetConfig("ftpUrl");
            try
            {
                _password = crypto.DecryptString(ch.GetConfig("ftpPassword"));
            }
            catch (Exception)
            {
                _password = "";
            }
            _userName = ch.GetConfig("ftpUserName");
            if (_url.EndsWith("/"))
            {
                _url.Remove(_url.Length - 1);
            }
            if (!_url.StartsWith("ftp://"))
            {
                _url = "ftp://" + _url;
            }
        }

        public void UploadFolder(String folderPath)
        {
            String[] files;
            try
            {
                files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("No files was created, so no reason to upload anything");
                return;
            }

            foreach (String file in files)
            {
                Debug.WriteLine("");
                Debug.WriteLine(file);
                Debug.WriteLine(file.Replace(folderPath + Globalfunctions.PathSeperator, ""));
                UploadFile(file, file.Replace(folderPath + Globalfunctions.PathSeperator, ""), "mods");
            }

        }

        private List<String> GetDirectoryContent(String location)
        {
            List<String> folderContent = new List<string>();
            FtpWebRequest request;
            if (location == null)
            {
                request = (FtpWebRequest)WebRequest.Create(_url);
            }
            else
            {
                request = (FtpWebRequest)WebRequest.Create(location);
            }
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.Credentials = new NetworkCredential(_userName, _password);

            using (FtpWebResponse responce = (FtpWebResponse)request.GetResponse())
            {
                Stream responceStream = responce.GetResponseStream();
                using (StreamReader reader = new StreamReader(responceStream))
                {
                    while (true)
                    {
                        try
                        {
                            String s = reader.ReadLine();
                            if (s.Contains("/"))
                            {
                                s = s.Substring(s.LastIndexOf("/") + 1);
                            }
                            folderContent.Add(s);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                }
            }

            return folderContent;
        }

        public void UploadFile(String fullyQualifiedPathName, String destinationOnServer, String constant)
        {
            GetDirectoryContent(null);
            if (constant != null)
            {
                destinationOnServer = constant + Globalfunctions.PathSeperator + destinationOnServer;
            }
            String[] tmp = destinationOnServer.Split(Globalfunctions.PathSeperator);
            List<String> folders = new List<String>(tmp);
            String fileToUpload = folders[folders.Count - 1];
            folders.Remove(folders[folders.Count - 1]);
            for (int i = 0; i < folders.Count; i++)
            {
                folders[i] = folders[i].Replace(Globalfunctions.PathSeperator.ToString(), "");
            }
            FtpWebRequest request = WebRequest.Create(_url) as FtpWebRequest;
            request.Credentials = new NetworkCredential(_userName, _password);
            Debug.WriteLine(request.RequestUri);
            foreach (String folder in folders)
            {
                Debug.WriteLine(folder);
                List<String> directoryContent = GetDirectoryContent(request.RequestUri.ToString());
                if (directoryContent.Contains(folder))
                {
                    if (request.RequestUri.ToString().EndsWith("/"))
                    {
                        request = WebRequest.Create(request.RequestUri + folder) as FtpWebRequest;
                    }
                    else
                    {
                        request = WebRequest.Create(request.RequestUri + "/" + folder) as FtpWebRequest;
                    }
                    Debug.WriteLine(request.RequestUri);
                }
                else
                {
                    if (request.RequestUri.ToString().EndsWith("/"))
                    {
                        request = WebRequest.Create(request.RequestUri + folder) as FtpWebRequest;
                    }
                    else
                    {
                        request = WebRequest.Create(request.RequestUri + "/" + folder) as FtpWebRequest;
                    }
                    Debug.WriteLine(request.RequestUri);
                    request.Credentials = new NetworkCredential(_userName, _password);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    try
                    {
                        FtpWebResponse responce = (FtpWebResponse)request.GetResponse();
                        Debug.WriteLine(responce.StatusDescription);
                    }
                    catch (WebException e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }

            try
            {
                Debug.WriteLine("Uploading file: " + fileToUpload);
                request = WebRequest.Create(request.RequestUri + "/" + fileToUpload) as FtpWebRequest;
                request.Credentials = new NetworkCredential(_userName, _password);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                using (FileStream fs = File.OpenRead(fullyQualifiedPathName))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Close();
                    requestStream.Flush();
                }
            }
            catch (WebException e)
            {
                //Console.WriteLine("error getting responce");
                Debug.WriteLine(e.Message);
            }
        }

    }
}

