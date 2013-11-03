// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapDownloader.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MapDownloader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.IO;
    using System.Net;
    using Adan.Client.Common;
    using Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Ionic.Zip;
    using Properties;
    using System.Threading;

    /// <summary>
    /// Class to download maps from server.
    /// </summary>
    public static class MapDownloader
    {
        /// <summary>
        /// Downloads the maps.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        public static void DownloadMaps([NotNull]InitializationStatusModel initializationStatusModel)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            try
            {
                initializationStatusModel.PluginInitializationStatus = "Checking for map updates";

                var request = (HttpWebRequest)WebRequest.Create(Settings.Default.MapsUrl);

                //Обновляет, только если файл существует.
                if (File.Exists(Path.Combine(GetMapsFolder(), "Maps.zip")))
                    request.IfModifiedSince = Settings.Default.LastMapsUpdateDate;

                CreateDirectories();

                using (var response = GetHttpResponse(request))
                {
                    if (response.StatusCode == HttpStatusCode.NotModified)
                        return;

                    initializationStatusModel.PluginInitializationStatus = "Downloading maps";
                    
                    using (var responceStream = response.GetResponseStream())
                    {
                        if (responceStream == null)
                            return;

                        using (var stream = File.Open(Path.Combine(GetMapsFolder(), "Maps.zip"), FileMode.Create, FileAccess.Write))
                        {
                            responceStream.CopyTo(stream);
                            Settings.Default.LastMapsUpdateDate = response.LastModified;
                        }
                    }

                    initializationStatusModel.PluginInitializationStatus = "Unpacking maps";
                    using (var zip = ZipFile.Read(Path.Combine(GetMapsFolder(), "Maps.zip")))
                    {
                        foreach (var zipEntry in zip.Entries)
                        {
                            if (zipEntry.IsDirectory)
                                continue;

                            var fileName = Path.GetFileName(zipEntry.FileName) ?? string.Empty;
                            initializationStatusModel.PluginInitializationStatus = string.Format("Unpacking maps: {0}", fileName);
                            zipEntry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                            zipEntry.Extract(GetMapsFolder());
                        }
                    }
                }
            }
            catch (Exception)
            {
                initializationStatusModel.PluginInitializationStatus = "Error connecting to server";
            }

            initializationStatusModel.PluginInitializationStatus = string.Empty;
            Settings.Default.Save();
        }

        [NotNull]
        private static string GetMapsFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Maps");
        }

        [NotNull]
        private static string GetZonesFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Maps", "MapGenerator", "MapResults");
        }

        [NotNull]
        private static string GetZoneVisitsFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Maps", "ZoneVisits");
        }

        private static void CreateDirectories()
        {
            if (!Directory.Exists(GetMapsFolder()))
            {
                Directory.CreateDirectory(GetMapsFolder());
            }

            if (!Directory.Exists(Path.Combine(ProfileHolder.Instance.Folder, "Maps", "MapGenerator")))
            {
                Directory.CreateDirectory(Path.Combine(ProfileHolder.Instance.Folder, "Maps", "MapGenerator"));
            }

            if (!Directory.Exists(GetZonesFolder()))
            {
                Directory.CreateDirectory(GetZonesFolder());
            }

            if (!Directory.Exists(GetZoneVisitsFolder()))
            {
                Directory.CreateDirectory(GetZoneVisitsFolder());
            }
        }

        [NotNull]
        private static HttpWebResponse GetHttpResponse([NotNull]WebRequest request)
        {
            Assert.ArgumentNotNull(request, "request");

            try
            {
                var responce = (HttpWebResponse)request.GetResponse();
                if (responce == null)
                {
                    throw new InvalidOperationException("Server did not reply.");
                }

                return responce;
            }
            catch (WebException ex)
            {
                // only handle protocol errors that have valid responses
                if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw;
                }

                return (HttpWebResponse)ex.Response;
            }
        }
    }
}
