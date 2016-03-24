// Streams.cs
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
// Time-stamp: <2010-February-10 12:13:27>
//
// ------------------------------------------------------------------
//
// This module defines tests for Streams interfaces into DotNetZip.
// ZipOutputStream, ZipInputStream, etc
//
// ------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;


namespace Ionic.Zip.Tests.Streams
{
    /// <summary>
    /// Summary description for StreamsTests
    /// </summary>
    [TestClass]
    public class StreamsTests : IonicTestClass
    {
        Ionic.CopyData.Transceiver _txrx;


        public StreamsTests() : base() { }


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


        EncryptionAlgorithm[] crypto =
            {
                EncryptionAlgorithm.None,
                EncryptionAlgorithm.PkzipWeak,
                EncryptionAlgorithm.WinZipAes128,
                EncryptionAlgorithm.WinZipAes256,
            };

#if NOT
        EncryptionAlgorithm[] cryptoNoPkzip =
            {
                EncryptionAlgorithm.None,
                EncryptionAlgorithm.WinZipAes128,
                EncryptionAlgorithm.WinZipAes256,
            };
#endif

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




        [TestMethod]
        public void ReadZip_OpenReader()
        {
            string[] passwords = { null, Path.GetRandomFileName(), "EE", "***()" };

            for (int j = 0; j < compLevels.Length; j++)
            {
                for (int k = 0; k < passwords.Length; k++)
                {
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("ReadZip_OpenReader-{0}-{1}.zip", j, k));

                    int entriesAdded = 0;
                    String filename = null;

                    string subdir = Path.Combine(TopLevelDir, String.Format("A{0}{1}", j, k));
                    Directory.CreateDirectory(subdir);

                    int fileCount = _rnd.Next(10) + 10;
                    for (int i = 0; i < fileCount; i++)
                    {
                        filename = Path.Combine(subdir, String.Format("file{0:D2}.txt", i));
                        int filesize = _rnd.Next(34000) + 5000;
                        TestUtilities.CreateAndFillFileText(filename, filesize);
                        entriesAdded++;
                    }

                    using (ZipFile zip1 = new ZipFile())
                    {
                        zip1.CompressionLevel = compLevels[j];
                        zip1.Password = passwords[k];
                        zip1.AddDirectory(subdir, Path.GetFileName(subdir));
                        zip1.Save(zipFileToCreate);
                    }

                    // Verify the files are in the zip
                    Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entriesAdded,
                                         String.Format("Trial {0}-{1}: The Zip file has the wrong number of entries.", j, k));

                    // now extract the files and verify their contents
                    using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            // try once with Password set on ZipFile,
                            // another with password on the entry, and
                            // a third time with password passed into the OpenReader() method.
                            if (i == 0)
                                zip2.Password = passwords[k];

                            foreach (string eName in zip2.EntryFileNames)
                            {
                                ZipEntry e1 = zip2[eName];

                                if (!e1.IsDirectory)
                                {

                                    Ionic.Zlib.CrcCalculatorStream s = null;
                                    try
                                    {
                                        if (i == 0)
                                            s = e1.OpenReader();
                                        else if (i == 1)
                                            s = e1.OpenReader(passwords[k]);
                                        else
                                        {
                                            e1.Password = passwords[k];
                                            s = e1.OpenReader();
                                        }
                                        string outFile = Path.Combine(TopLevelDir, String.Format("{0}.{1}.out", eName, i));
                                        using (var output = File.Create(outFile))
                                        {
                                            byte[] buffer = new byte[4096];
                                            int n, totalBytesRead = 0;
                                            do
                                            {
                                                n = s.Read(buffer, 0, buffer.Length);
                                                totalBytesRead += n;
                                                output.Write(buffer, 0, n);
                                            } while (n > 0);

                                            output.Flush();
                                            output.Close();
                                            TestContext.WriteLine("CRC expected({0:X8}) actual({1:X8})",
                                                                  e1.Crc, s.Crc);

                                            Assert.AreEqual<Int32>(s.Crc, e1.Crc,
                                                                   string.Format("The Entry {0} failed the CRC Check.", eName));

                                            Assert.AreEqual<Int32>(totalBytesRead, (int)e1.UncompressedSize,
                                                                   string.Format("We read an unexpected number of bytes. ({0})", eName));
                                        }
                                    }
                                    finally
                                    {
                                        if (s != null)
                                            s.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }




        [TestMethod]
        public void AddEntry_JitProvided()
        {
            for (int i = 0; i < crypto.Length; i++)
            {
                for (int k = 0; k < compLevels.Length; k++)
                {
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("AddEntry_JitProvided.{0}.{1}.zip", i, k));

                    string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                    var files = TestUtilities.GenerateFilesFlat(dirToZip);

                    string password = Path.GetRandomFileName();

                    using (var zip = new ZipFile())
                    {
                        TestContext.WriteLine("=================================");
                        TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFileToCreate));
                        TestContext.WriteLine("Encryption({0})  Compression({1})  pw({2})",
                                              crypto[i].ToString(), compLevels[k].ToString(), password);

                        zip.Password = password;
                        zip.Encryption = crypto[i];
                        zip.CompressionLevel = compLevels[k];

                        foreach (var file in files)
                            zip.AddEntry(file,
                                         (name) => File.Open(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                                         (name, stream) => stream.Close()
                                         );
                        zip.Save(zipFileToCreate);
                    }

                    if (crypto[i] == EncryptionAlgorithm.None)
                        BasicVerifyZip(zipFileToCreate);
                    else
                        BasicVerifyZip(zipFileToCreate, password);

                    Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                         "Trial ({0},{1}): The zip file created has the wrong number of entries.", i, k);
                }
            }
        }



        private delegate void TestCompressionLevels(string[] files, EncryptionAlgorithm crypto, bool seekable, int cycle, string format, int fileOutputOption);

        private void _TestDriver(TestCompressionLevels test, string label, bool seekable, bool zero)
        {
            _TestDriver(test, label, seekable, zero, 0);
        }


