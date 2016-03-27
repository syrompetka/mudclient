// ExtendedTests.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2008-2010 Dino Chiesa.
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
// Time-stamp: <2010-February-24 22:55:52>
//
// ------------------------------------------------------------------
//
// This module defines some extended tests for DotNetZip.  It gets into
// advanced features - file selection, encryption, and more.
//
// ------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;


namespace Ionic.Zip.Tests.Extended
{

    public class XTWFND : System.Xml.XmlTextWriter
    {
        public XTWFND(TextWriter w) : base(w) { Formatting = System.Xml.Formatting.Indented; }
        public override void WriteStartDocument() { }
    }

    /// <summary>
    /// Summary description for ExtendedTests
    /// </summary>
    [TestClass]
    public class ExtendedTests : IonicTestClass
    {
        public ExtendedTests() : base() { }


        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanupEx()
        {
            if (_txrx != null)
            {
                _txrx.Send("stop");
                _txrx = null;
            }
        }


        private System.Reflection.Assembly _myself;
        private System.Reflection.Assembly myself
        {
            get
            {
                if (_myself == null)
                {
                    _myself = System.Reflection.Assembly.GetExecutingAssembly();
                }
                return _myself;
            }
        }


        static String StreamToStringUTF8(Stream s)
        {
            string result = null;
            // UTF-8 is the default, but I want to be explicit here.
            using (var f = new StreamReader(s, System.Text.Encoding.UTF8))
            {
                result = f.ReadToEnd();
            }
            return result;
        }


        static bool IsEncodable(String s, Encoding e)
        {
            bool result = false;
            try
            {
                byte[] b = e.GetBytes(s);
                var s2 = e.GetString(b);
                result = (s == s2);
            }
            catch
            {
                result = false;
            }
            return result;
        }




        EncryptionAlgorithm[] crypto =
            {
                EncryptionAlgorithm.None,
                EncryptionAlgorithm.PkzipWeak,
                EncryptionAlgorithm.WinZipAes128,
                EncryptionAlgorithm.WinZipAes256,
            };

        EncryptionAlgorithm[] cryptoNoPkzip =
            {
                EncryptionAlgorithm.None,
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



        [TestMethod]
        public void TestZip_IsZipFile()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "TestZip_IsZipFile.zip");

            int entriesAdded = 0;
            String filename = null;

            string subdir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(subdir);

            int fileCount = _rnd.Next(10) + 10;
            for (int j = 0; j < fileCount; j++)
            {
                filename = Path.Combine(subdir, String.Format("FileToBeAdded-{0:D2}.txt", j));
                TestUtilities.CreateAndFillFileText(filename, _rnd.Next(34000) + 5000);
                entriesAdded++;
            }

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(subdir, Path.GetFileName(subdir));
                zip1.Save(zipFileToCreate);
            }

