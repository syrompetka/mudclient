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
    using Adan.Client.Common.ViewModel;
    using Adan.Client.Common.Settings;
    using Adan.Client.Common.Utils;

    /// <summary>
    /// Class to download maps from server.
    /// </summary>
    public static class MapDownloader
    {
        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler UpgradeComplete;

        /// <summary>
        /// 
        /// </summary>
        public static bool IsUpgrading
        {
            get;
            private set;
        }

        /// <summary>
        /// Downloads the maps.
        /// </summary>
        public static void DownloadMaps()
        {
            try
            {
                var mapFolder = GetMapsFolder();

                var request = (HttpWebRequest)WebRequest.Create(Settings.Default.MapsUrl);

                //Обновляет, только если файл существует.
                if (File.Exists(Path.Combine(mapFolder, "Maps.zip")))
                    request.IfModifiedSince = Settings.Default.LastMapsUpdateDate;

                CreateDirectories();

                using (var response = GetHttpResponse(request))
                {
                    if (response.StatusCode == HttpStatusCode.NotModified)
                       return;

                    using (var responceStream = response.GetResponseStream())
                    {
                        if (responceStream == null)
                            return;

                        File.Delete(Path.Combine(mapFolder, "Maps.zip"));
                        using (var stream = File.Open(Path.Combine(mapFolder, "Maps.zip"), FileMode.Create, FileAccess.Write))
                        {
                            responceStream.CopyTo(stream);
                            Settings.Default.LastMapsUpdateDate = response.LastModified;
                        }
                    }

                    try
                    {
                        IsUpgrading = true;
                        using (var zip = ZipFile.Read(Path.Combine(mapFolder, "Maps.zip")))
                        {
                            foreach (var zipEntry in zip.Entries)
                            {
                                if (zipEntry.IsDirectory)
                                    continue;

                                zipEntry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                                try
                                {
                                    zipEntry.Extract(mapFolder);
                                }
                                catch (Exception ex)
                                {
                                    ErrorLogger.Instance.Write(string.Format("Error extract file Maps.zip: {0}", ex.Message));
                                }
                            }
                        }
                    }
                    finally
                    {
                        IsUpgrading = false;
                        UpgradeComplete(null, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Instance.Write(string.Format("Error download maps: {0}", ex.Message));
            }

            Settings.Default.Save();
        }

        [NotNull]
        private static string GetMapsFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps");
        }

        [NotNull]
        private static string GetZonesFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps", "MapGenerator", "MapResults");
        }

        [NotNull]
        private static string GetZoneVisitsFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Maps", "ZoneVisits");
        }

        private static void CreateDirectories()
        {
            if (!Directory.Exists(GetMapsFolder()))
            {
                Directory.CreateDirectory(GetMapsFolder());
            }

            if (!Directory.Exists(Path.Combine(SettingsHolder.Instance.Folder, "Maps", "MapGenerator")))
            {
                Directory.CreateDirectory(Path.Combine(SettingsHolder.Instance.Folder, "Maps", "MapGenerator"));
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
