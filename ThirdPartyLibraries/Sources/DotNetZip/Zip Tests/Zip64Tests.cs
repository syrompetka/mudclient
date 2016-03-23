// Zip64Tests.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009-2010 Dino Chiesa
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs):
// Time-stamp: <2010-January-21 11:29:11>
//
// ------------------------------------------------------------------
//
// This module defines the tests for the ZIP64 capability within DotNetZip.
//
// ------------------------------------------------------------------


// define REMOTE_FILESYSTEM in order to use a remote filesystem for storage of
// the ZIP64 large archive (which can beb huge). Leave it undefined to simply
// use the local TEMP directory.
#define REMOTE_FILESYSTEM


using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;



namespace Ionic.Zip.Tests.Zip64
{

    /// <summary>
    /// Summary description for Zip64Tests
    /// </summary>
    [TestClass]
    public class Zip64Tests : IonicTestClass
    {

#if REMOTE_FILESYSTEM
                string homeDir = "t:\\tdir";
#else
                string homeDir = System.Environment.GetEnvironmentVariable("TEMP");
#endif

        public Zip64Tests() : base() { }

        private static string _HugeZipFile;
        private string GetHugeZipFile()
        {
            if (_HugeZipFile == null)
            {
                _HugeZipFile = _CreateHugeZipfile();
            }
            return _HugeZipFile;
        }


        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanupEx()
        {
            if (_txrx!=null)
            {
                try
                {
                    _txrx.Send("stop");
                    _txrx = null;
                }
                catch { }
            }
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (_HugeZipFile != null)
            {
                // Keep this huge zip file around, because it takes so much
                // time to create it. But Delete the directory if the file no
                // longer exists.
                if (!File.Exists(_HugeZipFile))
                {
                    //File.Delete(_HugeZipFile);
                    string d= Path.GetDirectoryName(_HugeZipFile);
                    if (Directory.Exists(d))
                        Directory.Delete(d, true);
                }
            }
        }

        EncryptionAlgorithm[] crypto =
            {
                EncryptionAlgorithm.None,
                EncryptionAlgorithm.PkzipWeak,
                EncryptionAlgorithm.WinZipAes128,
                EncryptionAlgorithm.WinZipAes256,
            };

        Ionic.Zlib.CompressionLevel[] compLevels =
            {
                Ionic.Zlib.CompressionLevel.None,
                Ionic.Zlib.CompressionLevel.BestSpeed,
                Ionic.Zlib.CompressionLevel.Default,
                Ionic.Zlib.CompressionLevel.BestCompression,
            };

        Zip64Option[] z64 =
            {
                Zip64Option.Never,
                Zip64Option.AsNecessary,
                Zip64Option.Always,
            };


        private Object LOCK = new Object();


        private string _CreateHugeZipfile()
        {
            lock (LOCK)
            {
                // look for existing directories, and re-use the large zip file there, if it exists.
                var oldDirs = Directory.GetDirectories(homeDir, "*.Zip64Tests");
                string zipFileToCreate = null;
                foreach (var dir in oldDirs)
                {
                    zipFileToCreate = Path.Combine(dir, "Zip64Data.zip");
                    if (File.Exists(zipFileToCreate))
                    {
                        FileInfo fi = new FileInfo(zipFileToCreate);
                        if (fi.Length < (long)System.UInt32.MaxValue)
                        {
                            TestContext.WriteLine("Deleting an existing zip file: {0}", zipFileToCreate);
                            File.Delete(zipFileToCreate);
                            Directory.Delete(Path.GetDirectoryName(zipFileToCreate), true);
                        }
                        else
                            break;
                    }
                }


                if (zipFileToCreate != null && File.Exists(zipFileToCreate))
                {
                    // the large zip exists, let's use it.
                    TestContext.WriteLine("Using the existing zip file: {0}", zipFileToCreate);
                    return zipFileToCreate;
                }
                else
                {
                    // remember this directory so we can restore later
                    string originalDir = Directory.GetCurrentDirectory();
                    //string testDir = TestUtilities.GenerateUniquePathname("Zip64Tests");

                    string pname = Path.GetFileName(TestUtilities.GenerateUniquePathname("Zip64Tests"));

                    string testDir = Path.Combine(homeDir, pname);

                    // CurrentDir is the dir that holds the test temp directory (or directorIES)
                    Directory.SetCurrentDirectory(CurrentDir);
                    Directory.CreateDirectory(testDir);
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(testDir));

                    zipFileToCreate = Path.Combine(testDir, "Zip64Data.zip");

                    TestContext.WriteLine("Creating a new zip file: {0}", zipFileToCreate);

                    // create a huge ZIP64 archive with a true 64-bit offset.
                    string progressChannel = "Zip64_Setup";
                    StartProgressMonitor(progressChannel);
                    StartProgressClient(progressChannel, "Test Setup", "Creating files");

                    _txrx.Send("bars 3");
                    _txrx.Send(String.Format("pb 0 max {0}", 3));

                    Directory.SetCurrentDirectory(testDir);

                    // create a directory with some files in it, to zip
#if REMOTE_FILESYSTEM
                    string dirToZip = "t:\\tdir\\largefiles";
#else
                    string dirToZip = "dir";
                    Directory.CreateDirectory(dirToZip);
#endif
                    // these params define the size range for the large, random text files
                    // that are created in the opener delegate.
                    _sizeBase =   0x20000000;
                    _sizeRandom = 0x01000000;

                    _txrx.Send("pb 0 step");

                    // Add links to a few very large files into the same directory.
                    // We do this because creating such large files will take a very very long time.
                    CreateLinksToLargeFiles(dirToZip);

                    Directory.SetCurrentDirectory(testDir); // again

                    OpenDelegate opener = (x) =>
                        {
                            return new Ionic.Zip.Tests.Utilities.RandomTextInputStream(_sizeBase + _rnd.Next(_sizeRandom));
                        };


                    // pb1 and pb2 will be set in the SaveProgress handler
                    _txrx.Send("pb 0 step");
                    _txrx.Send("status Saving the zip...");
                    _pb1Set = false;
                    _testTitle = "Zip64 Test Setup";
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.SaveProgress += zip64_SaveProgress;
                        zip.AddProgress += zip64_AddProgress;
                        zip.UpdateDirectory(dirToZip, "");
                        foreach (var e in zip)
                        {
                            if (e.FileName.EndsWith(".pst") ||
                                e.FileName.EndsWith(".ost") ||
                                e.FileName.EndsWith(".zip"))
                                e.CompressionMethod = CompressionMethod.None;
                        }
                        zip.UseZip64WhenSaving = Zip64Option.Always;
                        // use large buffer to speed up save for large files:
                        zip.BufferSize = 1024 * 756;
                        zip.CodecBufferSize = 1024 * 128;

                        // This bit adds a bunch of entries.  The streams are opened in
                        // the opener delegate.
                        int numFilesToAdd = _rnd.Next(4) + 6;
                        for (int i = 0; i < numFilesToAdd; i++)
                        {
                            string filename = TestUtilities.GetOneRandomUppercaseAsciiChar() +
                                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
                            zip.AddEntry(filename, opener, null);
                        }

                        zip.Save(zipFileToCreate);
                    }

                    _txrx.Send("pb 0 step");
                    System.Threading.Thread.Sleep(120);

#if REMOTE_FILESYSTEM
#else
                    Directory.Delete(dirToZip, true);
                    _txrx.Send("pb 0 step");
                    System.Threading.Thread.Sleep(120);
#endif

                    _txrx.Send("stop");

                    // restore the cwd:
                    Directory.SetCurrentDirectory(originalDir);
                }

