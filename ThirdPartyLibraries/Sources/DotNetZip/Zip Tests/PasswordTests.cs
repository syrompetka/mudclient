// PasswordTests.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2008, 2009 Dino Chiesa and Microsoft Corporation.  
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
// Time-stamp: <2009-September-13 17:49:11>
//
// ------------------------------------------------------------------
//
// This module provides tests for password features. 
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;

namespace Ionic.Zip.Tests.Password
{
    [TestClass]
    public class PasswordTests : IonicTestClass
    {
        public PasswordTests() : base() { }

        [TestMethod]
        public void Password_BasicAddAndExtract()
        {
            int i;
            string[] Passwords = { null, "Password!", TestUtilities.GenerateRandomPassword(), "A" };

            Ionic.Zlib.CompressionLevel[] compressionLevelOptions = {
                Ionic.Zlib.CompressionLevel.None,
                Ionic.Zlib.CompressionLevel.BestSpeed,
                Ionic.Zlib.CompressionLevel.Default,
                Ionic.Zlib.CompressionLevel.BestCompression,
            };

            for (int k = 0; k < compressionLevelOptions.Length; k++)
            {
                for (int j = 0; j < Passwords.Length; j++)
                {
                    TestContext.WriteLine("\n\n===================\nTrial ({0}) pw({1})", j, Passwords[j]);
                    string ZipFileToCreate = Path.Combine(TopLevelDir, String.Format("Password_BasicAddAndExtract-{0}-{1}.zip", k, j));
                    Assert.IsFalse(File.Exists(ZipFileToCreate), "The temporary zip file '{0}' already exists.", ZipFileToCreate);

                    Directory.SetCurrentDirectory(TopLevelDir);
                    string DirToZip = String.Format("zipthis-{0}-{1}", k, j);
                    Directory.CreateDirectory(DirToZip);

                    TestContext.WriteLine("\n---------------------creating files and computing checksums...");
                    int NumFilesToCreate = _rnd.Next(10) + 10;
                    string[] filenames = new string[NumFilesToCreate];
                    var checksums = new Dictionary<string, string>();
                    for (i = 0; i < NumFilesToCreate; i++)
                    {
                        filenames[i] = Path.Combine(DirToZip, String.Format("file{0:D3}.txt", i));
                        int sz = _rnd.Next(22000) + 3000;
                        //int sz = 1000;
                        var repeatedLine = String.Format("Line to Repeat... {0} {1} {2} filename: {3}", i, k, j, filenames[i]);
                        TestUtilities.CreateAndFillFileText(filenames[i], repeatedLine, sz);
                        string key = Path.GetFileName(filenames[i]);
                        checksums.Add(key, TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filenames[i])));
                        TestContext.WriteLine("  chk[{0}]={1}", key, checksums[key]);
                    }

                    TestContext.WriteLine("\n---------------------adding files to the archive...");

                    var sw = new StringWriter();
                    using (ZipFile zip = new ZipFile(ZipFileToCreate, sw))
                    {
                        zip.CompressionLevel = compressionLevelOptions[k];
                        zip.Password = Passwords[j];
                        zip.AddDirectory(Path.GetFileName(DirToZip));
                        zip.Save();
                    }
                    TestContext.WriteLine(sw.ToString());

                    Assert.AreEqual<int>(TestUtilities.CountEntries(ZipFileToCreate), NumFilesToCreate,
                            "The Zip file has an unexpected number of entries.");

                    TestContext.WriteLine("\n---------------------verifying checksums...");

                    using (ZipFile zip = ZipFile.Read(ZipFileToCreate))
                    {
                        foreach (ZipEntry e in zip)
                            TestContext.WriteLine("found entry: {0}", e.FileName);

                        var extractDir = String.Format("extract-{0}-{1}", k, j);
                        TestContext.WriteLine("  Extract with pw({0})", Passwords[j]);
                        foreach (ZipEntry e in zip)
                        {
                            e.ExtractWithPassword(extractDir, ExtractExistingFileAction.OverwriteSilently, Passwords[j]);
                            if (!e.IsDirectory)
                            {
                                byte[] c2 = TestUtilities.ComputeChecksum(Path.Combine(extractDir, e.FileName));
                                Assert.AreEqual<string>(checksums[e.FileName],
                                        TestUtilities.CheckSumToString(c2), "The checksum of the extracted file is incorrect.");
                            }
                        }
                    }
                    TestContext.WriteLine("\n");
                }
            }
        }



        [TestMethod]
        public void Password_MultipleEntriesDifferentPasswords()
        {
            string ZipFileToCreate = Path.Combine(TopLevelDir, "Password_MultipleEntriesDifferentPasswords.zip");
            Assert.IsFalse(File.Exists(ZipFileToCreate), "The temporary zip file '{0}' already exists.", ZipFileToCreate);

            string SourceDir = CurrentDir;
            for (int i = 0; i < 3; i++)
                SourceDir = Path.GetDirectoryName(SourceDir);

            Directory.SetCurrentDirectory(TopLevelDir);

            string[] filenames = 
            {
                Path.Combine(SourceDir, "Tools\\Zipit\\bin\\Debug\\Zipit.exe"),
                Path.Combine(SourceDir, "Zip Partial DLL\\bin\\Debug\\Ionic.Zip.Partial.xml"),
            };

            string[] checksums = 
            {
                TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filenames[0])),
                TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filenames[1])),
            };

            string[] passwords = 
            {
                    "12345678",
                    "0987654321",
            };

            int j = 0;
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    zip.Password = passwords[j];
                    zip.AddFile(filenames[j], "");
                }
                zip.Save();
            }

            Assert.AreEqual<int>(TestUtilities.CountEntries(ZipFileToCreate), filenames.Length,
                    "The zip file created has the wrong number of entries.");

            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    zip[Path.GetFileName(filenames[j])].ExtractWithPassword("unpack", ExtractExistingFileAction.OverwriteSilently, passwords[j]);
                    string newpath = Path.Combine("unpack", filenames[j]);
                    string chk = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(newpath));
                    Assert.AreEqual<string>(checksums[j], chk, "File checksums do not match.");
                }
            }
        }


        [TestMethod]
        [ExpectedException(typeof(Ionic.Zip.BadPasswordException))]
        public void Password_Extract_WrongPassword()
        {
            string ZipFileToCreate = Path.Combine(TopLevelDir, "MultipleEntriesDifferentPasswords.zip");
            Assert.IsFalse(File.Exists(ZipFileToCreate), "The temporary zip file '{0}' already exists.", ZipFileToCreate);

            string SourceDir = CurrentDir;
            for (int i = 0; i < 3; i++)
                SourceDir = Path.GetDirectoryName(SourceDir);

            Directory.SetCurrentDirectory(TopLevelDir);

            string[] filenames = 
            {
                Path.Combine(SourceDir, "Tools\\Zipit\\bin\\Debug\\Zipit.exe"),
                Path.Combine(SourceDir, "Zip Full DLL\\bin\\Debug\\Ionic.Zip.xml"),
            };

            string[] passwords = 
            {
                    "12345678",
                    "0987654321",
            };

            int j = 0;
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    zip.Password = passwords[j];
                    zip.AddFile(filenames[j], "");
                }
                zip.Save();
            }

            // now try to extract
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                    zip[Path.GetFileName(filenames[j])].ExtractWithPassword("unpack", ExtractExistingFileAction.OverwriteSilently, "WrongPassword");
            }
        }


        [TestMethod]
        public void Password_AddEntryWithPasswordToExistingZip()
        {
            string ZipFileToCreate = Path.Combine(TopLevelDir, "Password_AddEntryWithPasswordToExistingZip.zip");
            Assert.IsFalse(File.Exists(ZipFileToCreate), "The temporary zip file '{0}' already exists.", ZipFileToCreate);

            string DirToZip = Path.Combine(TopLevelDir, "zipthis");
            Directory.CreateDirectory(DirToZip);

            string SourceDir = CurrentDir;
            for (int i = 0; i < 3; i++)
                SourceDir = Path.GetDirectoryName(SourceDir);

            Directory.SetCurrentDirectory(TopLevelDir);

            string[] filenames = 
            {
                Path.Combine(SourceDir, "Tools\\Zipit\\bin\\Debug\\Zipit.exe"),
                Path.Combine(SourceDir, "Zip Partial DLL\\bin\\Debug\\Ionic.Zip.Partial.xml"),
            };

            string[] checksums = 
            {
                TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filenames[0])),
                TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(filenames[1])),
            };

            int j = 0;
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                    zip.AddFile(filenames[j], "");
                zip.Save();
            }

            Assert.AreEqual<int>(TestUtilities.CountEntries(ZipFileToCreate), 2,
                    "The Zip file has the wrong number of entries.");

            string fileX = Path.Combine(SourceDir, "Tools\\Unzip\\bin\\debug\\unzip.exe");
            string checksumX = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(fileX));
            string Password = "qw3sjknm!";
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                zip.Password = Password;
                zip.AddFile(fileX, "");
                zip.Save();
            }

            Assert.AreEqual<int>(TestUtilities.CountEntries(ZipFileToCreate), 3,
                    "The zip file created has the wrong number of entries.");

            string newpath, chk;
            using (ZipFile zip = new ZipFile(ZipFileToCreate))
            {
                for (j = 0; j < filenames.Length; j++)
                {
                    zip[Path.GetFileName(filenames[j])].Extract("unpack", ExtractExistingFileAction.OverwriteSilently);
                    newpath = Path.Combine(Path.Combine(TopLevelDir, "unpack"), filenames[j]);
                    chk = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(newpath));
                    Assert.AreEqual<string>(checksums[j], chk, "Checksums do not match.");
                }

                zip[Path.GetFileName(fileX)].ExtractWithPassword("unpack", ExtractExistingFileAction.OverwriteSilently, Password);
                newpath = Path.Combine(Path.Combine(TopLevelDir, "unpack"), fileX);
                chk = TestUtilities.CheckSumToString(TestUtilities.ComputeChecksum(newpath));
                Assert.AreEqual<string>(checksumX, chk, "Checksums for encrypted entry do not match.");
            }
        }

    }

}