        private void _TestDriver(TestCompressionLevels test, string label, bool seekable, bool zero, int fileOutputOption)
        {
            int[] fileCounts = new int[] { 1, 2, _rnd.Next(14) + 13 };

            for (int j = 0; j < fileCounts.Length; j++)
            {
                string dirToZip = String.Format("subdir{0}", j);
                string[] files = null;
                if (zero)
                {
                    // zero length files
                    Directory.CreateDirectory(dirToZip);
                    files = new string[fileCounts[j]];
                    for (int i = 0; i < fileCounts[j]; i++)
                        files[i] = TestUtilities.CreateUniqueFile("zerolength", dirToZip);
                }
                else
                    files = TestUtilities.GenerateFilesFlat(dirToZip, fileCounts[j], 40000, 72000);


                for (int i = 0; i < crypto.Length; i++)
                {
                    string format = String.Format("{0}.{1}.filecount{2}.Encryption.{3}.{4}seekable.{5}.zip",
                                                  label,
                                                  (zero) ? "ZeroBytes" : "regular",
                                                  fileCounts[j],
                                                  crypto[i].ToString(),
                                                  seekable ? "" : "non",
                                                  "{0}");

                    test(files, crypto[i], seekable, i, format, fileOutputOption);
                }
            }
        }



        private void _Internal_AddEntry_WriteDelegate(string[] files,
                                                      EncryptionAlgorithm crypto,
                                                      bool seekable,
                                                      int cycle,
                                                      string format,
                                                      int ignored)
        {
            int BufferSize = 2048;


            for (int k = 0; k < compLevels.Length; k++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format(format, k));
                string password = Path.GetRandomFileName();

                using (var zip = new ZipFile())
                {
                    TestContext.WriteLine("=================================");
                    TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFileToCreate));
                    TestContext.WriteLine("Encryption({0})  Compression({1})  pw({2})",
                                          crypto.ToString(), compLevels[k].ToString(), password);

                    zip.Password = password;
                    zip.Encryption = crypto;
                    zip.CompressionLevel = compLevels[k];