            // Verify the files are in the zip
            Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entriesAdded,
                                 "The Zip file has the wrong number of entries.");

            Assert.IsTrue(ZipFile.IsZipFile(zipFileToCreate),
                          "The IsZipFile() method returned an unexpected result for an existing zip file.");

            Assert.IsTrue(ZipFile.IsZipFile(zipFileToCreate, true),
                          "The IsZipFile() method returned an unexpected result for an existing zip file.");

            Assert.IsTrue(!ZipFile.IsZipFile(filename),
                          "The IsZipFile() method returned an unexpected result for a extant file that is not a zip.");

            filename = Path.Combine(subdir, String.Format("ThisFileDoesNotExist.{0:D2}.txt", _rnd.Next(2000)));
            Assert.IsTrue(!ZipFile.IsZipFile(filename),
                          "The IsZipFile() method returned an unexpected result for a non-existent file.");

        }


        [TestMethod]
        public void TestZip_IsZipFile_Stream()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "TestZip_IsZipFile-Stream.zip");

            int entriesAdded = 0;
            String filename = null;

            string Subdir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(Subdir);

            int fileCount = _rnd.Next(10) + 10;
            for (int j = 0; j < fileCount; j++)
            {
                filename = Path.Combine(Subdir, String.Format("FileToBeAdded-{0:D2}.txt", j));
                TestUtilities.CreateAndFillFileText(filename, _rnd.Next(34000) + 5000);
                entriesAdded++;
            }

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(Subdir, Path.GetFileName(Subdir));
                zip1.Save(zipFileToCreate);
            }

            // Verify the files are in the zip
            Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entriesAdded,
                                 "The Zip file has the wrong number of entries.");

            using (FileStream input = File.OpenRead(zipFileToCreate))
            {
                Assert.IsTrue(ZipFile.IsZipFile(input, false),
                              "The IsZipFile() method returned an unexpected result for an existing zip file.");
            }

            using (FileStream input = File.OpenRead(zipFileToCreate))
            {
                Assert.IsTrue(ZipFile.IsZipFile(input, true),
                              "The IsZipFile() method returned an unexpected result for an existing zip file.");
            }
        }






        [TestMethod]
        public void ReadZip_DirectoryBitSetForEmptyDirectories()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "ReadZip_DirectoryBitSetForEmptyDirectories.zip");

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectoryByName("Directory1");
                // must retrieve with a trailing slash.
                ZipEntry e1 = zip1["Directory1/"];
                Assert.AreNotEqual<ZipEntry>(null, e1);
                Assert.IsTrue(e1.IsDirectory,
                              "The IsDirectory property was not set as expected.");
                zip1.AddDirectoryByName("Directory2");
                zip1.AddEntry(Path.Combine("Directory2", "Readme.txt"), "This is the content");
                Assert.IsTrue(zip1["Directory2/"].IsDirectory,
                              "The IsDirectory property was not set as expected.");
                zip1.Save(zipFileToCreate);
                Assert.IsTrue(zip1["Directory1/"].IsDirectory,
                              "The IsDirectory property was not set as expected.");
            }

            // read the zip and retrieve the dir entries again
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                Assert.IsTrue(zip2["Directory1/"].IsDirectory,
                              "The IsDirectory property was not set as expected.");

                Assert.IsTrue(zip2["Directory2/"].IsDirectory,
                              "The IsDirectory property was not set as expected.");
            }

            // now specify dir names with backslash
            using (ZipFile zip3 = ZipFile.Read(zipFileToCreate))
            {
                Assert.IsTrue(zip3["Directory1\\"].IsDirectory,
                              "The IsDirectory property was not set as expected.");

                Assert.IsTrue(zip3["Directory2\\"].IsDirectory,
                              "The IsDirectory property was not set as expected.");

                Assert.IsNull(zip3["Directory1"]);
                Assert.IsNull(zip3["Directory2"]);
            }

        }


        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Create_DuplicateEntries_wi8047()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_DuplicateEntries_wi8047.zip");
            string filename = "file.test";
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);

            using (var zip = new ZipFile())
            {
                int n = _rnd.Next(files.Length);
                zip.UpdateFile(files[n]).FileName = filename;
                int n2 = 0;
                while ((n2 = _rnd.Next(files.Length)) == n) ;
                zip.UpdateFile(files[n2]).FileName = filename;
                zip.Save(zipFileToCreate);
            }
        }


        [TestMethod]
        public void Create_RenameRemoveAndRenameAgain_wi8047()
        {
            string filename = "file.test";
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);

            for (int m = 0; m < 2; m++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_RenameRemoveAndRenameAgain_wi8047-{0}.zip", m));

                using (var zip = new ZipFile())
                {
                    // select a single file from the list
                    int n = _rnd.Next(files.Length);

                    // insert the selected file into the zip, and also rename it
                    zip.UpdateFile(files[n]).FileName = filename;

                    // conditionally save
                    if (m > 0) zip.Save(zipFileToCreate);

                    // remove the original file
                    zip.RemoveEntry(zip[filename]);

                    // select another file from the list, making sure it is not the same file
                    int n2 = 0;
                    while ((n2 = _rnd.Next(files.Length)) == n) ;

                    // insert that other file and rename it
                    zip.UpdateFile(files[n2]).FileName = filename;
                    zip.Save(zipFileToCreate);
                }

                Assert.AreEqual<int>(1, TestUtilities.CountEntries(zipFileToCreate), "Trial {0}: The Zip file has the wrong number of entries.", m);
            }
        }


        [TestMethod]
        public void Create_EmitTimestampOptions()
        {
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);

            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_EmitTimestampOptions-{0}-{1}.zip", j, k));
                    using (var zip = new ZipFile())
                    {
                        if (j == 1) zip.EmitTimesInUnixFormatWhenSaving = false;
                        else if (j == 2) zip.EmitTimesInUnixFormatWhenSaving = true;

                        if (k == 1) zip.EmitTimesInWindowsFormatWhenSaving = false;
                        else if (k == 2) zip.EmitTimesInWindowsFormatWhenSaving = true;

                        zip.AddFiles(files, "files");
                        zip.Save(zipFileToCreate);
                    }

                    Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate), "The Zip file has the wrong number of entries.");

                    using (var zip = ZipFile.Read(zipFileToCreate))
                    {
                        for (int i = 0; i < zip.Entries.Count; i++)
                        {
                            if (j == 2)
                                Assert.AreEqual<ZipEntryTimestamp>(ZipEntryTimestamp.Unix, zip[i].Timestamp & ZipEntryTimestamp.Unix,
                                    "Missing Unix timestamp (cycle {0},{1}) (entry {2}).", j, k, i);
                            else
                                Assert.AreEqual<ZipEntryTimestamp>(ZipEntryTimestamp.None, zip[i].Timestamp & ZipEntryTimestamp.Unix,
                                    "Unix timestamp is present when none is expected (cycle {0},{1}) (entry {2}).", j, k, i);

                            if (k == 1)
                                Assert.AreEqual<ZipEntryTimestamp>(ZipEntryTimestamp.None, zip[i].Timestamp & ZipEntryTimestamp.Windows,
                                    "Windows timestamp is present when none is expected (cycle {0},{1}) (entry {2}).", j, k, i);
                            else
                                Assert.AreEqual<ZipEntryTimestamp>(ZipEntryTimestamp.Windows, zip[i].Timestamp & ZipEntryTimestamp.Windows,
                                    "Missing Windows timestamp (cycle {0},{1}) (entry {2}).", j, k, i);

                            Assert.AreEqual<ZipEntryTimestamp>(ZipEntryTimestamp.DOS, zip[i].Timestamp & ZipEntryTimestamp.DOS,
                                "Missing DOS timestamp (entry (cycle {0},{1}) (entry {2}).", j, k, i);
                        }
                    }
                }
            }
        }



        [TestMethod]
        public void Extract_AfterSaveNoDispose()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Extract_AfterSaveNoDispose.zip");
            string InputString = "<AAA><bob><YourUncle/></bob><w00t/></AAA>";

            using (ZipFile zip1 = new ZipFile())
            {
                MemoryStream ms1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(InputString));
                zip1.AddEntry("woo\\Test.xml", ms1);
                zip1.Save(zipFileToCreate);

                MemoryStream ms2 = new MemoryStream();
                zip1["Woo/Test.xml"].Extract(ms2);
                ms2.Seek(0, SeekOrigin.Begin);

                var sw1 = new StringWriter();
                var w1 = new XTWFND(sw1);

                var d1 = new System.Xml.XmlDocument();
                d1.Load(ms2);
                d1.Save(w1);

                var sw2 = new StringWriter();
                var w2 = new XTWFND(sw2);
                var d2 = new System.Xml.XmlDocument();
                d2.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(InputString)));
                d2.Save(w2);

                Assert.AreEqual<String>(sw2.ToString(), sw1.ToString(), "Unexpected value on extract ({0}).", sw1.ToString());
            }
        }



        [TestMethod]
        public void Test_AddUpdateFileFromStream()
        {
            string[] Passwords = { null, "Password", TestUtilities.GenerateRandomPassword(), "A" };
            for (int k = 0; k < Passwords.Length; k++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Test_AddUpdateFileFromStream-{0}.zip", k));
                string[] InputStrings = new string[]
                        {
                            TestUtilities.LoremIpsum.Substring(_rnd.Next(5), 170 + _rnd.Next(25)),
                            TestUtilities.LoremIpsum.Substring(100 + _rnd.Next(40), 180+ _rnd.Next(30))
                        };

                // add entries to a zipfile.
                // use a password.(possibly null)
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.Password = Passwords[k];
                    for (int i = 0; i < InputStrings.Length; i++)
                    {
                        zip1.AddEntry(String.Format("Lorem{0}.txt", i + 1), InputStrings[i]);
                    }
                    zip1.Save(zipFileToCreate);
                }

                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    zip2["Lorem2.txt"].Password = Passwords[k];
                    string output = StreamToStringUTF8(zip2["Lorem2.txt"].OpenReader());

                    Assert.AreEqual<String>(output, InputStrings[1], "Trial {0}: Read entry 2 after create: Unexpected value on extract.", k);

                    zip2["Lorem1.txt"].Password = Passwords[k];
                    Stream s = zip2["Lorem1.txt"].OpenReader();
                    output = StreamToStringUTF8(s);

                    Assert.AreEqual<String>(output, InputStrings[0], "Trial {0}: Read entry 1 after create: Unexpected value on extract.", k);
                }


                // update an entry in the zipfile.  For this pass, don't use a password.
                string UpdateString = "This is the updated content.  It will replace the original content, added from a string.";
                using (ZipFile zip3 = ZipFile.Read(zipFileToCreate))
                {
                    //zip2.Password = password;
                    var ms1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(UpdateString));
                    zip3.UpdateEntry("Lorem1.txt", ms1);
                    zip3.Save();
                }

                using (ZipFile zip4 = ZipFile.Read(zipFileToCreate))
                {
                    string output = StreamToStringUTF8(zip4["Lorem1.txt"].OpenReader());
                    Assert.AreEqual<String>(output, UpdateString, "Trial {0}: Reading after update: Unexpected value on extract.", k);
                }
            }
        }



        [TestMethod]
        public void Test_AddEntry_String()
        {
            string[] Passwords = { null, "Password", TestUtilities.GenerateRandomPassword(), "A" };

            Encoding[] encodings = { Encoding.UTF8,
                                     Encoding.Default,
                                     Encoding.ASCII,
                                     Encoding.GetEncoding("Big5"),
                                     Encoding.GetEncoding("iso-8859-1"),
                                     Encoding.GetEncoding("Windows-1252"),
                };

            string testBin = TestUtilities.GetTestBinDir(CurrentDir);
            string testStringsFile = Path.Combine(testBin, "Resources\\TestStrings.txt");
            var contentStrings = File.ReadAllLines(testStringsFile);

            int[] successfulEncodings = new int[contentStrings.Length];

            for (int a = 0; a < crypto.Length; a++)
            {
                for (int b = 0; b < Passwords.Length; b++)
                {
                    for (int c = 0; c < encodings.Length; c++)
                    {
                        string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Test_AddEntry_String-{0}.{1}.{2}.zip", a, b, c));
                        Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);


                        // add entries to a zipfile.
                        // use a password.(possibly null)
                        using (ZipFile zip1 = new ZipFile(zipFileToCreate))
                        {
                            zip1.Comment = String.Format("Test zip file.\nEncryption({0}) Pw({1}) fileEncoding({2})",
                                                        crypto[a].ToString(),
                                                        Passwords[b],
                                                        encodings[c].ToString());
                            zip1.Encryption = crypto[a];
                            zip1.Password = Passwords[b];
                            for (int d = 0; d < contentStrings.Length; d++)
                            {
                                string entryName = String.Format("File{0}.txt", d + 1);
                                // add each string using the given encoding
                                zip1.AddEntry(entryName, contentStrings[d], encodings[c]);
                            }
                            zip1.Save();
                        }

                        // Verify the number of files in the zip
                        Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), contentStrings.Length,
                                             "Incorrect number of entries in the zip file.");



                        using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                        {
                            zip2.Password = Passwords[b];
                            for (int d = 0; d < contentStrings.Length; d++)
                            {
                                try
                                {
                                    string entryName = String.Format("File{0}.txt", d + 1);
                                    //zip2[entryName].Password = Passwords[b];  // should not be necessary
                                    using (Stream s = zip2[entryName].OpenReader())
                                    {
                                        using (var sr = new StreamReader(s, encodings[c]))
                                        {
                                            try
                                            {
                                                Assert.AreNotEqual<StreamReader>(null, sr);
                                                string retrievedContent = sr.ReadLine();
                                                if (IsEncodable(contentStrings[d], encodings[c]))
                                                {
                                                    Assert.AreEqual<String>(contentStrings[d], retrievedContent,
                                                                            "encryption({0}) pw({1}) encoding({2}), contentString({3}) file({4}): the content did not match.",
                                                                            a, b, c, d, entryName);
                                                    successfulEncodings[d]++;
                                                }
                                                else
                                                {
                                                    Assert.AreNotEqual<Encoding>(Encoding.UTF8, encodings[c]);
                                                    Assert.AreNotEqual<String>(contentStrings[d], retrievedContent,
                                                                               "encryption({0}) pw({1}) encoding({2}), contentString({3}) file({4}): the content should not match, but does.",
                                                                               a, b, c, d, entryName);
                                                }
                                            }
                                            catch (Exception exc1)
                                            {
                                                TestContext.WriteLine("Exception while reading: a({0}) b({1}) c({2}) d({3})",
                                                                      a, b, c, d);
                                                throw new Exception("broken", exc1);
                                            }
                                        }
                                    }

                                }
                                catch (Exception e1)
                                {
                                    TestContext.WriteLine("Exception in OpenReader: Encryption({0}) pw({1}) c({2}) d({3})",
                                                          crypto[a].ToString(), Passwords[b], encodings[c].ToString(), d);

                                    throw new Exception("broken", e1);
                                }
                            }
                        }
                    }
                }
            }

            for (int d = 0; d < successfulEncodings.Length; d++)
                Assert.AreNotEqual<Int32>(0, successfulEncodings[d], "Content item #{0} ({1}) was never encoded successfully.", d, contentStrings[d]);

        }



        [TestMethod]
        public void Test_AddDirectoryByName()
        {
            for (int n = 1; n <= 10; n++)
            {
                var dirsAdded = new System.Collections.Generic.List<String>();
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Test_AddDirectoryByName{0:N2}.zip", n));
                using (ZipFile zip1 = new ZipFile())
                {
                    for (int i = 0; i < n; i++)
                    {
                        // create an arbitrary directory name, add it to the zip archive
                        string dirName = TestUtilities.GenerateRandomName(24);
                        zip1.AddDirectoryByName(dirName);
                        dirsAdded.Add(dirName + "/");
                    }
                    zip1.Save(zipFileToCreate);
                }


                int dirCount = 0;
                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    foreach (var e in zip2)
                    {
                        TestContext.WriteLine("dir: {0}", e.FileName);
                        Assert.IsTrue(dirsAdded.Contains(e.FileName), "Cannot find the expected entry");
                        Assert.IsTrue(e.IsDirectory);
                        dirCount++;
                    }
                }
                Assert.AreEqual<int>(n, dirCount);
            }
        }



        [TestMethod]
        public void Test_AddDirectoryByName_Nested()
        {
            Directory.SetCurrentDirectory(TopLevelDir);

            var DirsAdded = new System.Collections.Generic.List<String>();
            string zipFileToCreate = Path.Combine(TopLevelDir, "Test_AddDirectoryByName_Nested.zip");
            using (ZipFile zip1 = new ZipFile(zipFileToCreate))
            {
                for (int n = 1; n <= 14; n++)
                {
                    string DirName = n.ToString();
                    for (int i = 0; i < n; i++)
                    {
                        // create an arbitrary directory name, add it to the zip archive
                        DirName = Path.Combine(DirName, TestUtilities.GenerateRandomAsciiString(11));
                    }
                    zip1.AddDirectoryByName(DirName);
                    DirsAdded.Add(DirName.Replace("\\", "/") + "/");
                }
                zip1.Save();
            }

            int dirCount = 0;
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                foreach (var e in zip2)
                {
                    TestContext.WriteLine("dir: {0}", e.FileName);
                    Assert.IsTrue(DirsAdded.Contains(e.FileName), "Cannot find the expected directory.");
                    Assert.IsTrue(e.IsDirectory);
                    dirCount++;
                }
            }
            Assert.AreEqual<int>(DirsAdded.Count, dirCount);
        }


        [TestMethod]
        public void Test_AddDirectoryByName_WithFiles()
        {
            Directory.SetCurrentDirectory(TopLevelDir);

            var DirsAdded = new System.Collections.Generic.List<String>();
            string password = TestUtilities.GenerateRandomPassword();
            string zipFileToCreate = Path.Combine(TopLevelDir, "Test_AddDirectoryByName_WithFiles.zip");
            using (ZipFile zip1 = new ZipFile(zipFileToCreate))
            {
                string DirName = null;
                int T = 3 + _rnd.Next(4);
                for (int n = 0; n < T; n++)
                {
                    // nested directories
                    DirName = (n == 0) ? "root" :
                        Path.Combine(DirName, TestUtilities.GenerateRandomAsciiString(8));

                    zip1.AddDirectoryByName(DirName);
                    DirsAdded.Add(DirName.Replace("\\", "/") + "/");
                    if (n % 2 == 0) zip1.Password = password;
                    zip1.AddEntry(Path.Combine(DirName, new System.String((char)(n + 48), 3) + ".txt"), "Hello, Dolly!");
                    if (n % 2 == 0) zip1.Password = null;
                }
                zip1.Save();
            }

            int entryCount = 0;
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                foreach (var e in zip2)
                {
                    TestContext.WriteLine("e: {0}", e.FileName);
                    if (e.IsDirectory)
                        Assert.IsTrue(DirsAdded.Contains(e.FileName), "Cannot find the expected directory.");
                    else
                    {
                        if ((entryCount - 1) % 4 == 0) e.Password = password;
                        string output = StreamToStringUTF8(e.OpenReader());
                        Assert.AreEqual<string>("Hello, Dolly!", output);
                    }
                    entryCount++;
                }
            }
            Assert.AreEqual<int>(DirsAdded.Count * 2, entryCount);
        }




        int _progressEventCalls;
        int _cancelIndex;
        Int64 maxBytesXferred = 0;
        void SaveProgress(object sender, SaveProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_AfterWriteEntry:
                    _progressEventCalls++;
                    TestContext.WriteLine("{0}: {1} ({2}/{3})", e.EventType.ToString(), e.CurrentEntry.FileName, e.EntriesSaved, e.EntriesTotal);
                    if (_cancelIndex == _progressEventCalls)
                    {
                        e.Cancel = true;
                        TestContext.WriteLine("Cancelling...");
                    }
                    break;

                case ZipProgressEventType.Saving_EntryBytesRead:
                    Assert.IsTrue(e.BytesTransferred <= e.TotalBytesToTransfer,
                        "For entry {0}, BytesTransferred is greater than TotalBytesToTransfer: ({1} > {2})",
                        e.CurrentEntry.FileName, e.BytesTransferred, e.TotalBytesToTransfer);
                    maxBytesXferred = e.BytesTransferred;
                    break;

                default:
                    break;
            }
        }


        void ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    _progressEventCalls++;
                    TestContext.WriteLine("Extracted: {0} ({1}/{2})", e.CurrentEntry.FileName, e.EntriesExtracted, e.EntriesTotal);
                    // synthetic cancellation
                    if (_cancelIndex == _progressEventCalls)
                    {
                        e.Cancel = true;
                        TestContext.WriteLine("Cancelling...");
                    }
                    break;

                case ZipProgressEventType.Extracting_EntryBytesWritten:
                    maxBytesXferred = e.BytesTransferred;
                    break;

                default:
                    break;
            }
        }


        [TestMethod]
        public void Create_WithEvents()
        {
            string DirToZip = Path.Combine(TopLevelDir, "EventTest");
            Directory.CreateDirectory(DirToZip);

            var randomizerSettings = new int[]
                {
                    6, 4,        // dircount
                    7, 8,        // filecount
                    10000, 15000 // filesize
                };
            int subdirCount = 0;
            int entriesAdded = TestUtilities.GenerateFilesOneLevelDeep(TestContext, "Create_WithEvents", DirToZip, randomizerSettings, null, out subdirCount);

            for (int m = 0; m < 2; m++)
            {
                TestContext.WriteLine("=======================================================");
                TestContext.WriteLine("Trial {0}", m);

                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_WithEvents-{0}.zip", m));
                string targetDirectory = Path.Combine(TopLevelDir, "unpack" + m.ToString());

                _progressEventCalls = 0;
                _cancelIndex = -1; // don't cancel this Save

                // create a zip file
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.SaveProgress += SaveProgress;
                    zip1.Comment = "This is the comment on the zip archive.";
                    zip1.AddDirectory(DirToZip, Path.GetFileName(DirToZip));
                    zip1.Save(zipFileToCreate);
                }

                if (m > 0)
                {
                    // update the zip file
                    using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                    {
                        zip1.SaveProgress += SaveProgress;
                        zip1.Comment = "This is the comment on the zip archive.";
                        zip1.AddEntry("ReadThis.txt", "This is the content for the readme file in the archive.");
                        zip1.Save();
                    }
                    entriesAdded++;
                }

                int expectedNumberOfProgressCalls = (entriesAdded + subdirCount) * (m + 1) + 1;
                Assert.AreEqual<Int32>(expectedNumberOfProgressCalls, _progressEventCalls,
                                       "The number of progress events was unexpected ({0}!={1}).", expectedNumberOfProgressCalls, _progressEventCalls);

                _progressEventCalls = 0;
                _cancelIndex = -1; // don't cancel this Extract
                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    zip2.ExtractProgress += ExtractProgress;
                    zip2.ExtractAll(targetDirectory);
                }

                Assert.AreEqual<Int32>(_progressEventCalls, entriesAdded + subdirCount + 1,
                                       "The number of Entries added is not equal to the number of entries extracted.");

            }

        }


        Ionic.CopyData.Transceiver _txrx;
        bool _pb2Set;
        bool _pb1Set;
        void LF_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            string msg;
            switch (e.EventType)
            {
                case ZipProgressEventType.Saving_Started:
                    _txrx.Send("status saving started...");
                    _pb1Set = false;
                    //_txrx.Send(String.Format("pb1 max {0}", e.EntriesTotal));
                    //_txrx.Send("pb2 max 1");
                    break;

                case ZipProgressEventType.Saving_BeforeWriteEntry:
                    _txrx.Send(String.Format("status Compressing {0}", e.CurrentEntry.FileName));
                    if (!_pb1Set)
                    {
                        _txrx.Send(String.Format("pb 1 max {0}", e.EntriesTotal));
                        _pb1Set = true;
                    }
                    _pb2Set = false;
                    break;

                case ZipProgressEventType.Saving_EntryBytesRead:
                    if (!_pb2Set)
                    {
                        _txrx.Send(String.Format("pb 2 max {0}", e.TotalBytesToTransfer));
                        _pb2Set = true;
                    }
                    _txrx.Send(String.Format("status Saving {0} :: [{1}/{2}mb] ({3:N0}%)",
                                             e.CurrentEntry.FileName,
                                             e.BytesTransferred / (1024 * 1024), e.TotalBytesToTransfer / (1024 * 1024),
                                             ((double)e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)
                                             ));
                    msg = String.Format("pb 2 value {0}", e.BytesTransferred);
                    _txrx.Send(msg);
                    Assert.IsTrue(e.BytesTransferred <= e.TotalBytesToTransfer);
                    if (maxBytesXferred < e.BytesTransferred)
                        maxBytesXferred = e.BytesTransferred;

                    break;

                case ZipProgressEventType.Saving_AfterWriteEntry:
                    _txrx.Send("pb 1 step");
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



        private int _numFilesToExtract;
        void LF_ExtractProgress(object sender, ExtractProgressEventArgs e)
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
                    _txrx.Send(String.Format("status Extracting {0} :: [{2}/{3}] ({1:N0}%)",
                                             e.CurrentEntry.FileName,
                                             ((double)e.BytesTransferred/(1024*1024)) / (0.01 * e.TotalBytesToTransfer/(1024*1024)),
                                             e.BytesTransferred, e.TotalBytesToTransfer));
                    string msg = String.Format("pb 2 value {0}", e.BytesTransferred);
                    _txrx.Send(msg);

                    if (maxBytesXferred < e.BytesTransferred)
                        maxBytesXferred = e.BytesTransferred;

                    break;

                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    _txrx.Send("pb 1 step");
                    break;
            }
        }



        [Timeout(3600000), TestMethod]
        public void LargeFile_WithProgress()
        {
            // This test checks the Int64 limits in progress events (Save + Extract)
            TestContext.WriteLine("Test beginning {0}", System.DateTime.Now.ToString("G"));

            string testBin = TestUtilities.GetTestBinDir(CurrentDir);

            string progressMonitorTool = Path.Combine(testBin, "Resources\\UnitTestProgressMonitor.exe");
            string requiredDll = Path.Combine(testBin, "Resources\\Ionic.CopyData.dll");

            Assert.IsTrue(File.Exists(progressMonitorTool), "progress monitor tool does not exist ({0})", progressMonitorTool);
            Assert.IsTrue(File.Exists(requiredDll), "required DLL does not exist ({0})", requiredDll);

            string progressChannel = "LargeFile_Progress";
            // start the progress monitor
            this.Exec(progressMonitorTool, String.Format("-channel {0}", progressChannel), false);

            // System.Reflection.Assembly.Load(requiredDll);

            System.Threading.Thread.Sleep(1000);
            _txrx = new Ionic.CopyData.Transceiver();
            _txrx.Channel = progressChannel;
            _txrx.Send("test Large File Save and Verify");
            _txrx.Send("bars 3");
            System.Threading.Thread.Sleep(120);
            _txrx.Send("status Creating a large file...");
            _txrx.Send(String.Format("pb 0 max {0}", 3));

            string zipFileToCreate = Path.Combine(TopLevelDir, "LargeFile_WithProgress.zip");
            string TargetDirectory = Path.Combine(TopLevelDir, "unpack");

            string DirToZip = Path.Combine(TopLevelDir, "LargeFile");
            Directory.CreateDirectory(DirToZip);

            Int64 filesize = 0x7FFFFFFFL + _rnd.Next(1000000);
            TestContext.WriteLine("Creating a large file, size({0})", filesize);
            string filename = Path.Combine(DirToZip, "LargeFile.bin");

            _txrx.Send(String.Format("pb 1 max {0}", filesize));

            Action<Int64> progressUpdate = (x) =>
                {
                    _txrx.Send(String.Format("pb 1 value {0}", x));
                    _txrx.Send(String.Format("status Creating a large file, ({0}/{1}mb) ({2:N0}%)",
                                             x / (1024 * 1024), filesize / (1024 * 1024),
                                             ((double)x) / (0.01 * filesize)));
                };

            TestUtilities.CreateAndFillFileBinaryZeroes(filename, filesize, progressUpdate);
            _txrx.Send("pb 0 step");
            TestContext.WriteLine("File Create complete {0}", System.DateTime.Now.ToString("G"));

            maxBytesXferred = 0;
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.SaveProgress += LF_SaveProgress;
                zip1.Comment = "This is the comment on the zip archive.";
                zip1.AddEntry("Readme.txt", "This is some content.");
                zip1.AddDirectory(DirToZip, Path.GetFileName(DirToZip));
                zip1.BufferSize = 65536 * 8; // 512k
                zip1.CodecBufferSize = 65536 * 2; // 128k
                zip1.Save(zipFileToCreate);
            }

            _txrx.Send("pb 0 step");

            TestContext.WriteLine("Save complete {0}", System.DateTime.Now.ToString("G"));

            Assert.AreEqual<Int64>(filesize, maxBytesXferred,
                "The number of bytes saved is not the expected value.");

            // remove the very large file before extracting
            Directory.Delete(DirToZip, true);

            _pb1Set = false;
            maxBytesXferred = 0;
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                _numFilesToExtract = zip2.Entries.Count;
                zip2.ExtractProgress += LF_ExtractProgress;
                zip2.BufferSize = 65536 * 8;
                zip2.ExtractAll(TargetDirectory);
            }

            _txrx.Send("pb 0 step");

            TestContext.WriteLine("Extract complete {0}", System.DateTime.Now.ToString("G"));

            Assert.AreEqual<Int64>(filesize, maxBytesXferred,
                   "The number of bytes extracted is not the expected value.");

            TestContext.WriteLine("Test complete {0}", System.DateTime.Now.ToString("G"));

            _txrx.Send("stop");
        }


        [TestMethod]
        public void CreateZip_AddDirectory_NoFilesInRoot_WI5893()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "CreateZip_AddDirectory_NoFilesInRoot_WI5893.zip");
            int i, j;
            int entries = 0;

            int subdirCount = _rnd.Next(5) + 5;
            for (i = 0; i < subdirCount; i++)
            {
                string Subdir = Path.Combine(TopLevelDir, "DirectoryToZip.test." + i);
                Directory.CreateDirectory(Subdir);

                int fileCount = _rnd.Next(13) + 7;
                for (j = 0; j < fileCount; j++)
                {
                    String file = Path.Combine(Subdir, String.Format("file{0:D3}.a", j));
                    TestUtilities.CreateAndFillFile(file, _rnd.Next(100) + 500);
                    entries++;
                }
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(TopLevelDir, string.Empty);
                zip.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);
            Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entries, "The Zip file has the wrong number of entries.");
        }


        [TestMethod]
        public void Create_AddDirectory_NoFilesInRoot_WI5893a()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_AddDirectory_NoFilesInRoot_WI5893a.zip");

            int i, j;
            int entries = 0;

            int subdirCount = _rnd.Next(4) + 4;
            for (i = 0; i < subdirCount; i++)
            {
                string Subdir = Path.Combine(TopLevelDir, "DirectoryToZip.test." + i);
                Directory.CreateDirectory(Subdir);

                int fileCount = _rnd.Next(16) + 8;
                for (j = 0; j < fileCount; j++)
                {
                    String file = Path.Combine(Subdir, String.Format("testfile{0:D3}.a", j));
                    TestUtilities.CreateAndFillFile(file, _rnd.Next(100) + 500);
                    entries++;
                }
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(TopLevelDir, string.Empty);
                zip.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), entries, "The Zip file has the wrong number of entries.");
        }




        [TestMethod]
        public void Create_SaveCancellation()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_SaveCancellation.zip");

            string DirToZip = Path.Combine(TopLevelDir, "EventTest");
            Directory.CreateDirectory(DirToZip);

            int subdirCount = 0;
            int entriesAdded = TestUtilities.GenerateFilesOneLevelDeep(TestContext, "Create_SaveCancellation", DirToZip, null, out subdirCount);

            _cancelIndex = entriesAdded - _rnd.Next(entriesAdded / 2);
            _progressEventCalls = 0;
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.SaveProgress += SaveProgress;
                zip1.Comment = "The save on this zip archive will be canceled.";
                zip1.AddDirectory(DirToZip, Path.GetFileName(DirToZip));
                zip1.Save(zipFileToCreate);
            }

            Assert.AreEqual<Int32>(_progressEventCalls, _cancelIndex);

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file save should have been canceled.");
        }



        [TestMethod]
        public void ExtractAll_Cancellation()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "ExtractAll_Cancellation.zip");
            string TargetDirectory = Path.Combine(TopLevelDir, "unpack");

            string DirToZip = Path.Combine(TopLevelDir, "EventTest");
            Directory.CreateDirectory(DirToZip);

            int subdirCount = 0;
            int entriesAdded = TestUtilities.GenerateFilesOneLevelDeep(TestContext, "ExtractAll_Cancellation", DirToZip, null, out subdirCount);

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.Comment = "The extract on this zip archive will be canceled.";
                zip1.AddDirectory(DirToZip, Path.GetFileName(DirToZip));
                zip1.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);

            _cancelIndex = entriesAdded - _rnd.Next(entriesAdded / 2);
            _progressEventCalls = 0;
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                zip2.ExtractProgress += ExtractProgress;
                zip2.ExtractAll(TargetDirectory);
            }

            Assert.AreEqual<Int32>(_progressEventCalls, _cancelIndex);
        }



        [TestMethod]
        public void ExtractAll_WithPassword()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "ExtractAll_WithPassword.zip");
            string TargetDirectory = Path.Combine(TopLevelDir, "unpack");

            string DirToZip = Path.Combine(TopLevelDir, "DirToZip");
            Directory.CreateDirectory(DirToZip);
            int subdirCount = 0;

            int entriesAdded = TestUtilities.GenerateFilesOneLevelDeep(TestContext, "ExtractAll_WithPassword", DirToZip, null, out subdirCount);
            string password = TestUtilities.GenerateRandomPassword();
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.Password = password;
                zip1.Comment = "Brick walls are there for a reason: to let you show how badly you want your goal.";
                zip1.AddDirectory(DirToZip, Path.GetFileName(DirToZip));
                zip1.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate, password);

            _cancelIndex = -1; // don't cancel this Extract
            _progressEventCalls = 0;
            using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
            {
                zip2.Password = password;
                zip2.ExtractProgress += ExtractProgress;
                zip2.ExtractAll(TargetDirectory);
            }

            Assert.AreEqual<Int32>(_progressEventCalls, entriesAdded + subdirCount + 1);
        }





        [TestMethod]
        public void Extract_ImplicitPassword()
        {
            for (int k = 0; k < compLevels.Length; k++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Extract_ImplicitPassword-{0}.zip", k));

                Directory.SetCurrentDirectory(TopLevelDir);
                string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

                var files = TestUtilities.GenerateFilesFlat(dirToZip);
                string[] passwords = new string[files.Length];

                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.Comment = "Brick walls are there for a reason: to let you show how badly you want your goal.";
                    zip1.CompressionLevel = compLevels[k];
                    for (int i = 0; i < files.Length; i++)
                    {
                        passwords[i] = TestUtilities.GenerateRandomPassword();
                        zip1.Password = passwords[i];
                        TestContext.WriteLine("  Adding entry: {0} pw({1})", files[i], passwords[i]);
                        zip1.AddFile(files[i], Path.GetFileName(dirToZip));
                    }
                    zip1.Save(zipFileToCreate);
                }
                TestContext.WriteLine("\n");

                // extract using the entry from the enumerator
                int nExtracted = 0;
                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    foreach (ZipEntry e in zip2)
                    {
                        e.Password = passwords[nExtracted];
                        TestContext.WriteLine("  Extracting entry: {0} pw({1})", e.FileName, passwords[nExtracted]);
                        e.Extract("unpack1");
                        nExtracted++;
                    }
                }

                Assert.AreEqual<Int32>(files.Length, nExtracted);

                // extract using the filename indexer
                nExtracted = 0;
                using (ZipFile zip3 = ZipFile.Read(zipFileToCreate))
                {
                    foreach (var name in zip3.EntryFileNames)
                    {
                        zip3.Password = passwords[nExtracted];
                        zip3[name].Extract("unpack2");
                        nExtracted++;
                    }
                }

                Assert.AreEqual<Int32>(files.Length, nExtracted);
            }
        }



        [TestMethod]
        public void Extract_MultiThreaded_wi6637()
        {
            int nConcurrentZipFiles = 5;
            for (int k = 0; k < 1; k++)
            {
                TestContext.WriteLine("\n-----------------------------\r\n{0}: Trial {1}...",
                      DateTime.Now.ToString("HH:mm:ss"),
                      k);

                Directory.SetCurrentDirectory(TopLevelDir);

                string[] zipFileToCreate = new string[nConcurrentZipFiles];
                for (int m = 0; m < nConcurrentZipFiles; m++)
                {
                    zipFileToCreate[m] = Path.Combine(TopLevelDir, String.Format("Extract_MultiThreaded-{0}-{1}.zip", k, m));
                    TestContext.WriteLine("  Creating file: {0}", zipFileToCreate[m]);
                    string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

                    var files = TestUtilities.GenerateFilesFlat(dirToZip);
                    TestContext.WriteLine("Zipping {0} files from dir '{1}'...", files.Length, dirToZip);

                    using (ZipFile zip1 = new ZipFile())
                    {
                        zip1.Comment = "Brick walls are there for a reason: to let you show how badly you want your goal.";
                        for (int i = 0; i < files.Length; i++)
                        {
                            TestContext.WriteLine("  Adding entry: {0}", files[i]);
                            zip1.AddFile(files[i], Path.GetFileName(dirToZip));
                        }
                        zip1.Save(zipFileToCreate[m]);
                    }
                    TestContext.WriteLine("\n");
                    BasicVerifyZip(zipFileToCreate[m]);
                }


                // multi-thread extract
                foreach (string fileName in zipFileToCreate)
                {
                    TestContext.WriteLine("queueing unzip for file: {0}", fileName);
                    System.Threading.ThreadPool.QueueUserWorkItem(processZip, fileName);
                }

                while (completedEntries != zipFileToCreate.Length)
                    System.Threading.Thread.Sleep(400);

                TestContext.WriteLine("done.");

            }
        }



        private int _completedEntries;
        private int completedEntries
        {
            get { return _completedEntries; }
            set
            {
                lock (this)
                {
                    _completedEntries = value;
                }
            }
        }



        private void processZip(object o)
        {
            string fileName = o as string;

            string zDir = Path.Combine("extract",
                             Path.GetFileNameWithoutExtension(fileName.ToString()));

            TestContext.WriteLine("extracting {0}...", fileName);

            using (var zFile = ZipFile.Read(fileName))
            {
                zFile.ExtractAll(zDir, ExtractExistingFileAction.OverwriteSilently);
            }
            completedEntries++;
        }





        void OverwriteDecider(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite:
                    // randomly choose whether to overwrite or not
                    e.CurrentEntry.ExtractExistingFile = (_rnd.Next(2) == 0)
                        ? ExtractExistingFileAction.DoNotOverwrite
                        : ExtractExistingFileAction.OverwriteSilently;
                    break;
            }
        }



        [TestMethod]
        public void Extract_ExistingFile()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Extract_ExistingFile.zip");

            string SourceDir = CurrentDir;
            for (int i = 0; i < 3; i++)
                SourceDir = Path.GetDirectoryName(SourceDir);

            Directory.SetCurrentDirectory(TopLevelDir);

            string[] filenames =
                {
                    Path.Combine(SourceDir, "Tools\\Zipit\\bin\\Debug\\Zipit.exe"),
                    Path.Combine(SourceDir, "Zip Full DLL\\bin\\Debug\\Ionic.Zip.dll"),
                    Path.Combine(SourceDir, "Zip Full DLL\\bin\\Debug\\Ionic.Zip.pdb"),
                    Path.Combine(SourceDir, "Zip Full DLL\\bin\\Debug\\Ionic.Zip.xml"),
                    //Path.Combine(SourceDir, "AppNote.txt")
                };

            int j = 0;
            using (ZipFile zip = new ZipFile())
            {
                for (j = 0; j < filenames.Length; j++)
                    zip.AddFile(filenames[j], "");
                zip.Comment = "This is a Comment On the Archive";
                zip.Save(zipFileToCreate);
            }


            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), filenames.Length,
                                 "The zip file created has the wrong number of entries.");

            TestContext.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            TestContext.WriteLine("1. first extract - this should succeed");
            var options = new ReadOptions { StatusMessageWriter = new StringWriter() };
            using (ZipFile zip = ZipFile.Read(zipFileToCreate, options))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    var f = Path.GetFileName(filenames[j]);
                    zip[f].Extract("unpack", ExtractExistingFileAction.Throw);
                }
            }
            TestContext.WriteLine(options.StatusMessageWriter.ToString());

            TestContext.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            TestContext.WriteLine("2. extract again - DoNotOverwrite");
            options.StatusMessageWriter = new StringWriter();
            using (ZipFile zip = ZipFile.Read(zipFileToCreate, options))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    var f = Path.GetFileName(filenames[j]);
                    zip[f].Extract("unpack", ExtractExistingFileAction.DoNotOverwrite);
                }
            }
            TestContext.WriteLine(options.StatusMessageWriter.ToString());

            TestContext.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            TestContext.WriteLine("3. extract again - OverwriteSilently");
            options.StatusMessageWriter = new StringWriter();
            using (ZipFile zip = ZipFile.Read(zipFileToCreate, options))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    var f = Path.GetFileName(filenames[j]);
                    zip[f].Extract("unpack", ExtractExistingFileAction.OverwriteSilently);
                }
            }
            TestContext.WriteLine(options.StatusMessageWriter.ToString());

            TestContext.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            TestContext.WriteLine("4. extract again - InvokeExtractProgressEvent");
            options.StatusMessageWriter = new StringWriter();
            using (ZipFile zip = ZipFile.Read(zipFileToCreate, options))
            {
                zip.ExtractProgress += OverwriteDecider;
                for (j = 0; j < filenames.Length; j++)
                {
                    var f = Path.GetFileName(filenames[j]);
                    zip[f].Extract("unpack", ExtractExistingFileAction.InvokeExtractProgressEvent);
                }
            }
            TestContext.WriteLine(options.StatusMessageWriter.ToString());
        }




        [TestMethod]
        public void Extended_CheckZip1()
        {
            string[] dirNames = { "", Path.GetRandomFileName() };

            string textToEncode =
                "Pay no attention to this: " +
                "We've read in the regular entry header, the extra field, and any encryption " +
                "header.  The pointer in the file is now at the start of the filedata, which is " +
                "potentially compressed and encrypted.  Just ahead in the file, there are " +
                "_CompressedFileDataSize bytes of data, followed by potentially a non-zero length " +
                "trailer, consisting of optionally, some encryption stuff (10 byte MAC for AES), " +
                "and the bit-3 trailer (16 or 24 bytes). ";

            Directory.SetCurrentDirectory(TopLevelDir);

            for (int i = 0; i < crypto.Length; i++)
            {
                for (int j = 0; j < z64.Length; j++)
                {
                    for (int k = 0; k < dirNames.Length; k++)
                    {
                        string zipFile = Path.Combine(TopLevelDir, String.Format("Extended-CheckZip1-{0}.{1}.{2}.zip", i, j, k));
                        string password = Path.GetRandomFileName();

                        TestContext.WriteLine("=================================");
                        TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFile));
                        TestContext.WriteLine("Encryption:{0}  Zip64:{1} pw={2}",
                                              crypto[i].ToString(), z64[j].ToString(), password);
                        using (var zip = new ZipFile())
                        {
                            zip.Comment = String.Format("Encryption={0}  Zip64={1}  pw={2}",
                                                        crypto[i].ToString(), z64[j].ToString(), password);
                            zip.Encryption = crypto[i];
                            zip.Password = password;
                            zip.UseZip64WhenSaving = z64[j];
                            if (!String.IsNullOrEmpty(dirNames[k]))
                                zip.AddDirectoryByName(dirNames[k]);
                            zip.AddEntry(Path.Combine(dirNames[k], "File1.txt"), textToEncode);
                            zip.Save(zipFile);
                        }

                        BasicVerifyZip(zipFile, password);
                        TestContext.WriteLine("Checking zip...");
                        System.Collections.ObjectModel.ReadOnlyCollection<String> msgs;

                        bool result = ZipFile.CheckZip(zipFile, false, out msgs);
                        TestContext.WriteLine("Messages: ({0})", msgs.Count);
                        foreach (var m in msgs)
                        {
                            TestContext.WriteLine("{0}", m);
                        }

                        Assert.IsTrue(result, "Zip ({0}) does not check OK", zipFile);

                    }
                }
            }
        }



        [TestMethod]
        public void Extended_CheckZip2()
        {
            string textToEncode =
                "Pay no attention to this: " +
                "We've read in the regular entry header, the extra field, and any encryption " +
                "header.  The pointer in the file is now at the start of the filedata, which is " +
                "potentially compressed and encrypted.  Just ahead in the file, there are " +
                "_CompressedFileDataSize bytes of data, followed by potentially a non-zero length " +
                "trailer, consisting of optionally, some encryption stuff (10 byte MAC for AES), " +
                "and the bit-3 trailer (16 or 24 bytes). All the various combinations of possibilities " +
                "are what make testing a zip library so challenging.";

            string testBin = TestUtilities.GetTestBinDir(CurrentDir);
            string fileToZip = Path.Combine(testBin, "Ionic.Zip.dll");

            Directory.SetCurrentDirectory(TopLevelDir);

            for (int i = 0; i < crypto.Length; i++)
            {
                for (int j = 0; j < z64.Length; j++)
                {
                    string zipFile = Path.Combine(TopLevelDir, String.Format("Extended-CheckZip2-{0}.{1}.zip", i, j));
                    string password = Path.GetRandomFileName();

                    TestContext.WriteLine("=================================");
                    TestContext.WriteLine("Creating {0}...", Path.GetFileName(zipFile));
                    TestContext.WriteLine("Encryption({0})  Zip64({1}) pw({2})",
                                          crypto[i].ToString(), z64[j].ToString(), password);

                    string dir = Path.GetRandomFileName();
                    using (var zip = new ZipFile())
                    {
                        zip.Comment = String.Format("Encryption={0}  Zip64={1}  pw={2}",
                                                    crypto[i].ToString(), z64[j].ToString(), password);
                        zip.Encryption = crypto[i];
                        zip.Password = password;
                        zip.UseZip64WhenSaving = z64[j];
                        int N = _rnd.Next(14) + 8;
                        for (int k = 0; k < N; k++)
                            zip.AddDirectoryByName(Path.GetRandomFileName());

                        zip.AddEntry("File1.txt", textToEncode);
                        zip.AddFile(fileToZip, Path.GetRandomFileName());
                        zip.Save(zipFile);
                    }

                    BasicVerifyZip(zipFile, password);

                    TestContext.WriteLine("Checking zip...");
                    System.Collections.ObjectModel.ReadOnlyCollection<String> msgs;

                    bool result = ZipFile.CheckZip(zipFile, false, out msgs);
                    TestContext.WriteLine("Messages: ({0})", msgs.Count);
                    foreach (var m in msgs)
                    {
                        TestContext.WriteLine("{0}", m);
                    }

                    Assert.IsTrue(result, "Zip ({0}) does not check OK", zipFile);

                }
            }
        }


        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Create_DuplicateNames_DifferentFolders_wi8982_flat()
        {
            _Internal_DuplicateNames_DifferentFolders_wi8982(true);
        }

        [TestMethod]
        public void Create_DuplicateNames_DifferentFolders_wi8982_PreserveHierarchy()
        {
            _Internal_DuplicateNames_DifferentFolders_wi8982(false);
        }

        public void _Internal_DuplicateNames_DifferentFolders_wi8982(bool flat)
        {
            Directory.SetCurrentDirectory(TopLevelDir);

            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            TestUtilities.GenerateFilesFlat(dirToZip, 3);
            string subdir = Path.Combine(dirToZip, "subdir1");
            TestUtilities.GenerateFilesFlat(subdir, 2);

            for (int i = 0; i < 2; i++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_DuplicateNames_DifferentFolders.{0}.zip", i));

                using (var zip = new ZipFile())
                {
                    zip.ZipErrorAction = ZipErrorAction.Throw;
                    if (i == 0)
                        zip.AddDirectory(dirToZip, "fodder");
                    else
                    {
                        var files = Directory.GetFiles(dirToZip, "*.*", SearchOption.AllDirectories);
                        if (flat)
                            zip.AddFiles(files, "fodder");
                        else
                            zip.AddFiles(files, true, "fodder");

                    }

                    zip.Save(zipFileToCreate);
                }

                BasicVerifyZip(zipFileToCreate);

                Assert.AreEqual<int>(5, TestUtilities.CountEntries(zipFileToCreate),
                                     "Trial {0}: The zip file created has the wrong number of entries.", i);
            }
        }



        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void Create_ZipErrorAction_Throw()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_ZipErrorAction_Throw.zip");
            Directory.SetCurrentDirectory(TopLevelDir);
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            int n = _rnd.Next(files.Length);

            TestContext.WriteLine("Locking file {0}...", files[n]);
            using (Stream lockStream = new FileStream(files[n], FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (var zip = new ZipFile())
                {
                    zip.ZipErrorAction = ZipErrorAction.Throw;
                    zip.AddFiles(files, "fodder");
                    zip.Save(zipFileToCreate);
                }
            }
        }





        [TestMethod]
        public void Create_ZipErrorAction_Skip()
        {
            Directory.SetCurrentDirectory(TopLevelDir);
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            //var files = TestUtilities.GenerateFilesFlat(dirToZip, 4);

            // m is the number of files to lock
            for (int m = 1; m < 4; m++)
            {
                // k is the type of locking.  0 == whole file, 1 == range lock
                for (int k = 0; k < 2; k++)
                {
                    TestContext.WriteLine("Trial {0}.{1}...", m, k);
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_ZipErrorAction_Skip-{0}-{1}.zip", m, k));
                    var locked = new Dictionary<String, FileStream>();
                    try
                    {
                        for (int i = 0; i < m; i++)
                        {
                            int n = 0;
                            do
                            {
                                n = _rnd.Next(files.Length);
                            } while (locked.ContainsKey(files[n]));

                            TestContext.WriteLine("  Locking file {0}...", files[n]);

                            FileStream lockStream = null;
                            if (k == 0)
                            {
                                lockStream = new FileStream(files[n], FileMode.Open, FileAccess.Read, FileShare.None);
                            }
                            else
                            {
                                lockStream = new FileStream(files[n], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                int r = _rnd.Next((int)(lockStream.Length / 2));
                                int s = _rnd.Next((int)(lockStream.Length / 2));
                                lockStream.Lock(s, r);
                            }

                            locked.Add(files[n], lockStream);
                        }

                        using (var zip = new ZipFile())
                        {
                            zip.ZipErrorAction = ZipErrorAction.Skip;
                            zip.AddFiles(files, "fodder");
                            zip.Save(zipFileToCreate);
                        }

                        using (var zip = new ZipFile(zipFileToCreate))
                        {
                            // Writing the info as a single block puts everything on the
                            // same line, makes it unreadable.  So we split the strings on
                            // newline boundaries and write them individually.
                            foreach (string s in zip.Info.Split('\r', '\n'))
                            {
                                Console.WriteLine("{0}", s);
                            }
                        }

                        BasicVerifyZip(zipFileToCreate);

                        Assert.AreEqual<int>(files.Length - m, TestUtilities.CountEntries(zipFileToCreate),
                                             "The zip file created has the wrong number of entries.");
                    }
                    finally
                    {
                        foreach (String s in locked.Keys)
                        {
                            locked[s].Close();
                        }
                    }

                    TestContext.WriteLine("  ...");
                    System.Threading.Thread.Sleep(320);
                }
            }
        }



        private int _retryCount;
        void ErrorHandler_RetryAndEventuallySkip(object sender, ZipErrorEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Error_Saving:
                    _retryCount++;
                    if (_retryCount < 29)
                        e.CurrentEntry.ZipErrorAction = ZipErrorAction.Retry;
                    else
                        e.CurrentEntry.ZipErrorAction = ZipErrorAction.Skip;
                    break;
            }
        }

        void ErrorHandler_RetryAndEventuallyThrow(object sender, ZipErrorEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Error_Saving:
                    _retryCount++;
                    if (_retryCount < 29)
                        e.CurrentEntry.ZipErrorAction = ZipErrorAction.Retry;
                    else
                        e.CurrentEntry.ZipErrorAction = ZipErrorAction.Throw;
                    break;
            }
        }



        [TestMethod]
        public void Create_ZipErrorAction_RetryAndEventuallySkip()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_ZipErrorAction_RetryAndEventuallySkip.zip");
            Directory.SetCurrentDirectory(TopLevelDir);
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            int n = _rnd.Next(files.Length);

            TestContext.WriteLine("Locking file {0}...", files[n]);
            using (Stream lockStream = new FileStream(files[n], FileMode.Open, FileAccess.Read, FileShare.None))
            {
                _retryCount = 0;
                using (var zip = new ZipFile())
                {
                    zip.ZipErrorAction = ZipErrorAction.InvokeErrorEvent;
                    zip.ZipError += ErrorHandler_RetryAndEventuallySkip;
                    zip.AddFiles(files, "fodder");
                    zip.Save(zipFileToCreate);
                }
            }

            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(files.Length - 1, TestUtilities.CountEntries(zipFileToCreate),
                                 "The zip file created has the wrong number of entries.");
        }



        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void Create_ZipErrorAction_RetryAndEventuallyThrow()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_ZipErrorAction_RetryAndEventuallyThrow.zip");
            Directory.SetCurrentDirectory(TopLevelDir);
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            int n = _rnd.Next(files.Length);

            TestContext.WriteLine("Locking file {0}...", files[n]);
            using (Stream lockStream = new FileStream(files[n], FileMode.Open, FileAccess.Read, FileShare.None))
            {
                _retryCount = 0;
                using (var zip = new ZipFile())
                {
                    zip.ZipErrorAction = ZipErrorAction.InvokeErrorEvent;
                    zip.ZipError += ErrorHandler_RetryAndEventuallyThrow;
                    zip.AddFiles(files, "fodder");
                    zip.Save(zipFileToCreate);
                }
            }
        }


        private void lockFile(object state)
        {
            Object[] a = (Object[])state;
            string filename = (string)a[0];
            int duration = (int)a[1];

            using (Stream lockStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                // hold the lock for a specified period of time
                System.Threading.Thread.Sleep(duration);
            }
        }



        [TestMethod]
        public void Create_ZipErrorAction_RetryAndEventuallySucceed()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Create_ZipErrorAction_RetryAndEventuallySucceed.zip");
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            int n = _rnd.Next(files.Length);

            TestContext.WriteLine("Locking file {0}...", files[n]);

            // This will lock the file for 3 seconds, then release it.
            // The goal is to test whether the retry actually succeeds.
            System.Threading.ThreadPool.QueueUserWorkItem(lockFile, new Object[] { files[n], 3000 });
            System.Threading.Thread.Sleep(200);

            _retryCount = 0;
            using (var zip = new ZipFile())
            {
                zip.ZipErrorAction = ZipErrorAction.Retry;
                zip.AddFiles(files, "fodder");
                zip.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                 "The zip file created has the wrong number of entries.");
        }





        [TestMethod]
        public void ParallelDeflateStream_Create()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "ParallelDeflateStream_Create.zip");
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip, _rnd.Next(5) + 5, 128 * 1024 + _rnd.Next(20000));

            using (var zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = 65536;
                zip.AddFiles(files, "fodder");
                zip.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                 "The zip file created has the wrong number of entries.");
        }



        [TestMethod]
        public void ParallelDeflateStream_Create_CompareSpeeds()
        {
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip, _rnd.Next(5) + 5, 2048 * 1024 + _rnd.Next(200000));

            TimeSpan[] ts = new TimeSpan[2];

            // 2 sets of 2 cycles: first set is warmup, 2nd is timed.
            // Actually they're both timed but times for the 2nd set
            // overwrite the times for the 1st set.
            // Within a set, the first run is non-parallel, 2nd is timed parallel.
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("ParallelDeflateStream_Create.{0}.{1}.zip", i, j));

                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    using (var zip = new ZipFile())
                    {
                        if (j == 0)
                            zip.ParallelDeflateThreshold = -1L; // disable parallel deflate
                        else
                            zip.ParallelDeflateThreshold = 128 * 1024;  // threshold for parallel deflating

                        zip.AddFiles(files, "fodder");

                        zip.Save(zipFileToCreate);
                    }
                    sw.Stop();

                    BasicVerifyZip(zipFileToCreate);

                    Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                         "The zip file created has the wrong number of entries.");
                    ts[j] = sw.Elapsed;
                    TestContext.WriteLine("Cycle {0},{1}, Timespan: {2}", i, j, ts[j]);
                }
            }
            Assert.IsTrue(ts[1] < ts[0], "Parallel deflating is NOT faster than single-threaded, for large files.");
        }



        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ParallelDeflateStream_Create_InvalidThreshold()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "ParallelDeflateStream_Create_InvalidThreshold.zip");
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip, _rnd.Next(5) + 5, 128 * 1024 + _rnd.Next(20000));

            using (var zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = 17129;
                zip.AddFiles(files, "fodder");
                zip.Save(zipFileToCreate);
            }

            // not reached
        }





        [TestMethod]
        public void CompressTiff_Level9_wi8647()
        {
            Directory.SetCurrentDirectory(TopLevelDir);
            string testBin = TestUtilities.GetTestBinDir(CurrentDir);

            string tifFile = Path.Combine(testBin, "Resources\\wi8647.tif");
            Assert.IsTrue(File.Exists(tifFile), "tif file does not exist ({0})", tifFile);

            byte[] chk1 = TestUtilities.ComputeChecksum(tifFile);
            string chk1String = TestUtilities.CheckSumToString(chk1);

            for (int x = 0; x < (int)(Ionic.Zlib.CompressionLevel.BestCompression); x++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir,
                                                      String.Format("CompressTiff_Level9-{0}.zip", x));
                byte[] chk2 = null;

                using (var zip = new ZipFile())
                {
                    zip.CompressionLevel = (Ionic.Zlib.CompressionLevel)x;
                    zip.AddFile(tifFile, "fodder");
                    zip.Save(zipFileToCreate);
                }

                BasicVerifyZip(zipFileToCreate);

                Assert.AreEqual<int>(1, TestUtilities.CountEntries(zipFileToCreate),
                                     "The zip file created has the wrong number of entries.");

                TestContext.WriteLine("---------------Reading {0}...", zipFileToCreate);
                string extractDir = String.Format("extract{0}", x);
                using (ZipFile zip = ZipFile.Read(zipFileToCreate))
                {
                    var e = zip[0];

                    TestContext.WriteLine(" Entry: {0}  c({1})  u({2})",
                                          e.FileName,
                                          e.CompressedSize,
                                          e.UncompressedSize);
                    e.Extract(extractDir);
                    string filename = Path.Combine(extractDir, e.FileName);
                    chk2 = TestUtilities.ComputeChecksum(filename);
                }

                string chk2String = TestUtilities.CheckSumToString(chk2);

                Assert.AreEqual<string>(chk1String, chk2String, "Cycle {0}, Checksums for ({1}) do not match.", x, tifFile);
                TestContext.WriteLine(" Cycle {0}: Checksums match ({1}).\n", x, chk1String);
            }
        }




        [Timeout(30000), TestMethod]  // timeout in ms.  30000 = 30s
        public void AddDirectory_ReparsePoint_wi8617()
        {
            _Internal_AddDirectory_ReparsePoint_wi8617(1);
        }



        [TestMethod]
        [ExpectedException(typeof(System.IO.PathTooLongException))]
        public void AddDirectory_ReparsePoint_wi8617_Error1()
        {
            _Internal_AddDirectory_ReparsePoint_wi8617(2);
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.PathTooLongException))]
        public void AddDirectory_ReparsePoint_wi8617_Error2()
        {
            _Internal_AddDirectory_ReparsePoint_wi8617(0);
        }


        private void _Internal_AddDirectory_ReparsePoint_wi8617(int flavor)
        {
            string zipFileToCreate = Path.Combine(TopLevelDir,
                                                  String.Format("AddDirectory_ReparsePoint-{0}.zip",
                                                                flavor));
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);

            string junction = Path.Combine(dirToZip, "cycle");
            Ionic.IO.JunctionPoint.Create(junction, dirToZip);

            using (var zip = new ZipFile())
            {
                if (flavor == 1)
                    zip.AddDirectoryWillTraverseReparsePoints = false;
                else if (flavor == 2)
                    zip.AddDirectoryWillTraverseReparsePoints = true;
                // else nothing
                zip.AddDirectory(dirToZip, "fodder");
                zip.Save(zipFileToCreate);
            }

            BasicVerifyZip(zipFileToCreate);

            Assert.AreEqual<int>(files.Length, TestUtilities.CountEntries(zipFileToCreate),
                                 "The zip file created has the wrong number of entries.");

        }



        [TestMethod]
        public void SortedSave()
        {
            var rtg = new RandomTextGenerator();

            WriteDelegate writer = (name, stream) =>
                {
                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes(rtg.Generate(_rnd.Next(2000) + 200));
                    stream.Write(buffer, 0, buffer.Length);
                };

            int numEntries = _rnd.Next(256) + 48;

            // Two trials, one with sorted output, and the other with non-sorted output.
            for (int m = 0; m < 2; m++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir,
                                                      String.Format("SortedSave-{0}.zip", m));
                using (var zip = new ZipFile())
                {
                    for (int i = 0; i < numEntries; i++)
                    {
                        // I need the randomness in the first part, to force the sort.
                        string filename = String.Format("{0}-{1:000}.txt",
                                                        TestUtilities.GenerateRandomAsciiString(6), i);
                        zip.AddEntry(filename, writer);
                    }

                    zip.SortEntriesBeforeSaving = (m == 1);
                    zip.Save(zipFileToCreate);
                }

                using (var zip = ZipFile.Read(zipFileToCreate))
                {
                    bool sorted = true;
                    for (int i = 0; i < zip.Entries.Count - 1 && sorted; i++)
                    {
                        for (int j = i; j < zip.Entries.Count && sorted; j++)
                        {
                            if (String.Compare(zip[i].FileName, zip[j].FileName, StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                sorted = false;
                            }
                        }
                    }

                    Assert.IsTrue((((m == 1) && sorted) || ((m == 0) && !sorted)),
                        "Unexpected sort results");
                }
            }

        }


    }


}
