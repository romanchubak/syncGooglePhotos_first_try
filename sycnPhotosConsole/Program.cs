﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Newtonsoft.Json;

// so the main idea to create a service which download a photos from 
// Google Photos and put them in Apple Photo Gallery file,
// it is the first try to download photos from google,

// but I rejected this idea
// because i had not found the library for converting photo files to apple file


//it is the console app, but it is very bad do all work in one class
//it is only first try to download files from google photos

namespace sycnPhotosConsole { //bad namespace name
   class Program {
      static void Main(string[] args) {
         //hardcode is bad, maybe i have to put this in configuration file or some secured store
         string credPath = @"StorePath";
         string[] scopes = {
            "https://www.googleapis.com/auth/photoslibrary.sharing",
            "https://www.googleapis.com/auth/photoslibrary.readonly"
         };
         string UserName = "romanchubak@gmail.com";
         string ClientID = "***clientID***";
         string ClientSecret = "***secret***";

         UserCredential credential;
         //need some manager class for this, hardcode is bad
         using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
         {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                scopes,
                UserName,
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result; //bad usage of async methods
         }

         //need some class for configuring this
         try {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://photoslibrary.googleapis.com/v1/mediaItems:search");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("client_id", ClientID);
            httpWebRequest.Headers.Add("client_secret", ClientSecret);
            httpWebRequest.Headers.Add("Authorization:" + credential.Token.TokenType + " " + credential.Token.AccessToken);
            httpWebRequest.Method = "POST";

            //need some builder to build filter, maybe there are some external libraries for this
            var filters = new Filters() {
               DateFilters = new DateFilters() {
                  Dates = new List<GoogleFilterDate>() {
                     new GoogleFilterDate {
                        Day = 21,
                        Month = 7,
                        Year = 2020
                     }
                  }
               }
            };

            string jsonFilter = JsonConvert.SerializeObject(new {filters}); // this is bad "new {filters}", the same problem, need builder
            
            using (Stream requestStream = httpWebRequest.GetRequestStream()) {
               ASCIIEncoding encoding = new ASCIIEncoding ();
               byte[] requestByteArrayBytes = encoding.GetBytes (jsonFilter);
               
               requestStream.Write(requestByteArrayBytes);
            }
            
            

            HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream()) //response can be null
            {
               StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

               СlsResponseRootObject responseObject = new СlsResponseRootObject(); //bad name for response
               responseObject = JsonConvert.DeserializeObject<СlsResponseRootObject>(reader.ReadToEnd());

               if (responseObject != null) // bad because create only one if block like if (responseObject?.mediaItems != null && responseObject.mediaItems.Count > 0)
               {
                  if (responseObject.mediaItems != null && responseObject.mediaItems.Count > 0)
                  {
                     Console.WriteLine("------------------------Retrieving media files--------------------------------");
                     foreach (var item in responseObject.mediaItems)
                     { 
                        using (WebClient client = new WebClient()) {
                           string url = "";

                           if (item.mimeType.Contains("image")) {
                              url = $"{item.baseUrl}=w{item.mediaMetadata.width}-h{item.mediaMetadata.height}";
                           } else if (item.mimeType.Contains("video")) {
                              url = $"{item.baseUrl}=dv";
                           } else {
                              throw new Exception("unknown mime type");
                           }
                           
                           string filesDirectory = $"Photos/{filters.DateFilters.Dates[0].Year}_{filters.DateFilters.Dates[0].Month}_{filters.DateFilters.Dates[0].Day}"; // put this pattern in configuration
                           
                           Directory.CreateDirectory(filesDirectory); // need try catch only for this
                           
                           client.DownloadFile(new Uri(url), $"{filesDirectory}/{item.filename}"); // put this pattern in configuration
                        }
                        Console.WriteLine($"ID:{item.id}, Filename:{item.filename}, MimeType:{item.mimeType}");
                     }
                  }
               }
            }
         }
         catch (Exception ex) {
            Console.WriteLine("Error occured: " + ex.Message);
         }
      } 
   }
}