                    foreach (var file in files)
                    {
                        zip.AddEntry(file, (name, output) =>
                            {
                                using (var input = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    int n;
                                    while ((n = input.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        output.Write(buffer, 0, n);
                                    }
                                }
                            });
                    }


                    if (!seekable)
                    {
                        // conditionally use a non-seekable output stream
                        using (Stream raw = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
                        {
                            using (var ns = new Ionic.Zip.Tests.NonSeekableOutputStream(raw))
                            {
                                zip.Save(ns);
                            }
                        }
                    }
                    else
                        zip.Save(zipFileToCreate);
                }

                BasicVerifyZip(zipFileToCreate, password);

                Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                     "Trial ({0},{1}): The zip file created has the wrong number of entries.", cycle, k);
            }
        }



        [TestMethod]
        public void AddEntry_WriteDelegate()
        {
            _TestDriver(new TestCompressionLevels(_Internal_AddEntry_WriteDelegate), "WriteDelegate", true, false);
        }


        [TestMethod]
        public void AddEntry_WriteDelegate_NonSeekable()
        {
            _TestDriver(new TestCompressionLevels(_Internal_AddEntry_WriteDelegate), "WriteDelegate", false, false);
        }


        [TestMethod]
        public void AddEntry_WriteDelegate_ZeroBytes_wi8931()
        {
            _TestDriver(new TestCompressionLevels(_Internal_AddEntry_WriteDelegate), "WriteDelegate", true, true);
        }



        [TestMethod]
        public void Create_ZipOutputStream_ZeroBytes_Encrypt_NonSeekable()
        {
            // There was a moment when using ZipOutputStream with Encryption and a
            // non-seekable output stream, did not work.  DotNetZip was changed to be
            // smarter, so that works now. This test used to verify that the combination
            // of stuff failed.  It now should succeed.

            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            Directory.CreateDirectory(dirToZip);
            int fileCount = _rnd.Next(4)+1;
            string[] files = new string[fileCount];
            for (int i = 0; i < fileCount; i++)
                files[i] = TestUtilities.CreateUniqueFile("zerolength", dirToZip);

            for (int i = 0; i < crypto.Length; i++)
            {
                string format = String.Format("ZipOutputStream.ZeroBytes.filecount{0}.Encryption.{1}.NonSeekable.{2}.zip",
                                              fileCount,
                                              crypto[i],
                                              "{0}");

                _Internal_Create_ZipOutputStream(files, EncryptionAlgorithm.PkzipWeak, false, 99, format);
            }
        }



        [TestMethod, Timeout(1800000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void ZipOutputStream_over_65534_Entries_PkZipEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_ZipOutputStream_Zip64_over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.PkzipWeak, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(7200000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void ZipOutputStream_over_65534_Entries_WinZipEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_ZipOutputStream_Zip64_over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.WinZipAes256, Ionic.Zlib.CompressionLevel.Default);
        }

        [TestMethod, Timeout(1800000)]  // timeout is in milliseconds, (3600 * 1000) = 1 hour;
        public void ZipOutputStream_over_65534_Entries_NoEncryption_DefaultCompression_AsNecessary()
        {
            _Internal_ZipOutputStream_Zip64_over_65534_Entries(Zip64Option.AsNecessary, EncryptionAlgorithm.None, Ionic.Zlib.CompressionLevel.Default);
        }


        [TestMethod, Timeout(30 * 60 * 1000)]  // timeout is in milliseconds, (30 * 60 * 1000) = 30 mins
[ExpectedException(typeof(System.InvalidOperationException))]
public void ZipOutputStream_Zip64_over_65534_Entries_FAIL()
        {
            _Internal_ZipOutputStream_Zip64_over_65534_Entries(Zip64Option.Never, EncryptionAlgorithm.PkzipWeak, Ionic.Zlib.CompressionLevel.Default);
        }


        int fileCount;
        private void _Internal_ZipOutputStream_Zip64_over_65534_Entries(Zip64Option z64option, EncryptionAlgorithm encryption, Ionic.Zlib.CompressionLevel compression)
        {
            // Emitting a zip file with > 65534 entries requires the use of ZIP64 in the central directory.
            fileCount = _rnd.Next(7616)+65534;
            string txrxLabel = String.Format("ZipOutputStream #({0}) E({1}) C({2})",
                                             fileCount,encryption.ToString(), compression.ToString());
            string zipFileToCreate = String.Format("ZipOutputStream.Zip64.over_65534_Entries.{0}.{1}.{2}.zip",
                                                   z64option.ToString(), encryption.ToString(), compression.ToString());

            StartProgressMonitor(zipFileToCreate);
            StartProgressClient(zipFileToCreate, txrxLabel, "starting up...");

            _txrx.Send("pb 0 max 2"); // 2 stages: Write, Verify
            _txrx.Send("pb 0 value 0");

            string password = Path.GetRandomFileName();

            string statusString = String.Format("status Encryption:{0} Compression:{1}...",
                                                encryption.ToString(),
                                                compression.ToString());
            _txrx.Send(statusString);

            int dirCount= 0;

            using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var output = new ZipOutputStream(fs))
                {
                    _txrx.Send("test " + txrxLabel);
                    System.Threading.Thread.Sleep(400);
                    _txrx.Send(String.Format("pb 1 max {0}", fileCount/4));
                    _txrx.Send("pb 1 value 0");

                    output.Password = password;
                    output.Encryption = encryption;
                    output.CompressionLevel = compression;
                    output.EnableZip64 = z64option;
                    for (int k=0; k<fileCount;  k++)
                    {
                        if (_rnd.Next(7)==0)
                        {
                            // make it a directory
                            string entryName = String.Format("{0:D4}/", k);
                            output.PutNextEntry(entryName);
                            dirCount++;
                        }
                        else
                        {
                            string entryName = String.Format("{0:D4}.txt", k);
                            output.PutNextEntry(entryName);

                            if (_rnd.Next(8)==0)
                            {
                                string content = String.Format("This is the content for entry #{0}.", k);
                                byte[] buffer = Encoding.ASCII.GetBytes(content);
                                output.Write(buffer, 0, buffer.Length);
                            }
                        }
                        if (k % 4 == 0)
                            _txrx.Send("pb 1 step");

                        if (k % 128 == 0)
                            _txrx.Send(String.Format("{0} ({1}/{2}) {3:N0}%",
                                                     statusString, k, fileCount,
                                                     ((double)k)/(0.01 * fileCount)));
                    }
                }
            }

            _txrx.Send("pb 1 max 1");
            _txrx.Send("pb 1 value 1");
            _txrx.Send("pb 0 step");

            System.Threading.Thread.Sleep(400);
            _txrx.Send(statusString + " Verifying...");

            // exec WinZip. But the output is really large, so we pass emitOutput=false .
            BasicVerifyZip(zipFileToCreate, password, false);

            Assert.AreEqual<int>(fileCount-dirCount, TestUtilities.CountEntries(zipFileToCreate),
                                 "{0}: The zip file created has the wrong number of entries.", zipFileToCreate);
            _txrx.Send("stop");
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




        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Create_ZipOutputStream_WriteBeforePutNextEntry()
        {
            string zipFileToCreate = "Create_ZipOutputStream_WriteBeforePutNextEntry.zip";
            using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var output = new ZipOutputStream(fs))
                {
                    //output.PutNextEntry("entry1.txt");
                    byte[] buffer = Encoding.ASCII.GetBytes("This is the content for entry #1.");
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }




        [TestMethod]
        public void Create_ZipOutputStream_Directories()
        {
            for (int i = 0; i < crypto.Length; i++)
            {
                for (int j = 0; j < compLevels.Length; j++)
                {
                    string password = Path.GetRandomFileName();

                    for (int k = 0; k < 2; k++)
                    {
                        string zipFileToCreate = String.Format("Create_ZipOutputStream_Directories.Encryption.{0}.{1}.{2}.zip",
                                                               crypto[i].ToString(), compLevels[j].ToString(), k);

                        using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
                        {
                            using (var output = new ZipOutputStream(fs))
                            {
                                byte[] buffer;
                                output.Password = password;
                                output.Encryption = crypto[i];
                                output.CompressionLevel = compLevels[j];
                                output.PutNextEntry("entry1.txt");
                                if (k == 0)
                                {
                                    buffer = Encoding.ASCII.GetBytes("This is the content for entry #1.");
                                    output.Write(buffer, 0, buffer.Length);
                                }

                                output.PutNextEntry("entry2/");  // this will be a directory
                                output.PutNextEntry("entry3.txt");
                                if (k == 0)
                                {
                                    buffer = Encoding.ASCII.GetBytes("This is the content for entry #3.");
                                    output.Write(buffer, 0, buffer.Length);
                                }
                                output.PutNextEntry("entry4.txt");  // this will be zero length
                                output.PutNextEntry("entry5.txt");  // this will be zero length
                            }
                        }

                        BasicVerifyZip(zipFileToCreate, password);

                        Assert.AreEqual<int>(4, TestUtilities.CountEntries(zipFileToCreate),
                                             "Trial ({0},{1}): The zip file created has the wrong number of entries.", i, j);
                    }
                }
            }
        }





        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void Create_ZipOutputStream_Directories_Write()
        {
            for (int k = 0; k < 2; k++)
            {
                string zipFileToCreate = String.Format("Create_ZipOutputStream_Directories.{0}.zip", k);

                using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var output = new ZipOutputStream(fs))
                    {
                        byte[] buffer;
                        output.Encryption = EncryptionAlgorithm.None;
                        output.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                        output.PutNextEntry("entry1/");
                        if (k == 0)
                        {
                            buffer = Encoding.ASCII.GetBytes("This is the content for entry #1.");
                            // this should fail
                            output.Write(buffer, 0, buffer.Length);
                        }

                        output.PutNextEntry("entry2/");  // this will be a directory
                        output.PutNextEntry("entry3.txt");
                        if (k == 0)
                        {
                            buffer = Encoding.ASCII.GetBytes("This is the content for entry #3.");
                            output.Write(buffer, 0, buffer.Length);
                        }
                        output.PutNextEntry("entry4.txt");  // this will be zero length
                        output.PutNextEntry("entry5.txt");  // this will be zero length
                    }
                }
            }
        }



        [TestMethod]
        public void Create_ZipOutputStream_EmptyEntries()
        {
            for (int i = 0; i < crypto.Length; i++)
            {
                for (int j = 0; j < compLevels.Length; j++)
                {
                    string password = Path.GetRandomFileName();

                    for (int k = 0; k < 2; k++)
                    {

                        string zipFileToCreate = String.Format("Create_ZipOutputStream_EmptyEntries.Encryption.{0}.{1}.{2}.zip",
                                                               crypto[i].ToString(), compLevels[j].ToString(), k);

                        using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
                        {
                            using (var output = new ZipOutputStream(fs))
                            {
                                byte[] buffer;
                                output.Password = password;
                                output.Encryption = crypto[i];
                                output.CompressionLevel = compLevels[j];
                                output.PutNextEntry("entry1.txt");
                                if (k == 0)
                                {
                                    buffer = Encoding.ASCII.GetBytes("This is the content for entry #1.");
                                    output.Write(buffer, 0, buffer.Length);
                                }

                                output.PutNextEntry("entry2.txt");  // this will be zero length
                                output.PutNextEntry("entry3.txt");
                                if (k == 0)
                                {
                                    buffer = Encoding.ASCII.GetBytes("This is the content for entry #3.");
                                    output.Write(buffer, 0, buffer.Length);
                                }
                                output.PutNextEntry("entry4.txt");  // this will be zero length
                                output.PutNextEntry("entry5.txt");  // this will be zero length
                            }
                        }

                        BasicVerifyZip(zipFileToCreate, password);

                        Assert.AreEqual<int>(5, TestUtilities.CountEntries(zipFileToCreate),
                                             "Trial ({0},{1}): The zip file created has the wrong number of entries.", i, j);
                    }
                }
            }
        }




        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Create_ZipOutputStream_DuplicateEntry()
        {
            string zipFileToCreate = "Create_ZipOutputStream_DuplicateEntry.zip";

            string entryName = Path.GetRandomFileName();

            using (FileStream fs = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var output = new ZipOutputStream(fs))
                {
                    output.PutNextEntry(entryName);
                    output.PutNextEntry(entryName);
                }
            }
        }



        [TestMethod]
        public void Create_ZipOutputStream()
        {
            bool seekable = true;
            bool zero = false;
            _TestDriver(new TestCompressionLevels(_Internal_Create_ZipOutputStream), "ZipOutputStream", seekable, zero);
        }

        [TestMethod]
        public void Create_ZipOutputStream_file()
        {
            bool seekable = true;
            bool zero = false;
            int fileOutputOption = 1;
            _TestDriver(new TestCompressionLevels(_Internal_Create_ZipOutputStream), "ZipOutputStream", seekable, zero, fileOutputOption);
        }

        [TestMethod]
        public void Create_ZipOutputStream_NonSeekable()
        {
            bool seekable = false;
            bool zero = false;
            _TestDriver(new TestCompressionLevels(_Internal_Create_ZipOutputStream), "ZipOutputStream", seekable, zero);
        }

        [TestMethod]
        public void Create_ZipOutputStream_ZeroLength_wi8933()
        {
            bool seekable = true;
            bool zero = true;
            _TestDriver(new TestCompressionLevels(_Internal_Create_ZipOutputStream), "ZipOutputStream", seekable, zero);
        }

        [TestMethod]
        public void Create_ZipOutputStream_ZeroLength_wi8933_file()
        {
            bool seekable = true;
            bool zero = true;
            int fileOutputOption = 1;
            _TestDriver(new TestCompressionLevels(_Internal_Create_ZipOutputStream), "ZipOutputStream", seekable, zero, fileOutputOption);
        }


        private void _Internal_Create_ZipOutputStream(string[] files,
                                                      EncryptionAlgorithm crypto,
                                                      bool seekable,
                                                      int cycle,
                                                      string format)
        {
            _Internal_Create_ZipOutputStream(files, crypto, seekable, cycle, format, 0);
        }


        private void _Internal_Create_ZipOutputStream(string[] files,
                                                      EncryptionAlgorithm crypto,
                                                      bool seekable,
                                                      int cycle,
                                                      string format,
                                                      int fileOutputOption)
        {
            int BufferSize = 2048;

            for (int k = 0; k < compLevels.Length; k++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format(format, k));
                string password = Path.GetRandomFileName();

                TestContext.WriteLine("=================================");
                TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFileToCreate));
                TestContext.WriteLine("Encryption({0})  Compression({1})  pw({2})",
                                      crypto.ToString(), compLevels[k].ToString(), password);

                using (ZipOutputStream output = GetZipOutputStream(seekable, fileOutputOption, zipFileToCreate))
                {
                    if (crypto != EncryptionAlgorithm.None)
                    {
                        output.Password = password;
                        output.Encryption = crypto;
                    }
                    output.CompressionLevel = compLevels[k];

                    byte[] buffer = new byte[BufferSize];
                    int n;
                    foreach (var file in files)
                    {
                        TestContext.WriteLine("file: {0}", file);

                        output.PutNextEntry(file);
                        using (var input = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                        {
                            while ((n = input.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, n);
                            }
                        }
                    }
                }

                BasicVerifyZip(zipFileToCreate, password);

                Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                     "Trial ({0},{1}): The zip file created has the wrong number of entries.", cycle, k);
            }
        }

        private static ZipOutputStream GetZipOutputStream(bool seekable, int fileOutputOption, string zipFileToCreate)
        {
            ZipOutputStream output = null;
            if (fileOutputOption == 0)
            {
                Stream raw = File.Open(zipFileToCreate, FileMode.Create, FileAccess.ReadWrite);

                // conditionally use a non-seekable output stream
                if (!seekable)
                    raw = new Ionic.Zip.Tests.NonSeekableOutputStream(raw);

                output = new ZipOutputStream(raw);
            }
            else
            {
                output = new ZipOutputStream(zipFileToCreate);
            }
            return output;
        }



        bool _pb2Set;
        bool _pb1Set;
        int _numSaving;
        int _totalToSave;

        private void streams_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            string msg;
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_Started:
                    //_txrx.Send("status saving started...");
                    _pb1Set = false;
                    _numSaving= 1;
                    break;

                case ZipProgressEventType.Saving_BeforeWriteEntry:
                    //_txrx.Send(String.Format("status Compressing {0}", e.CurrentEntry.FileName));
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