                return zipFileToCreate;
            }
        }


        private void CreateLinksToLargeFiles(string dirForLinks)
        {
#if REMOTE_FILESYSTEM
#else
            string current = Directory.GetCurrentDirectory();
            _txrx.Send("status Creating links");
            var namesOfLargeFiles = new String[]
                {
                    "c:\\dinoch\\PST\\archive.pst",
                    "c:\\dinoch\\PST\\archive1.pst",
                    "c:\\dinoch\\PST\\Lists.pst",
                    "c:\\dinoch\\PST\\OldStuff.pst",
                    "c:\\dinoch\\PST\\Personal1.pst",
                };

            string subdir = Path.Combine(dirForLinks, "largelinks");
            Directory.CreateDirectory(subdir);
            Directory.SetCurrentDirectory(subdir);
            var w = System.Environment.GetEnvironmentVariable("Windir");
            Assert.IsTrue(Directory.Exists(w), "%windir% does not exist ({0})", w);
            var fsutil = Path.Combine(Path.Combine(w, "system32"), "fsutil.exe");
            Assert.IsTrue(File.Exists(fsutil), "fsutil.exe does not exist ({0})", fsutil);
            string ignored;
            foreach (var f in namesOfLargeFiles)
            {
                Assert.IsTrue(File.Exists(f));
                FileInfo fi = new FileInfo(f);
                Assert.IsTrue(fi.Length > 100);
                string cmd = String.Format("hardlink create {0} {1}", Path.GetFileName(f), f);
                _txrx.Send("status " + cmd);
                TestUtilities.Exec_NoContext(fsutil, cmd, out ignored);
                cmd = String.Format("hardlink create {0}-Copy1{1} {2}",
                                    Path.GetFileNameWithoutExtension(f), Path.GetExtension(f), f);
                TestUtilities.Exec_NoContext(fsutil, cmd, out ignored);
            }
            Directory.SetCurrentDirectory(current);
#endif
        }





        [TestMethod]
        public void Zip64_Create()
        {
            Zip64Option[] Options = { Zip64Option.Always, Zip64Option.Never, Zip64Option.AsNecessary };
            for (int k = 0; k < Options.Length; k++)
            {
                string filename = null;
                Directory.SetCurrentDirectory(TopLevelDir);
                TestContext.WriteLine("\n\n==================Trial {0}...", k);
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Zip64_Create-{0}.zip", k));

                TestContext.WriteLine("Creating file {0}", zipFileToCreate);
                TestContext.WriteLine("  ZIP64 option: {0}", Options[k].ToString());
                int entries = _rnd.Next(5) + 13;
                //int entries = 3;


                var checksums = new Dictionary<string, string>();
                using (ZipFile zip1 = new ZipFile())
                {
                    for (int i = 0; i < entries; i++)
                    {
                        if (_rnd.Next(2) == 1)
                        {
                            filename = Path.Combine(TopLevelDir, String.Format("Data{0}.bin", i));
                            int filesize = _rnd.Next(44000) + 5000;
                            //int filesize = 2000;
                            TestUtilities.CreateAndFillFileBinary(filename, filesize);
                        }
                        else
                        {
                            filename = Path.Combine(TopLevelDir, String.Format("Data{0}.txt", i));
                            int filesize = _rnd.Next(44000) + 5000;
                            //int filesize = 1000;
                            TestUtilities.CreateAndFillFileText(filename, filesize);
                        }
                        zip1.AddFile(filename, "");

                        var chk = TestUtilities.ComputeChecksum(filename);
                        checksums.Add(Path.GetFileName(filename), TestUtilities.CheckSumToString(chk));
                    }

                    zip1.UseZip64WhenSaving = Options[k];
                    zip1.Comment = String.Format("This archive uses zip64 option: {0}", Options[k].ToString());
                    zip1.Save(zipFileToCreate);

                    if (Options[k] == Zip64Option.Always)
                        Assert.IsTrue(zip1.OutputUsedZip64.Value);
                    else if (Options[k] == Zip64Option.Never)
                        Assert.IsFalse(zip1.OutputUsedZip64.Value);

                }

                BasicVerifyZip(zipFileToCreate);

                TestContext.WriteLine("---------------Reading {0}...", zipFileToCreate);
                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    string extractDir = String.Format("extract{0}", k);
                    foreach (var e in zip2)
                    {
                        TestContext.WriteLine(" Entry: {0}  c({1})  unc({2})", e.FileName, e.CompressedSize, e.UncompressedSize);

                        e.Extract(extractDir);
                        filename = Path.Combine(extractDir, e.FileName);
                        string actualCheckString = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filename));
                        Assert.IsTrue(checksums.ContainsKey(e.FileName), "Checksum is missing");
                        Assert.AreEqual<string>(checksums[e.FileName], actualCheckString, "Checksums for ({0}) do not match.", e.FileName);
                        TestContext.WriteLine("     Checksums match ({0}).\n", actualCheckString);
                    }
                }
            }
        }



        [TestMethod]
        public void Zip64_Convert()
        {
            string trialDescription = "Trial {0}/{1}:  create archive as 'zip64={2}', then open it and re-save with 'zip64={3}'";
            Zip64Option[] z64a = {
                Zip64Option.Never,
                Zip64Option.Always,
                Zip64Option.AsNecessary};

            for (int u = 0; u < 2; u++)
            {

                for (int m = 0; m < z64a.Length; m++)
                {
                    for (int n = 0; n < z64a.Length; n++)
                    {
                        int k = m * z64a.Length + n;

                        string filename = null;
                        Directory.SetCurrentDirectory(TopLevelDir);
                        TestContext.WriteLine("\n\n==================Trial {0}...", k);

                        TestContext.WriteLine(trialDescription, k, (z64a.Length * z64a.Length) - 1, z64a[m], z64a[n]);

                        string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Zip64_Convert-{0}.A.zip", k));

                        int entries = _rnd.Next(8) + 6;
                        //int entries = 2;
                        TestContext.WriteLine("Creating file {0}, zip64={1}, {2} entries",
                                              Path.GetFileName(zipFileToCreate), z64a[m].ToString(), entries);

                        var checksums = new Dictionary<string, string>();
                        using (ZipFile zip1 = new ZipFile())
                        {
                            for (int i = 0; i < entries; i++)
                            {
                                if (_rnd.Next(2) == 1)
                                {
                                    filename = Path.Combine(TopLevelDir, String.Format("Data{0}.bin", i));
                                    TestUtilities.CreateAndFillFileBinary(filename, _rnd.Next(44000) + 5000);
                                }
                                else
                                {
                                    filename = Path.Combine(TopLevelDir, String.Format("Data{0}.txt", i));
                                    TestUtilities.CreateAndFillFileText(filename, _rnd.Next(44000) + 5000);
                                }
                                zip1.AddFile(filename, "");

                                var chk = TestUtilities.ComputeChecksum(filename);
                                checksums.Add(Path.GetFileName(filename), TestUtilities.CheckSumToString(chk));
                            }

                            TestContext.WriteLine("---------------Saving to {0} with Zip64={1}...",
                                                  Path.GetFileName(zipFileToCreate), z64a[m].ToString());
                            zip1.UseZip64WhenSaving = z64a[m];
                            zip1.Comment = String.Format("This archive uses Zip64Option={0}", z64a[m].ToString());
                            zip1.Save(zipFileToCreate);
                        }


                        Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entries,
                                             "The Zip file has the wrong number of entries.");


                        string newFile = zipFileToCreate.Replace(".A.", ".B.");
                        using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                        {
                            TestContext.WriteLine("---------------Extracting {0} ...",
                                                  Path.GetFileName(zipFileToCreate));
                            string extractDir = String.Format("extract-{0}-{1}.A", k, u);
                            foreach (var e in zip2)
                            {
                                TestContext.WriteLine(" {0}  crc({1:X8})  c({2:X8}) unc({3:X8})", e.FileName, e.Crc, e.CompressedSize, e.UncompressedSize);

                                e.Extract(extractDir);
                                filename = Path.Combine(extractDir, e.FileName);
                                string actualCheckString = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filename));
                                Assert.IsTrue(checksums.ContainsKey(e.FileName), "Checksum is missing");
                                Assert.AreEqual<string>(checksums[e.FileName], actualCheckString, "Checksums for ({0}) do not match.", e.FileName);
                            }

                            if (u==1)
                            {
                                TestContext.WriteLine("---------------Updating:  Renaming an entry...");
                                zip2[4].FileName += ".renamed";

                                string entriesToRemove = (_rnd.Next(2) == 0) ? "*.txt" : "*.bin";
                                TestContext.WriteLine("---------------Updating:  Removing {0} entries...", entriesToRemove);
                                zip2.RemoveSelectedEntries(entriesToRemove);
                            }

                            TestContext.WriteLine("---------------Saving to {0} with Zip64={1}...",
                                                  Path.GetFileName(newFile), z64a[n].ToString());

                            zip2.UseZip64WhenSaving = z64a[n];
                            zip2.Comment = String.Format("This archive uses Zip64Option={0}", z64a[n].ToString());
                            zip2.Save(newFile);
                        }



                        using (ZipFile zip3 = ZipFile.Read(newFile))
                        {
                            TestContext.WriteLine("---------------Extracting {0} ...",
                                                  Path.GetFileName(newFile));
                            string extractDir = String.Format("extract-{0}-{1}.B", k, u);
                            foreach (var e in zip3)
                            {
                                TestContext.WriteLine(" {0}  crc({1:X8})  c({2:X8}) unc({3:X8})", e.FileName, e.Crc, e.CompressedSize, e.UncompressedSize);

                                e.Extract(extractDir);
                                filename = Path.Combine(extractDir, e.FileName);
                                string actualCheckString = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filename));
                                if (!e.FileName.EndsWith(".renamed"))
                                {
                                    Assert.IsTrue(checksums.ContainsKey(e.FileName), "Checksum is missing");
                                    Assert.AreEqual<string>(checksums[e.FileName], actualCheckString, "Checksums for ({0}) do not match.", e.FileName);
                                }
                            }
                        }
                    }
                }
            }
        }


        Ionic.CopyData.Transceiver _txrx;
        bool _pb2Set;
        bool _pb1Set;


        private string _testTitle;
        private int _sizeBase;
        private int _sizeRandom;
        int _numSaving;
        int _totalToSave;

        private void zip64_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            string msg;
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_Started:
                    _txrx.Send("status saving started...");
                    _pb1Set = false;
                    _numSaving= 1;
                    break;

                case ZipProgressEventType.Saving_BeforeWriteEntry:
                    _txrx.Send(String.Format("status Compressing {0}", e.CurrentEntry.FileName));
                    if (!_pb1Set)
                    {
                        _txrx.Send(String.Format("pb 1 max {0}", e.EntriesTotal));
                        _pb1Set = true;
                    }
                    _totalToSave = e.EntriesTotal;
                    _pb2Set = false;
                    break;

                case ZipProgressEventType.Saving_EntryBytesRead:
                    if (!_pb2Set)
                    {
                        _txrx.Send(String.Format("pb 2 max {0}", e.TotalBytesToTransfer));
                        _pb2Set = true;
                    }
                    _txrx.Send(String.Format("status Saving entry {0}/{1} :: {2} :: {3}/{4}mb {5:N0}%",
                                             _numSaving, _totalToSave,
                                             e.CurrentEntry.FileName,
                                             e.BytesTransferred/(1024*1024), e.TotalBytesToTransfer/(1024*1024),
                                             ((double)e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)));
                    msg = String.Format("pb 2 value {0}", e.BytesTransferred);
                    _txrx.Send(msg);
                    break;

                case ZipProgressEventType.Saving_AfterWriteEntry:
                    _txrx.Send("test " +  _testTitle); // just in case it was missed
                    _txrx.Send("pb 1 step");
                    _numSaving++;
                    break;

                case ZipProgressEventType.Saving_Completed:
                    _txrx.Send("status Save completed");
                    _pb1Set = false;
                    _pb2Set = false;
                    _txrx.Send("pb 1 max 1");
                    _txrx.Send("pb 1 value 1");
                    break;
            }
        }



        void zip64_AddProgress(object sender, AddProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Adding_Started:
                    _txrx.Send("status Adding files to the zip...");
                    break;
                case ZipProgressEventType.Adding_AfterAddEntry:
                    _txrx.Send(String.Format("status Adding file {0}", e.CurrentEntry.FileName));
                    break;
                case ZipProgressEventType.Adding_Completed:
                    _txrx.Send("status Added all files");
                    break;
            }
        }



        private int _numExtracted;
        private int _numFilesToExtract;
        void zip64_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_BeforeExtractEntry:
                    if (!_pb1Set)
                    {
                        _txrx.Send(String.Format("pb 1 max {0}", _numFilesToExtract));
                        _pb1Set = true;
                    }
                    _pb2Set = false;
                    break;

                case ZipProgressEventType.Extracting_EntryBytesWritten:
                    if (!_pb2Set)
                    {
                        _txrx.Send(String.Format("pb 2 max {0}", e.TotalBytesToTransfer));
                        _pb2Set = true;
                    }
                    _txrx.Send(String.Format("status {0} entry {1}/{2} :: {3} :: {4}/{5}mb ::  {6:N0}%",
                                             verb,
                                             _numExtracted, _numFilesToExtract,
                                             e.CurrentEntry.FileName,
                                             e.BytesTransferred/(1024*1024),
                                             e.TotalBytesToTransfer/(1024*1024),
                                             ((double)e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)
                                             ));
                    string msg = String.Format("pb 2 value {0}", e.BytesTransferred);
                    _txrx.Send(msg);
                    break;

                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    _numExtracted++;
                    _txrx.Send("pb 1 step");
                    break;
            }
        }



        string verb;

        private void VerifyZip(string zipfile)
        {
            _pb1Set = false;
            Stream bitBucket = Stream.Null;
            TestContext.WriteLine("\nChecking file {0}", zipfile);
            verb = "Verifying";
            using (ZipFile zip = ZipFile.Read(zipfile))
            {
                zip.BufferSize = 65536*8; // 65536 * 8 = 512k - large buffer better for large files
                _numFilesToExtract = zip.Entries.Count;
                _numExtracted= 1;
                zip.ExtractProgress += zip64_ExtractProgress;
                foreach (var s in zip.EntryFileNames)
                {
                    TestContext.WriteLine("  Entry: {0}", s);
                    zip[s].Extract(bitBucket);
                }
            }
            System.Threading.Thread.Sleep(0x500);
        }



        [Timeout(19400000), TestMethod] // in milliseconds. 7200000 = 2 hours; 13,200,000 = 3:40
        public void Zip64_Update()
        {
            _txrx = new Ionic.CopyData.Transceiver();
            try
            {
                int numUpdates = 2;

                string zipFileToUpdate = GetHugeZipFile(); // this may take a long time
                Assert.IsTrue(File.Exists(zipFileToUpdate), "The required ZIP file does not exist ({0})",  zipFileToUpdate);

                // start the progress monitor
                string progressChannel = "Zip64-Update";
                StartProgressMonitor(progressChannel);
                StartProgressClient(progressChannel,
                                    "Zip64 Update",
                                    "Creating files");

                int baseSize = _rnd.Next(0x1000ff) + 80000;

                _txrx.Send( String.Format("pb 0 max {0}", numUpdates + 1));


                // make sure the zip is larger than the 4.2gb size
                FileInfo fi = new FileInfo(zipFileToUpdate);
                Assert.IsTrue(fi.Length > (long)System.UInt32.MaxValue, "The zip file ({0}) is not large enough.", zipFileToUpdate);

                _txrx.Send("status Verifying the zip");
                VerifyZip(zipFileToUpdate);

                _txrx.Send("pb 0 step");

                var sw = new StringWriter();
                for (int j=0; j < numUpdates; j++)
                {
                    _txrx.Send("test Zip64 Update");
                    // create another folder with a single file in it
                    string subdir = String.Format("newfolder-{0}", j);
                    Directory.CreateDirectory(subdir);
                    string fileName = Path.Combine(subdir, "newfile.txt");
                    long size = baseSize + _rnd.Next(28000);
                    TestUtilities.CreateAndFillFileBinary(fileName, size);

                    TestContext.WriteLine("");
                    TestContext.WriteLine("Updating the zip file...");
                    _txrx.Send("status Updating the zip file...");
                    // update the zip with that new folder+file
                    using (ZipFile zip = ZipFile.Read(zipFileToUpdate))
                    {
                        zip.SaveProgress += zip64_SaveProgress;
                        zip.StatusMessageTextWriter = sw;
                        zip.UpdateDirectory(subdir, subdir);
                        zip.UseZip64WhenSaving = Zip64Option.Always;
                        zip.BufferSize = 65536*8; // 65536 * 8 = 512k
                        zip.Save();
                    }

                string status = sw.ToString();
                TestContext.WriteLine("status:");
                foreach (string line in status.Split('\n'))
                    TestContext.WriteLine(line);


                    _txrx.Send("status Verifying the zip");
                    _txrx.Send("pb 0 step");
                }

                VerifyZip(zipFileToUpdate);

                _txrx.Send("pb 0 step");

                System.Threading.Thread.Sleep(120);
            }
            finally
            {
                if (_txrx!=null)
                {
                    try
                    {
                        _txrx.Send("stop");
                        _txrx = null;
                    }
                    catch { }
                }
            }
        }



        [TestMethod]
        public void Zip64_Winzip_Unzip_OneFile()
        {
            string testBin = TestUtilities.GetTestBinDir(CurrentDir);
            string fileToZip = Path.Combine(testBin, "Ionic.Zip.dll");

            Directory.SetCurrentDirectory(TopLevelDir);

            for (int p=0; p < compLevels.Length; p++)
            {
                for (int n=0; n < crypto.Length; n++)
                {
                    for (int m=0; m < z64.Length; m++)
                    {
                        string zipFile = Path.Combine(TopLevelDir, String.Format("Zip64-Winzip-Unzip.OneFile.{0}.{1}.{2}.zip",
                                                                                 compLevels[p].ToString(),
                                                                                 crypto[n].ToString(),
                                                                                 z64[m].ToString()));
                        string password = Path.GetRandomFileName();

                        TestContext.WriteLine("=================================");
                        TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFile));
                        TestContext.WriteLine("Encryption:{0}  Zip64:{1} pw={2}  compLevel={3}",
                                              crypto[n].ToString(), z64[m].ToString(), password, compLevels[p].ToString());

                        using (var zip = new ZipFile())
                        {
                            zip.Comment = String.Format("Encryption={0}  Zip64={1}  pw={2}",
                                                        crypto[n].ToString(), z64[m].ToString(), password);
                            zip.Encryption = crypto[n];
                            zip.Password = password;
                            zip.CompressionLevel = compLevels[p];
                            zip.UseZip64WhenSaving = z64[m];
                            zip.AddFile(fileToZip, "file");
                            zip.Save(zipFile);
                        }

                        TestContext.WriteLine("Unzipping with WinZip...");

                        string extractDir = String.Format("extract.{0}.{1}.{2}",p,n,m);
                        Directory.CreateDirectory(extractDir);

                        // this will throw if the command has a non-zero exit code.
                        this.Exec(wzunzip,
                                  String.Format("-s{0} -d {1} {2}\\", password, zipFile, extractDir));
                    }
                }
            }
        }



        [Timeout((int)(60 * 60 * 1000 * 5)), TestMethod] // in milliseconds.
        public void Zip64_Winzip_Unzip_Huge()
        {
            try
            {
                string zipFileToExtract = GetHugeZipFile(); // may take a very long time
                Assert.IsTrue(File.Exists(zipFileToExtract), "required ZIP file does not exist ({0})",  zipFileToExtract);

                int baseSize = _rnd.Next(0x1000ff) + 80000;

                string progressChannel = "Zip64-WinZip-Unzip";
                StartProgressMonitor(progressChannel);
                StartProgressClient(progressChannel, "Zip64 WinZip unzip", "Creating files");

                string extractDir = "extract";
                Directory.SetCurrentDirectory(TopLevelDir);
                Directory.CreateDirectory(extractDir);

                _txrx.Send("pb 0 max 3");

                // make sure the zip is larger than the 4.2gb size
                FileInfo fi = new FileInfo(zipFileToExtract);
                Assert.IsTrue(fi.Length > (long)System.UInt32.MaxValue, "The zip file ({0}) is not large enough.", zipFileToExtract);

                // Verifying the zip takes a long time, like an hour. So we skip it.
                // _txrx.Send("status Verifying the zip");
                // VerifyZip(zipFileToUpdate);

                _txrx.Send("pb 0 step");

                _txrx.Send("status Counting entries in the zip file...");

                int numEntries = TestUtilities.CountEntries(zipFileToExtract);

                _txrx.Send("status Using WinZip to list the entries...");

                // examine and unpack the zip archive via WinZip
                // first, examine the zip entry metadata:
                string wzzipOut = this.Exec(wzzip, String.Format("-vt {0}", zipFileToExtract));
                TestContext.WriteLine(wzzipOut);

                int x = 0;
                int y = 0;
                int wzzipEntryCount=0;
                string textToLookFor= "Filename: ";
                TestContext.WriteLine("================");
                TestContext.WriteLine("Files listed by WinZip:");
                while (true)
                {
                    x = wzzipOut.IndexOf(textToLookFor, y);
                    if (x < 0) break;
                    y = wzzipOut.IndexOf("\n", x);
                    string name = wzzipOut.Substring(x + textToLookFor.Length, y-x-1).Trim();
                    TestContext.WriteLine("  {0}", name);
                    if (!name.EndsWith("\\"))
                    {
                        wzzipEntryCount++;
                        if (wzzipEntryCount > numEntries * 3) throw new Exception("too many entries!");
                    }
                }
                TestContext.WriteLine("================");

                Assert.AreEqual(numEntries, wzzipEntryCount, "Unexpected number of entries found by WinZip.");

                _txrx.Send("pb 0 step");
                System.Threading.Thread.Sleep(120);

                _txrx.Send(String.Format("pb 1 max {0}", numEntries*2));
                x=0; y = 0;
                _txrx.Send("status Extracting the entries...");
                int nCycles = 0;
                while (true)
                {
                    _txrx.Send("test Zip64 WinZip extract");
                    x = wzzipOut.IndexOf(textToLookFor, y);
                    if (x < 0) break;
                    if (nCycles > numEntries * 4) throw new Exception("too many entries?");
                    y = wzzipOut.IndexOf("\n", x);
                    string name = wzzipOut.Substring(x + textToLookFor.Length, y-x-1).Trim();
                    if (!name.EndsWith("\\"))
                    {
                        nCycles++;
                        _txrx.Send(String.Format("status Extracting {1}/{2} :: {0}", name, nCycles, wzzipEntryCount));
                        this.Exec(wzunzip,
                                  String.Format("-d {0} {1}\\ {2}", zipFileToExtract, extractDir, name));
                        string path = Path.Combine(extractDir, name);
                        _txrx.Send("pb 1 step");
                        Assert.IsTrue(File.Exists(path), "extracted file ({0}) does not exist", path);
                        File.Delete(path);
                        System.Threading.Thread.Sleep(120);
                        _txrx.Send("pb 1 step");
                    }
                }

                _txrx.Send("pb 0 step");
                System.Threading.Thread.Sleep(120);
            }
            finally
            {
                try
                {
                    if (_txrx!=null)
                    {
                        _txrx.Send("stop");
                        _txrx = null;
                    }
                }
                catch { }
            }
        }




        private void CreateLargeFiles(int numFilesToAdd, int baseSize, string dir)
        {
            bool firstFileDone = false;
            string fileName = "";
            long fileSize = 0;

            _txrx.Send(String.Format("pb 1 max {0}", numFilesToAdd));

            Action<Int64> progressUpdate = (x) =>
                {
                    _txrx.Send(String.Format("pb 2 value {0}", x));
                    _txrx.Send(String.Format("status Creating {0}, [{1}/{2}] ({3:N0}%)", fileName, x, fileSize, ((double)x)/ (0.01 * fileSize) ));
                };

            // It takes a long time to create a large file. And we need
            // a bunch of them.

            for (int i = 0; i < numFilesToAdd; i++)
            {
                int n = _rnd.Next(2);
                fileName = string.Format("Pippo{0}.{1}", i, (n==0) ? "bin" : "txt" );
                if (i != 0)
                {
                    int x = _rnd.Next(6);
                    if (x != 0)
                    {
                        string folderName = string.Format("folder{0}", x);
                        fileName = Path.Combine(folderName, fileName);
                        if (!Directory.Exists(Path.Combine(dir, folderName)))
                            Directory.CreateDirectory(Path.Combine(dir, folderName));
                    }
                }
                fileName = Path.Combine(dir, fileName);
                // first file is 2x larger
                fileSize = (firstFileDone) ? (baseSize + _rnd.Next(0x880000)) : (2*baseSize);
                _txrx.Send(String.Format("pb 2 max {0}", fileSize));
                // randomly select bniary or text
                if (n==0)
                    TestUtilities.CreateAndFillFileBinary(fileName, fileSize, progressUpdate);
                else
                    TestUtilities.CreateAndFillFileText(fileName, fileSize, progressUpdate);
                firstFileDone = true;
                _txrx.Send("pb 1 step");
            }
        }



        [Timeout((int)(60 * 60 * 1000 * 10)), TestMethod] // in milliseconds. 3600 000 0 = 10 hours
        public void Zip64_Winzip_Zip_Huge()
        {
            try
            {
                int baseSize = _rnd.Next(80000) + 0x1000ff;

                string progressChannel = "Zip64-WinZip-Zip-Huge";
                StartProgressMonitor(progressChannel);
                StartProgressClient(progressChannel, "Create Huge ZIP64 via WinZip", "Creating links");

                string extractDir = "extract";
                Directory.SetCurrentDirectory(TopLevelDir);
                Directory.CreateDirectory(extractDir);

                _txrx.Send("pb 0 max 5");

                CreateLinksToLargeFiles(extractDir);

                TestContext.WriteLine("Creating large files...");

                CreateLargeFiles(_rnd.Next(3) + 3, baseSize, extractDir);
                _txrx.Send("pb 0 step");

                TestContext.WriteLine("Creating a new Zip with winzip");

                var fileList = Directory.GetFiles(extractDir, "*.*", SearchOption.AllDirectories);

                // examine and unpack the zip archive via WinZip
                string wzzipOut= null;
                string zipFileToCreate = Path.Combine(TopLevelDir, "Zip64-WinZip-Huge.zip");
                int nCycles= 0;
                _txrx.Send(String.Format("pb 1 max {0}", fileList.Length));
                // Add one file at a time and delete the
                // previously added file. This allows status updates.
                // Not sure about the impact on disk space, though.
                foreach (var filename in fileList)
                {
                    nCycles++;
                    _txrx.Send(String.Format("status adding {0}...({1}/{2})", filename, nCycles, fileList.Length+1));
                    // exec wzzip.exe to create a new zip file
                    wzzipOut = this.Exec(wzzip, String.Format("-a -p -r -yx {0} {1}", zipFileToCreate, filename));
                    TestContext.WriteLine(wzzipOut);
                    _txrx.Send("pb 1 step");
                    File.Delete(filename);
                }

                // Create one small text file and add it to the zip.  For this test,
                // it must be added last, at the end of the ZIP file.
                nCycles++;
                var newfile = Path.Combine(extractDir, "zzz-" + Path.GetRandomFileName() + ".txt");
                _txrx.Send(String.Format("status adding {0}...({1}/{2})", newfile, nCycles, fileList.Length+1));
                int filesize = _rnd.Next(44000) + 5000;
                TestUtilities.CreateAndFillFileText(newfile, filesize);
                wzzipOut = this.Exec(wzzip, String.Format("-a -p -r -yx {0} {1}", zipFileToCreate, newfile));
                TestContext.WriteLine(wzzipOut);
                _txrx.Send("pb 1 step");
                File.Delete(newfile);

                _txrx.Send("pb 0 step");
                System.Threading.Thread.Sleep(120);

                // make sure the zip is larger than the 4.2gb size
                FileInfo fi = new FileInfo(zipFileToCreate);
                Assert.IsTrue(fi.Length > (long)System.UInt32.MaxValue, "The zip file ({0}) is not large enough.", zipFileToCreate);

                // Now use DotNetZip to extract the large zip file to the bit bucket.
                TestContext.WriteLine("Verifying the new Zip with DotNetZip");
                _txrx.Send("status Verifying the zip");
                VerifyZip(zipFileToCreate);

                _txrx.Send("pb 0 step");
                System.Threading.Thread.Sleep(120);

            }
            finally
            {
                try
                {
                    if (_txrx!=null)
                    {
                        _txrx.Send("stop");
                        _txrx = null;
                    }
                }
                catch { }
            }
        }



        [TestMethod, Timeout((int)(60 * 60 * 1000 * 4))] // in milliseconds. 14400000 = 4 hours
        public void Zip64_Winzip_Setup()
        {
            // not really a test.  This thing just sets up the big zip file. Can run it
            // while working on edits for other ZIP64 tests.
            GetHugeZipFile(); // may take a very long time
        }


        void StartProgressMonitor(string progressChannel)
        {
            string testBin = TestUtilities.GetTestBinDir(CurrentDir);
            string progressMonitorTool = Path.Combine(testBin, "Resources\\UnitTestProgressMonitor.exe");
            string requiredDll = Path.Combine(testBin, "Resources\\Ionic.CopyData.dll");
            Assert.IsTrue(File.Exists(progressMonitorTool), "progress monitor tool does not exist ({0})",  progressMonitorTool);
            Assert.IsTrue(File.Exists(requiredDll), "required DLL does not exist ({0})",  requiredDll);

            // start the progress monitor
            string ignored;
            //this.Exec(progressMonitorTool, String.Format("-channel {0}", progressChannel), false);
            TestUtilities.Exec_NoContext(progressMonitorTool, String.Format("-channel {0}", progressChannel), false, out ignored);
        }



        void StartProgressClient(string progressChannel, string title, string initialStatus)
        {
            _txrx = new Ionic.CopyData.Transceiver();
            System.Threading.Thread.Sleep(1000);
            _txrx.Channel = progressChannel;
            System.Threading.Thread.Sleep(450);
            _txrx.Send("test " + title);
            System.Threading.Thread.Sleep(120);
            _txrx.Send("status " + initialStatus);
        }

        private void EmitStatus(String s)
        {
            TestContext.WriteLine("status:");
            foreach (string line in s.Split('\n'))
                TestContext.WriteLine(line);
        }


        [TestMethod, Timeout(25200000)]  // in milliseconds,  25,200,000 = 7 hrs
        public void Zip64_Over_4gb()
        {
            try
            {
                Int64 desiredSize= System.UInt32.MaxValue;
                desiredSize+= System.Int32.MaxValue/4;
                desiredSize+= _rnd.Next(0x1000000);

                string progressChannel = "Zip64-Over-4gb";
                StartProgressMonitor(progressChannel);
                StartProgressClient(progressChannel,
                                    "Zip and Extract huge file",
                                    "starting up...");

                string zipFileToCreate = Path.Combine(TopLevelDir, "Zip64_Over_4gb.zip");
                Directory.SetCurrentDirectory(TopLevelDir);
                string nameOfFodderFile="VeryVeryLargeFile.txt";
                string nameOfExtractedFile = nameOfFodderFile + ".extracted";

                // Steps in this test: 3
                _txrx.Send("pb 0 max 3");

                // create a very large file
                Action<Int64> progressUpdate = (x) =>
                    {
                        _txrx.Send(String.Format("pb 1 value {0}", x));
                        _txrx.Send(String.Format("status Creating {0}, [{1}/{2}mb] ({3:N0}%)",
                                                 nameOfFodderFile,
                                                 x/(1024*1024),
                                                 desiredSize/(1024*1024),
                                                 ((double)x)/ (0.01 * desiredSize)));
                    };


                // This will take ~1 hour
                _txrx.Send(String.Format("pb 1 max {0}", desiredSize));
                TestUtilities.CreateAndFillFileText(nameOfFodderFile,
                                                    desiredSize,
                                                    progressUpdate);

                // make sure it is larger than 4.2gb
                FileInfo fi = new FileInfo(nameOfFodderFile);
                Assert.IsTrue(fi.Length > (long)System.UInt32.MaxValue,
                              "The fodder file ({0}) is not large enough.",
                              nameOfFodderFile);

                _txrx.Send("pb 0 step");

                var sw = new StringWriter();
                using (var zip = new ZipFile())
                {
                    zip.StatusMessageTextWriter = sw;
                    zip.UseZip64WhenSaving = Zip64Option.Always;
                    zip.BufferSize = 65536*8; // 65536 * 8 = 512k
                    zip.SaveProgress += zip64_SaveProgress;
                    var e = zip.AddFile(nameOfFodderFile, "");
                    _txrx.Send("status Saving......");
                    TestContext.WriteLine("zipping one file......");
                    zip.Save(zipFileToCreate);
                }

                EmitStatus(sw.ToString());

                File.Delete(nameOfFodderFile);
                _txrx.Send("status Extracting the file...");
                _txrx.Send("pb 0 step");

                var options = new ReadOptions { StatusMessageWriter= new StringWriter() };
                verb = "Extracting";
                using (var zip = ZipFile.Read(zipFileToCreate, options))
                {
                    Assert.AreEqual<int>(1, zip.Entries.Count,
                                         "Incorrect number of entries in the zip file");
                    zip.ExtractProgress += zip64_ExtractProgress;
                    _numFilesToExtract = zip.Entries.Count;
                    _numExtracted= 1;
                    ZipEntry e = zip[0];
                    e.FileName = nameOfExtractedFile;
                    TestContext.WriteLine("extracting one file......");
                    _txrx.Send("status extracting......");
                    e.Extract();
                }

                EmitStatus(options.StatusMessageWriter.ToString());

                _txrx.Send("pb 0 step");

                System.Threading.Thread.Sleep(120);
            }
            finally
            {
                if (_txrx!=null)
                {
                    try
                    {
                        _txrx.Send("stop");
                        _txrx = null;
                    }
                    catch { }
                }
            }
        }


        [TestMethod, Timeout(3600000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_NoEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.None, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(3600000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_PkZipEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.PkzipWeak, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(7200000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_WinZipEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.WinZipAes256, Ionic.Zlib.CompressionLevel.Default);
        }


        [TestMethod, Timeout(3600000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_NoEncryption_DefaultCompression_Always()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.Always, EncryptionAlgorithm.None, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(3600000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_PkZipEncryption_DefaultCompression_Always()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.Always, EncryptionAlgorithm.PkzipWeak, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(7200000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void Zip64_Over_65534_Entries_WinZipEncryption_DefaultCompression_Always()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.Always, EncryptionAlgorithm.WinZipAes256, Ionic.Zlib.CompressionLevel.Default);
        }




        [TestMethod, Timeout(30 * 60 * 1000)]  // timeout is in milliseconds, (30 * 60 * 1000) = 30 mins
        [ExpectedException(typeof(Ionic.Zip.ZipException))]
        public void Zip64_Over_65534_Entries_NOZIP64()
        {
            _Internal_Zip64_Over_65534_Entries(Zip64Option.Never, EncryptionAlgorithm.None, Ionic.Zlib.CompressionLevel.Default);
        }


        int fileCount;
        public void _Internal_Zip64_Over_65534_Entries(Zip64Option z64option, EncryptionAlgorithm encryption, Ionic.Zlib.CompressionLevel compression)
        {
            // Emitting a zip file with > 65534 entries requires the use of ZIP64 in the central directory.
            fileCount = _rnd.Next(7616)+65534;
            //fileCount = _rnd.Next(761)+6534;

            string zipFileToCreate = String.Format("Zip64.ZipFile_Over_65534_Entries.{0}.{1}.{2}.zip",
                                                   z64option.ToString(), encryption.ToString(), compression.ToString());

            StartProgressMonitor(zipFileToCreate);
            StartProgressClient(zipFileToCreate,
                                String.Format("ZipFile, {0} entries, E({1}), C({2})", fileCount,encryption.ToString(), compression.ToString()),
                                "starting up...");

            _txrx.Send("pb 0 max 3"); // 3 stages: AddEntry, Save, Verify
            _txrx.Send("pb 0 value 0");

            string password = Path.GetRandomFileName();

            string statusString = String.Format("status Encryption:{0} Compression:{1}...",
                                                encryption.ToString(),
                                                compression.ToString());
            _txrx.Send(statusString);
            int dirCount= 0;
            using (var zip = new ZipFile())
            {
                _txrx.Send(String.Format("pb 1 max {0}", fileCount/4));
                _txrx.Send("pb 1 value 0");

                zip.Password = password;
                zip.Encryption = encryption;
                zip.CompressionLevel = compression;
                zip.SaveProgress += SaveProgress2;
                zip.UseZip64WhenSaving = z64option;
                // save space when saving the file:
                zip.EmitTimesInWindowsFormatWhenSaving = false;
                zip.EmitTimesInUnixFormatWhenSaving = false;

                for (int m=0; m<fileCount;  m++)
                {
                    if (_rnd.Next(7)==0)
                    {
                        string entryName = String.Format("{0:D5}", m);
                        zip.AddDirectoryByName(entryName);
                        dirCount++;
                    }
                    else
                    {
                        string entryName = String.Format("{0:D5}.txt", m);
                        if (_rnd.Next(8)==0)
                        {
                            string content = String.Format("This is the content for entry #{0}.", m);
                            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(content);
                            zip.AddEntry(entryName, buffer);
                        }
                        else
                            zip.AddEntry(entryName, Stream.Null);
                    }

                    if (m % 4 == 0)
                        _txrx.Send("pb 1 step");

                    if (m % 100 == 0)
                        _txrx.Send(statusString + " count " + m.ToString());
                }

                _txrx.Send("pb 0 step");
                _txrx.Send(statusString + " Saving...");
                zip.Save(zipFileToCreate);
            }

            _txrx.Send("pb 0 step");
            _txrx.Send(statusString + " Verifying...");

            // exec WinZip. But the output is really large, so we pass emitOutput=false .
            BasicVerifyZip(zipFileToCreate, password, false);

            Assert.AreEqual<int>(fileCount-dirCount, TestUtilities.CountEntries(zipFileToCreate),
                                 "{0}: The zip file created has the wrong number of entries.", zipFileToCreate);

            _txrx.Send("stop");
        }



        private void SaveProgress2(object sender, SaveProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_Started:
                    _txrx.Send("status saving started...");
                    _txrx.Send(String.Format("pb 1 max {0}", fileCount));
                    _numSaving= 0;
                    break;

                case ZipProgressEventType.Saving_AfterWriteEntry:
                    _txrx.Send("pb 1 step");
                    _numSaving++;
                    if (_numSaving % 50 == 0)
                    _txrx.Send(String.Format("status Saving entry {0}/{1}",
                                             _numSaving, fileCount));
                    break;

                case ZipProgressEventType.Saving_Completed:
                    _txrx.Send("status Save completed");
                    _txrx.Send("pb 0 value 2");
                    _txrx.Send("pb 1 max 1");
                    _txrx.Send("pb 1 value 1");
                    break;
            }
        }




        [Timeout(24000000), TestMethod]    // 18,000,000 = 7.5 hrs
        public void Zip64_AutomaticSelection_wi9214()
        {
            //string zipFileToCreate = Path.Combine(TopLevelDir, "Zip64-AutoSelect.zip");
            string zipFileToCreate = Path.Combine("t:\\tdir", "Zip64-AutoSelect.zip");
            // Test whether opening a zip64 file and then re-saving, allows the file to remain valid.
            // Must use a huge zpi64 file (bonafide over 4gb) in order to verify this.
            // want to check that UseZip64WhenSaving is automatically selected as appropriate.

            Assert.IsFalse(File.Exists(zipFileToCreate), "The ZIP file already exists ({0})",  zipFileToCreate);

            _txrx = new Ionic.CopyData.Transceiver();
            try
            {
                string newComment = "This is an updated comment on the first entry > 4gb. (" + DateTime.Now.ToString("u") +")";
                string zipFileToUpdate = GetHugeZipFile(); // this may take a long time
                Assert.IsTrue(File.Exists(zipFileToUpdate), "The required ZIP file does not exist ({0})",  zipFileToUpdate);

                // start the progress monitor
                string progressChannel = "Zip64-AutoSelect";
                StartProgressMonitor(progressChannel);
                StartProgressClient(progressChannel, "Zip64 Auto", "Creating files");

                // make sure the zip is larger than the 4.2gb size
                FileInfo fi = new FileInfo(zipFileToUpdate);
                Assert.IsTrue(fi.Length > (long)System.UInt32.MaxValue, "The zip file ({0}) is not large enough.", zipFileToUpdate);
                TestContext.WriteLine("Verifying the zip file...");
                _txrx.Send("status Verifying the zip");
                VerifyZip(zipFileToUpdate); // can take an hour or more

                TestContext.WriteLine("Updating the zip file...");
                _txrx.Send("status Updating the zip file...");
                // update the zip with that new folder+file
                var sw = new StringWriter();
                using (ZipFile zip = ZipFile.Read(zipFileToUpdate))
                {
                    // required:  the option must be set automatically and intelligently
                    Assert.IsTrue(zip.UseZip64WhenSaving == Zip64Option.Always,
                                  "The UseZip64WhenSaving option is set incorrectly ({0})",
                                  zip.UseZip64WhenSaving);

                    // according to workitem 9214, the comment must be modified on an entry larger than 4gb.
                    ZipEntry e = null;
                    foreach (var e2 in zip)
                    {
                        if (e2.UncompressedSize > (long)System.UInt32.MaxValue)
                        {
                            e = e2;
                            break;
                        }
                    }
                    Assert.IsTrue(e != null &&  e.UncompressedSize > (long)System.UInt32.MaxValue,
                                  "No entry in the zip file is large enough.");

                    e.Comment = newComment;
                    zip.SaveProgress += zip64_SaveProgress;
                    zip.StatusMessageTextWriter = sw;
                    zip.Save(zipFileToCreate);
                }
                string status = sw.ToString();
                TestContext.WriteLine("status:");
                foreach (string line in status.Split('\n'))
                    TestContext.WriteLine(line);

                _txrx.Send("status Verifying the updated zip");
                VerifyZip(zipFileToCreate); // can take an hour or more

                // finally, verify that the comment is correct.
                _txrx.Send("status checking the updated comment");
                using (ZipFile zip = ZipFile.Read(zipFileToCreate))
                {
                    // required:  the option must eb set automatically and intelligently
                    Assert.IsTrue(zip.UseZip64WhenSaving == Zip64Option.Always,
                                  "The UseZip64WhenSaving option is set incorrectly ({0})",
                                  zip.UseZip64WhenSaving);

                    ZipEntry e = null;
                    foreach (var e2 in zip)
                    {
                        if (e2.UncompressedSize > (long)System.UInt32.MaxValue)
                        {
                            e = e2;
                            break;
                        }
                    }
                    Assert.IsTrue(e != null &&  e.UncompressedSize > (long)System.UInt32.MaxValue,
                                  "No entry in the zip file is large enough.");

                    Assert.AreEqual<String>(newComment, e.Comment, "The comment on the entry is unexpected.");

                    TestContext.WriteLine("The comment on the entry is {0}", e.Comment);
                }
            }
            finally
            {
                if (_txrx!=null)
                {
                    try
                    {
                        _txrx.Send("stop");
                        _txrx = null;
                    }
                    catch { }
                }

                if (File.Exists(zipFileToCreate))
                {
                    try
                    {
                        File.Delete(zipFileToCreate);
                    }
                    catch { }
                }
            }


        }



    }

}