//                     _txrx.Send(String.Format("status Saving entry {0}/{1} :: {2} :: {3}/{4}mb {5:N0}%",
//                                              _numSaving, _totalToSave,
//                                              e.CurrentEntry.FileName,
//                                              e.BytesTransferred/(1024*1024), e.TotalBytesToTransfer/(1024*1024),
//                                              ((double)e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)));
                    msg = String.Format("pb 2 value {0}", e.BytesTransferred);
                    _txrx.Send(msg);
                    //System.Threading.Thread.Sleep(40);
                    break;

                case ZipProgressEventType.Saving_AfterWriteEntry:
                    _txrx.Send("pb 1 step");
                    _numSaving++;
                    break;

                case ZipProgressEventType.Saving_Completed:
                    //_txrx.Send("status Save completed");
                    _pb1Set = false;
                    _pb2Set = false;
                    _txrx.Send("pb 1 max 1");
                    _txrx.Send("pb 1 value 1");
                    break;
            }
        }


        [TestMethod, Timeout(1800000)]  // timeout is in milliseconds, (1800 * 1000) = 1/2 hour;
        public void ZipFile_Parallel_wi10030()
        {
            // Test memory growth over many many cycles.
            // There was a leak in the ParallelDeflateOutputStream, where
            // the PDOS was not being GC'd.  This test checks for that.
            //
            // If the error is present, this test will either timeout or
            // throw an InsufficientMemoryException (or whatever).  The
            // timeout occurs because GC begins to get verrrrry
            // sloooooow.  IF the error is not present, this test will
            // complete successfully, in about 20 minutes.
            //

            string zipFileToCreate = "ZipFile_Parallel_wi10030.zip";
            int nCycles = 4096;
            int nFiles = 3;
            int sizeBase = 384 * 1024;
            int sizeRange = 32 * 1024;
            int count = 0;
            int BufferSize = 1024;
            byte[] buffer = new byte[BufferSize];
            int n;

            // fill a couple memory streams with random text
            MemoryStream[] ms = new MemoryStream[nFiles];
            for (int i=0; i < ms.Length;  i++)
            {
                ms[i] = new MemoryStream();
                int sz = sizeBase + _rnd.Next(sizeRange);
                using (Stream rtg = new Ionic.Zip.Tests.Utilities.RandomTextInputStream(sz))
                {
                    while ((n = rtg.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms[i].Write(buffer, 0, n);
                    }
                }
            }
            buffer= null;

            OpenDelegate opener = (x) =>
                {
                    Stream s = ms[count%ms.Length];
                    s.Seek(0L,SeekOrigin.Begin);
                    count++;
                    return s;
                };

            CloseDelegate closer = (e,s) =>
                {
                    //s.Close();
                };

            string channel = "ZipFile_Parallel_wi10030";
            string txrxLabel = "PDOS Leak Test";
            StartProgressMonitor(channel);
            StartProgressClient(channel, txrxLabel, "starting up...");

            TestContext.WriteLine("Testing for leaks....");

            _txrx.Send(String.Format("pb 0 max {0}", nCycles));
            _txrx.Send("pb 0 value 0");

            for (int x=0; x < nCycles; x++)
            {
                if (x!=0 && x%16 == 0)
                    TestContext.WriteLine("Cycle {0}...", x);

                string status = String.Format("status Cycle {0}/{1}", x+1, nCycles);
                _txrx.Send(status);

                using (ZipFile zip = new ZipFile())
                {
                    zip.ParallelDeflateThreshold = 128 * 1024;
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    //zip.SaveProgress += streams_SaveProgress;
                    for (int y=0; y < nFiles;  y++)
                    {
                        zip.AddEntry("Entry" + y + ".txt", opener, closer);
                    }
                    zip.Comment = "Produced at " + System.DateTime.UtcNow.ToString("G");
                    zip.Save(zipFileToCreate);
                }

                _txrx.Send("pb 0 step");
            }

            for (int i=0; i < ms.Length;  i++)
            {
                ms[i].Close();
                ms[i].Dispose();
                ms[i]=null;
            }
            ms=null;
        }



        [TestMethod]
        public void ZipOutputStream_Parallel()
        {
            int _sizeBase      = 1024 * 1024;
            int _sizeRange     = 256 * 1024;
            //int _sizeBase      = 1024 * 256;
            //int _sizeRange     = 256 * 12;
            var sw             = new System.Diagnostics.Stopwatch();
            byte[] buffer      = new byte[0x8000];
            int n              = 0;
            TimeSpan[] ts      = new TimeSpan[2];
            int nFiles         = _rnd.Next(8) + 8;
            //int nFiles         = 2;
            string[] filenames = new string[nFiles];
            string dirToZip    = Path.Combine(TopLevelDir, "dirToZip");


            string channel = String.Format("ZOS_Parallel{0:000}",_rnd.Next(1000));
            string txrxLabel = "ZipOutputStream Parallel";
            StartProgressMonitor(channel);
            StartProgressClient(channel, txrxLabel, "starting up...");

            TestContext.WriteLine("Creating {0} fodder files...", nFiles);
            Directory.CreateDirectory(dirToZip);

            _txrx.Send(String.Format("pb 0 max {0}", nFiles));
            _txrx.Send("pb 0 value 0");

            sw.Start();

            for (int x=0; x < nFiles; x++)
            {
                string status = String.Format("status Creating file {0}/{1}", x+1, nFiles);
                _txrx.Send(status);

                filenames[x] = Path.Combine(dirToZip, String.Format("file{0:000}.txt", x));
                using (var output = File.Create(filenames[x]))
                {
                    using (Stream input = new Ionic.Zip.Tests.Utilities.RandomTextInputStream(_sizeBase + _rnd.Next(_sizeRange)))
                    {
                        while ((n = input.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            output.Write(buffer, 0, n);
                        }
                    }
                }
                _txrx.Send("pb 0 step");
            }
            sw.Stop();
            TestContext.WriteLine("file generation took {0}", sw.Elapsed);

            _txrx.Send(String.Format("pb 0 max {0}", crypto.Length));
            _txrx.Send("pb 0 value 0");

            for (int i = 0; i < crypto.Length; i++)
            {
                //int c = i;
                int c = (i + 2) % crypto.Length;

                _txrx.Send(String.Format("pb 1 max {0}", compLevels.Length));
                _txrx.Send("pb 1 value 0");

                for (int j = 0; j < compLevels.Length; j++)
                {
                    string password = Path.GetRandomFileName();

                    // I wanna do 2 cycles if there is compression, so I can compare MT
                    // vs 1T compression.  The first cycle will ALWAYS use the threaded
                    // compression, the 2nd will NEVER use it.  If
                    // CompressionLevel==None, then just do one cycle.
                    //
                    int kCycles = (compLevels[j] == Ionic.Zlib.CompressionLevel.None)
                        ? 1
                        : 2;

                    for (int k = 0; k < kCycles; k++)
                    {
                        // Also, I use Stopwatch to time the compression, and compare.
                        // In light of that, I wanna do one warmup, and then one timed
                        // trial (for t==0..2).  But here again, if CompressionLevel==None, then I
                        // don't want to do a timing comparison, so I don't need 2 trials.
                        // Therefore, in that case, the "warmup" is the only trial I want to do.
                        // So when k==1 and Compression==None, do no cycles at all.
                        //
                        int tCycles = (compLevels[j] == Ionic.Zlib.CompressionLevel.None)
                            ? ((k==0) ? 1 : 0)
                            : 2;

                        if (k==0)
                        {
                        _txrx.Send(String.Format("pb 2 max {0}", kCycles*tCycles));
                        _txrx.Send("pb 2 value 0");
                        }

                        for (int t=0; t < tCycles;  t++)
                        {
                            TestContext.WriteLine(new String('-', 72));
                            string zipFileToCreate = String.Format("ZipOutputStream_Parallel.E-{0}.C-{1}.{2}.{3}timed.zip",
                                                                   crypto[c].ToString(), compLevels[j].ToString(),
                                                                   (compLevels[j] == Ionic.Zlib.CompressionLevel.None)
                                                                   ? "NA"
                                                                   : (k==0)?"1T":"MT",
                                                                   (t==0)?"not-":"");

                            TestContext.WriteLine("Trial {0}.{1}.{2}.{3}", i,j,k,t);
                            TestContext.WriteLine("Create zip file {0}", zipFileToCreate);

                            _txrx.Send("status " + zipFileToCreate);

                            sw.Reset();
                            sw.Start();
                            using (var output = new ZipOutputStream(zipFileToCreate))
                            {
                                if (k==0)
                                    output.ParallelDeflateThreshold = -1L;   // never
                                else
                                    output.ParallelDeflateThreshold = 0L; // always

                                output.Password = password;
                                output.Encryption = crypto[c]; // maybe "None"
                                output.CompressionLevel = compLevels[j];

                                _txrx.Send(String.Format("pb 3 max {0}", nFiles));
                                _txrx.Send("pb 3 value 0");

                                for (int x=0; x < nFiles; x++)
                                {
                                    output.PutNextEntry(Path.GetFileName(filenames[x]));
                                    using (var input = File.Open(filenames[x], FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                                    {
                                        while ((n = input.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            output.Write(buffer, 0, n);
                                        }
                                    }
                                    _txrx.Send("pb 3 step");
                                }
                            }

                            sw.Stop();
                            ts[k]= sw.Elapsed;
                            TestContext.WriteLine("compression took {0}", ts[k]);

                            //if (t==0)
                            BasicVerifyZip(zipFileToCreate, password);

                            Assert.AreEqual<int>(nFiles, TestUtilities.CountEntries(zipFileToCreate),
                                                 "Trial ({0}.{1}.{2}.{3}): The zip file created has the wrong number of entries.", i, j, k, t);

                            _txrx.Send("pb 2 step");
                        }

                    }

#if NOT_DEBUGGING
                    // parallel is not always faster!
                    if (_sizeBase > 256 * 1024 &&
                        compLevels[j] != Ionic.Zlib.CompressionLevel.None &&
                        compLevels[j] != Ionic.Zlib.CompressionLevel.BestSpeed &&
                        crypto[c] != EncryptionAlgorithm.WinZipAes256  &&
                        crypto[c] != EncryptionAlgorithm.WinZipAes128 )
                        Assert.IsTrue(ts[0]>ts[1], "Whoops! Cycle {0}.{1} (crypto({4}) Comp({5})): Parallel deflate is slower ({2}<{3})",
                                      i, j, ts[0], ts[1],
                                      crypto[c],
                                      compLevels[j]);
#endif
                    _txrx.Send("pb 1 step");
                }
                _txrx.Send("pb 0 step");
            }

            _txrx.Send("stop");
        }





        [TestMethod]
        public void Streams_7z_Zip_ZeroLength()
        {
            _Internal_Streams_7z_Zip(0, "zero");
        }

        [TestMethod]
        public void Streams_7z_Zip()
        {
            _Internal_Streams_7z_Zip(1, "nonzero");
        }

        [TestMethod]
        public void Streams_7z_Zip_Mixed()
        {
            _Internal_Streams_7z_Zip(2, "mixed");
        }

        [TestMethod]
        public void Streams_Winzip_Zip_Mixed_Password()
        {
            string password = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _Internal_Streams_WinZip_Zip(2, password, "mixed");
        }

        [TestMethod]
        public void Streams_Winzip_Zip()
        {
            _Internal_Streams_WinZip_Zip(1, null, "nonzero");
        }

        private string CreateZeroLengthFile(int ix, string directory)
        {
            string nameOfFileToCreate = Path.Combine(directory, String.Format("ZeroLength{0:D4}.txt", ix));
            using (var fs = File.Create(nameOfFileToCreate)) { }
            return nameOfFileToCreate;
        }


        public void _Internal_Streams_7z_Zip(int flavor, string label)
        {
            if (!SevenZipIsPresent)
            {
                TestContext.WriteLine("skipping test [_Internal_Streams_7z_Zip] : SevenZip is not present");
                return;
            }

            int[] fileCounts = { 1, 2, _rnd.Next(8) + 6, _rnd.Next(18) + 16, _rnd.Next(48) + 56 } ;

            for (int m=0; m < fileCounts.Length; m++)
            {
                string dirToZip = String.Format("trial{0:D2}",m);
                if (!Directory.Exists(dirToZip)) Directory.CreateDirectory(dirToZip);

                int fileCount = fileCounts[m];
                string zipFileToCreate = Path.Combine(TopLevelDir,
                                                      String.Format("Streams_7z_Zip.{0}.{1}.{2}.zip", flavor, label, m));

                string[] files = null;
                if (flavor == 0)
                {
                    // zero length files
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                        files[i] = CreateZeroLengthFile(i,dirToZip);
                }
                else if (flavor == 1)
                    files = TestUtilities.GenerateFilesFlat(dirToZip, fileCount, 100, 72000);
                else
                {
                    // mixed
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                    {
                        if (_rnd.Next(3) == 0)
                            files[i] = CreateZeroLengthFile(i,dirToZip);
                        else
                        {
                            files[i] = Path.Combine(dirToZip, String.Format("nonzero{0:D4}.txt", i));
                            TestUtilities.CreateAndFillFileText(files[i], _rnd.Next(60000)+100);
                        }
                    }
                }

                // Create the zip archive via 7z.exe
                this.Exec(sevenZip, String.Format("a {0} {1}", zipFileToCreate, dirToZip));

                // Verify the number of files in the zip
                Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), files.Length,
                                     "Incorrect number of entries in the zip file.");

                // extract the files
                string extractDir = String.Format("extract{0:D2}",m);
                byte[] buffer= new byte[2048];
                int n;
                using (var raw = File.Open(zipFileToCreate, FileMode.Open, FileAccess.Read ))
                {
                    using (var input= new ZipInputStream(raw))
                    {
                        ZipEntry e;
                        while (( e = input.GetNextEntry()) != null)
                        {
                            TestContext.WriteLine("entry: {0}", e.FileName);

                            string outputPath = Path.Combine(extractDir, e.FileName);

                            if (e.IsDirectory)
                            {
                                // create the directory
                                Directory.CreateDirectory(outputPath);
                            }
                            else
                            {
                                // create the file
                                using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite ))
                                {
                                    while ((n= input.Read(buffer,0,buffer.Length)) > 0)
                                    {
                                        output.Write(buffer,0,n);
                                    }
                                }
                            }

                            // we don't set the timestamps or attributes on the file/directory.
                        }
                    }
                }

                // winzip does not include the base path in the filename;
                // 7zip does.
                string[] filesUnzipped = Directory.GetFiles(Path.Combine(extractDir, dirToZip));

                // Verify the number of files extracted
                Assert.AreEqual<int>(files.Length, filesUnzipped.Length,
                                     "Incorrect number of files extracted.");
            }
        }


        public void _Internal_Streams_WinZip_Zip(int fodderOption, string password, string label)
        {
            if (!WinZipIsPresent)
            {
                TestContext.WriteLine("skipping test [_Internal_Streams_WinZip_Zip] : winzip is not present");
                return;
            }

            int[] fileCounts = { 1, 2, _rnd.Next(8) + 6, _rnd.Next(18) + 16, _rnd.Next(48) + 56 } ;

            for (int m=0; m < fileCounts.Length; m++)
            {
                string dirToZip = String.Format("trial{0:D2}",m);
                if (!Directory.Exists(dirToZip)) Directory.CreateDirectory(dirToZip);

                int fileCount = fileCounts[m];
                string zipFileToCreate = Path.Combine(TopLevelDir,
                                                      String.Format("Streams_Winzip_Zip.{0}.{1}.{2}.zip", fodderOption, label, m));

                string[] files = null;
                if (fodderOption == 0)
                {
                    // zero length files
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                        files[i] = CreateZeroLengthFile(i,dirToZip);
                }
                else if (fodderOption == 1)
                    files = TestUtilities.GenerateFilesFlat(dirToZip, fileCount, 100, 72000);
                else
                {
                    // mixed
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                    {
                        if (_rnd.Next(3) == 0)
                            files[i] = CreateZeroLengthFile(i,dirToZip);
                        else
                        {
                            files[i] = Path.Combine(dirToZip, String.Format("nonzero{0:D4}.txt", i));
                            TestUtilities.CreateAndFillFileText(files[i], _rnd.Next(60000)+100);
                        }
                    }
                }

                // Create the zip archive via WinZip.exe
                string pwdOption = String.IsNullOrEmpty(password) ? "" : "-s"+password;
                string formatString = "-a -p {0} -yx {1} {2}\\*.*";
                string wzzipOut = this.Exec(wzzip, String.Format(formatString, pwdOption, zipFileToCreate, dirToZip));

                // Verify the number of files in the zip
                Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), files.Length,
                                     "Incorrect number of entries in the zip file.");

                // extract the files
                string extractDir = String.Format("extract{0:D2}",m);
                Directory.CreateDirectory(extractDir);
                byte[] buffer= new byte[2048];
                int n;

                using (var raw = File.Open(zipFileToCreate, FileMode.Open, FileAccess.Read ))
                {
                    using (var input= new ZipInputStream(raw))
                    {
                        input.Password = password;
                        ZipEntry e;
                        while (( e = input.GetNextEntry()) != null)
                        {
                            TestContext.WriteLine("entry: {0}", e.FileName);

                            string outputPath = Path.Combine(extractDir, e.FileName);

                            if (e.IsDirectory)
                            {
                                // create the directory
                                Directory.CreateDirectory(outputPath);
                            }
                            else
                            {
                                // create the file
                                using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite ))
                                {
                                    while ((n= input.Read(buffer,0,buffer.Length)) > 0)
                                    {
                                        output.Write(buffer,0,n);
                                    }
                                }
                            }

                            // we don't set the timestamps or attributes on the file/directory.
                        }
                    }
                }

                string[] filesUnzipped = Directory.GetFiles(extractDir);

                // Verify the number of files extracted
                Assert.AreEqual<int>(files.Length, filesUnzipped.Length,
                                     "Incorrect number of files extracted.");
            }
        }




        [TestMethod]
        public void Streams_ZipInput_Encryption_zero()
        {
            _Internal_Streams_ZipInput_Encryption(0);
        }

        [TestMethod]
        public void Streams_ZipInput_Encryption_zero_subdir()
        {
            _Internal_Streams_ZipInput_Encryption(3);
        }

        [TestMethod]
        public void Streams_ZipInput_Encryption_nonzero()
        {
            _Internal_Streams_ZipInput_Encryption(1);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_nonzero_subdir()
        {
            _Internal_Streams_ZipInput_Encryption(4);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_mixed()
        {
            _Internal_Streams_ZipInput_Encryption(2);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_mixed_subdir()
        {
            _Internal_Streams_ZipInput_Encryption(5);
        }





        [TestMethod]
        public void Streams_ZipInput_Encryption_zero_file()
        {
            _Internal_Streams_ZipInput_Encryption(0,1);
        }

        [TestMethod]
        public void Streams_ZipInput_Encryption_zero_subdir_file()
        {
            _Internal_Streams_ZipInput_Encryption(3,1);
        }

        [TestMethod]
        public void Streams_ZipInput_Encryption_nonzero_file()
        {
            _Internal_Streams_ZipInput_Encryption(1,1);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_nonzero_subdir_file()
        {
            _Internal_Streams_ZipInput_Encryption(4,1);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_mixed_file()
        {
            _Internal_Streams_ZipInput_Encryption(2,1);
        }


        [TestMethod]
        public void Streams_ZipInput_Encryption_mixed_subdir_file()
        {
            _Internal_Streams_ZipInput_Encryption(5,1);
        }





        public void _Internal_Streams_ZipInput_Encryption(int fodderOption)
        {
            _Internal_Streams_ZipInput_Encryption(fodderOption, 0);
        }

        public void _Internal_Streams_ZipInput_Encryption(int fodderOption, int fileReadOption)
        {
            int[] fileCounts = { 1, 2, _rnd.Next(8) + 6, _rnd.Next(18) + 16, _rnd.Next(48) + 56 } ;

            for (int m=0; m < fileCounts.Length; m++)
            {
                string password = Path.GetRandomFileName();
                string dirToZip = String.Format("trial{0:D2}",m);
                if (!Directory.Exists(dirToZip)) Directory.CreateDirectory(dirToZip);

                int fileCount = fileCounts[m];

                string[] files = null;
                if (fodderOption == 0)
                {
                    // zero length files
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                        files[i] = CreateZeroLengthFile(i,dirToZip);
                }
                else if (fodderOption == 1)
                    files = TestUtilities.GenerateFilesFlat(dirToZip, fileCount, 100, 72000);
                else
                {
                    // mixed = some zero and some not
                    files = new string[fileCount];
                    for (int i = 0; i < fileCount; i++)
                    {
                        if (_rnd.Next(3) == 0)
                            files[i] = CreateZeroLengthFile(i,dirToZip);
                        else
                        {
                            files[i] = Path.Combine(dirToZip, String.Format("nonzero{0:D4}.txt", i));
                            TestUtilities.CreateAndFillFileText(files[i], _rnd.Next(60000)+100);
                        }
                    }
                }

                for (int i = 0; i < crypto.Length; i++)
                {
                    EncryptionAlgorithm c = crypto[i];

                    string zipFileToCreate = Path.Combine(TopLevelDir,
                                                          String.Format("Streams_ZipInput_AES.{0}.count.{1:D2}.{2}.zip",
                                                                        c.ToString(), fileCounts[m], fodderOption));
                    // Create the zip archive
                    using (var zip = new ZipFile())
                    {
                        zip.Password = password;
                        zip.Encryption = c;
                        if (fodderOption > 2)
                        {
                            zip.AddDirectoryByName("subdir");
                            zip.AddDirectory(dirToZip, "subdir");
                        }
                        else
                            zip.AddDirectory(dirToZip);

                        zip.Save(zipFileToCreate);
                    }


                    // Verify the number of files in the zip
                    Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), files.Length,
                                         "Incorrect number of entries in the zip file.");

                    // extract the files
                    string extractDir = String.Format("extract{0:D2}.{1:D2}", m, i);
                    Directory.CreateDirectory(extractDir);

                    byte[] buffer= new byte[2048];
                    int n;

                    ZipInputStream input = null;
                    if (fileReadOption == 0)
                    {
                        var raw = File.Open(zipFileToCreate, FileMode.Open, FileAccess.Read );
                        input= new ZipInputStream(raw);
                    }
                    else
                    {
                        input= new ZipInputStream(zipFileToCreate);
                    }

                    using (input)
                    {
                        // set password if necessary
                        if (crypto[i] != EncryptionAlgorithm.None)
                            input.Password = password;

                        ZipEntry e;
                        while (( e = input.GetNextEntry()) != null)
                        {
                            TestContext.WriteLine("entry: {0}", e.FileName);

                            string outputPath = Path.Combine(extractDir, e.FileName);

                            if (e.IsDirectory)
                            {
                                // create the directory
                                Directory.CreateDirectory(outputPath);
                            }
                            else
                            {
                                // emit the file
                                using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite ))
                                {
                                    while ((n= input.Read(buffer,0,buffer.Length)) > 0)
                                    {
                                        output.Write(buffer,0,n);
                                    }
                                }
                            }
                        }

                    }

                    string[] filesUnzipped = (fodderOption > 2)
                        ? Directory.GetFiles(Path.Combine(extractDir, "subdir" ))
                        : Directory.GetFiles(extractDir);

                    // Verify the number of files extracted
                    Assert.AreEqual<int>(files.Length, filesUnzipped.Length,
                                         "Incorrect number of files extracted. ({0}!={1})", files.Length, filesUnzipped.Length);
                }
            }
        }



    }
}
