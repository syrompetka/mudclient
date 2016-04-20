// UnicodeTests.cs
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
// Time-stamp: <2010-February-10 15:16:40>
//
// ------------------------------------------------------------------
//
// This module defines the tests for the Unicode features in DotNetZip.
//
// ------------------------------------------------------------------

﻿
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;
using System.IO;

namespace Ionic.Zip.Tests.Unicode
{
    /// <summary>
    /// Summary description for UnicodeTests
    /// </summary>
    [TestClass]
    public class UnicodeTests : IonicTestClass
    {
        public UnicodeTests() : base() { }


        [TestMethod]
        public void Create_UnicodeEntries()
        {
            int i;
            string OrigComment = "This is a Unicode comment. Chinese: 弹 出 应 用 程 序 Norwegian/Danish: æøåÆØÅ. Portugese: Configurações.";
            string[] formats = {"弹出应用程序{0:D3}.bin",
                                   "n.æøåÆØÅ{0:D3}.bin",
                               "Configurações-弹出-ÆØÅ-xx{0:D3}.bin"};

            for (int k = 0; k < formats.Length; k++)
            {
                // create the subdirectory
                string Subdir = Path.Combine(TopLevelDir, "files" + k);
                Directory.CreateDirectory(Subdir);

                // create a bunch of files
                int NumFilesToCreate = _rnd.Next(8) + 4;
                string[] FilesToZip = new string[NumFilesToCreate];
                for (i = 0; i < NumFilesToCreate; i++)
                {
                    FilesToZip[i] = Path.Combine(Subdir, String.Format(formats[k], i));
                    TestUtilities.CreateAndFillFileBinary(FilesToZip[i], _rnd.Next(5000) + 2000);
                }

                // create a zipfile twice, once using Unicode, once without
                for (int j = 0; j < 2; j++)
                {
                    // select the name of the zip file
                    string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_UnicodeEntries_{0}_{1}.zip", k, j));
                    Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

                    TestContext.WriteLine("\n\nFormat {0}, trial {1}.  filename: {2}...", k, j, zipFileToCreate);
                    string dirInArchive = String.Format("{0}-{1}", Path.GetFileName(Subdir), j);

                    using (ZipFile zip1 = new ZipFile())
                    {
                        zip1.UseUnicodeAsNecessary = (j == 0);
                        for (i = 0; i < FilesToZip.Length; i++)
                        {
                            // use the local filename (not fully qualified)
                            ZipEntry e = zip1.AddFile(FilesToZip[i], dirInArchive);
                            e.Comment = String.Format("This entry encoded with {0}", (j == 0) ? "unicode" : "the default code page.");
                        }
                        zip1.Comment = OrigComment;
                        zip1.Save(zipFileToCreate);
                    }

                    // Verify the number of files in the zip
                    Assert.AreEqual<int>(TestUtilities.CountEntries(zipFileToCreate), FilesToZip.Length,
                            "Incorrect number of entries in the zip file.");

                    i = 0;

                    // verify the filenames are (or are not) unicode

                    var options = new ReadOptions {
                            Encoding = (j == 0) ? System.Text.Encoding.UTF8 : ZipFile.DefaultEncoding
                    };
                    using (ZipFile zip2 = ZipFile.Read(zipFileToCreate, options))
                    {
                        foreach (ZipEntry e in zip2)
                        {
                            string fname = String.Format(formats[k], i);
                            if (j == 0)
                            {
                                Assert.AreEqual<String>(fname, Path.GetFileName(e.FileName));
                            }
                            else
                            {
                                Assert.AreNotEqual<String>(fname, Path.GetFileName(e.FileName));
                            }
                            i++;
                        }


                        // according to the spec,
                        // unicode is not supported on the zip archive comment!
                        // But this library won't enforce that.
                        // We will leave it up to the application.
                        // Assert.AreNotEqual<String>(OrigComment, zip2.Comment);

                    }
                }
            }
        }

        [TestMethod]
        public void Create_UnicodeEntries_Mixed()
        {
            int i;
            string[] formats = { "file{0:D3}.bin",         // keep this at index==0
                                 "弹出应用程序{0:D3}.bin",   // Chinese
                                 "codeplexの更新RSSを見てふと書いた投稿だったけど日本語情報がないかは調{0:D3}.bin", // Japanese
                                "n.æøåÆØÅ{0:D3}.bin",      // greek
                                "Configurações-弹出-ÆØÅ-xx{0:D3}.bin",  // portugese + Chinese
                                "Â¡¢£ ¥â° €Ãƒ †œ Ñ añoAbba{0:D3.bin}",   //??
                                "А Б В Г Д Є Ж Ѕ З И І К Л М Н О П Р С Т Ф Х Ц Ч Ш Щ Ъ ЪІ Ь Ю ІА {0:D3}.b", // Russian
                               "Ελληνικό αλφάβητο {0:D3}.b",
                                "א ב ג ד ה ו ז ח ט י " + "{0:D3}",  // I don't know what this is
                               };

            // create the subdirectory
            string Subdir = Path.Combine(TopLevelDir, "files");
            Directory.CreateDirectory(Subdir);

            // create a bunch of files
            int NumFilesToCreate = _rnd.Next(18) + 14;
            string[] FilesToZip = new string[NumFilesToCreate];
            for (i = 0; i < NumFilesToCreate; i++)
            {
                int k = i % formats.Length;
                FilesToZip[i] = Path.Combine(Subdir, String.Format(formats[k], i));
                TestUtilities.CreateAndFillFileBinary(FilesToZip[i], _rnd.Next(5000) + 2000);
            }

            Directory.SetCurrentDirectory(Subdir);

            // create a zipfile twice
            for (int j = 0; j < 2; j++)
            {
                // select the name of the zip file
                string ZipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_UnicodeEntries_Mixed{0}.zip", j));
                Assert.IsFalse(File.Exists(ZipFileToCreate), "The zip file '{0}' already exists.", ZipFileToCreate);

                using (ZipFile zip1 = new ZipFile(ZipFileToCreate))
                {
                    zip1.UseUnicodeAsNecessary = (j == 0);
                    for (i = 0; i < FilesToZip.Length; i++)
                    {
                        // use the local filename (not fully qualified)
                        ZipEntry e = zip1.AddFile(Path.GetFileName(FilesToZip[i]));
                    }
                    zip1.Save();
                }

                // Verify the number of files in the zip
                Assert.AreEqual<int>(TestUtilities.CountEntries(ZipFileToCreate), FilesToZip.Length,
                        "Incorrect number of entries in the zip file.");

                i = 0;
                // Verify that the filenames do, or do not, match the names that were added.
                // They will match if unicode was used (j==0) or if the filename used was the
                // first in the formats list (k==0).
                using (ZipFile zip2 = ZipFile.Read(ZipFileToCreate))
                {
                    foreach (ZipEntry e in zip2)
                    {
                        int k = i % formats.Length;
                        string fname = String.Format(formats[k], i);
                        if (j == 0 || k == 0)
                        {
                            Assert.AreEqual<String>(fname, e.FileName, "cycle ({0},{1},{2})", i, j, k);
                        }
                        else
                        {
                            Assert.AreNotEqual<String>(fname, e.FileName, "cycle ({0},{1},{2})", i, j, k);
                        }
                        i++;
                    }
                }
            }
        }


        struct CodepageTrial
        {
            public string codepage;
            public string filenameFormat;
            public bool exceptionExpected; // not all codepages will yield legal filenames for a given filenameFormat
            public CodepageTrial(string cp, string format, bool except)
            {
                codepage = cp;
                filenameFormat = format;
                exceptionExpected = except;
            }
        }

        [TestMethod]
        public void Create_WithSpecifiedCodepage()
        {
            int i;
            CodepageTrial[] trials = {
                                     new CodepageTrial( "big5",   "弹出应用程序{0:D3}.bin", true),
                                     new CodepageTrial ("big5",   "您好{0:D3}.bin",         false),
                                     new CodepageTrial ("gb2312", "弹出应用程序{0:D3}.bin", false),
                                     new CodepageTrial ("gb2312", "您好{0:D3}.bin",         false),
                                     // insert other languages here.??
                                     };

            for (int k = 0; k < trials.Length; k++)
            {
                TestContext.WriteLine("\n---------------------Trial {0}....", k);
                TestContext.WriteLine("\n---------------------codepage: {0}....", trials[k].codepage);
                // create the subdirectory
                string subdir = Path.Combine(TopLevelDir, String.Format("trial{0}-files", k));
                Directory.CreateDirectory(subdir);

                // create a bunch of files
                int numFiles = _rnd.Next(3) + 3;
                string[] filesToZip = new string[numFiles];
                for (i = 0; i < numFiles; i++)
                {
                    filesToZip[i] = Path.Combine(subdir, String.Format(trials[k].filenameFormat, i));
                    TestUtilities.CreateAndFillFileBinary(filesToZip[i], _rnd.Next(5000) + 2000);
                }

                Directory.SetCurrentDirectory(subdir);

                // select the name of the zip file
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Create_WithSpecifiedCodepage_{0}_{1}.zip", k, trials[k].codepage));
                Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

                TestContext.WriteLine("\n---------------------Creating zip....");

                using (ZipFile zip1 = new ZipFile(zipFileToCreate))
                {
                    zip1.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding(trials[k].codepage);
                    for (i = 0; i < filesToZip.Length; i++)
                    {
                        TestContext.WriteLine("adding entry {0}", filesToZip[i]);
                        // use the local filename (not fully qualified)
                        ZipEntry e = zip1.AddFile(Path.GetFileName(filesToZip[i]));
                        e.Comment = String.Format("This entry was encoded in the {0} codepage", trials[k].codepage);
                    }
                    zip1.Save();
                }

                TestContext.WriteLine("\n---------------------Extracting....");
                Directory.SetCurrentDirectory(TopLevelDir);

                try
                {

                    // verify the filenames are (or are not) unicode
                    var options = new ReadOptions {
                            Encoding = System.Text.Encoding.GetEncoding(trials[k].codepage)
                    };
                    using (ZipFile zip2 = ZipFile.Read(zipFileToCreate, options))
                    {
                        foreach (ZipEntry e in zip2)
                        {
                            TestContext.WriteLine("found entry {0}", e.FileName);
                            e.Extract(String.Format("trial{0}-{1}-extract", k, trials[k].codepage));
                        }
                    }
                }
                catch (Exception e1)
                {
                    if (!trials[k].exceptionExpected)
                        throw new System.Exception("while extracting", e1);

                }
            }

            TestContext.WriteLine("\n---------------------Done.");
        }


        [TestMethod]
        public void CodePage_UpdateZip_AlternateEncoding_wi10180()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "UpdateZip_AlternateEncoding_wi10180.zip");
            System.Text.Encoding JIS = System.Text.Encoding.GetEncoding("shift_jis");

            TestContext.WriteLine("The CP for JIS is: {0}", JIS.CodePage);

            string[] filenames = {
                "日本語.txt",
                "日本語テスト.txt"
            };

            // pass 1 - create it
            TestContext.WriteLine("Create zip...");
            using (var zip = new ZipFile())
            {
                zip.ProvisionalAlternateEncoding = JIS;
                zip.AddEntry(filenames[0], "This is the content for entry (" + filenames[0] + ")");
                TestContext.WriteLine("adding file: {0}", filenames[0]);
                zip.Save(zipFileToCreate);
            }

            // pass 2 - update it
            TestContext.WriteLine("Update zip...");
            ReadOptions options = new ReadOptions { Encoding = JIS };
            using (var zip0 = ZipFile.Read(zipFileToCreate, options))
            {
                foreach (var e in zip0)
                {
                    TestContext.WriteLine("existing entry name: {0}  encoding: {1}",
                                          e.FileName, e.ProvisionalAlternateEncoding.EncodingName
                                          );
                }
                zip0.AddEntry(filenames[1], "This is more content..." + System.DateTime.UtcNow.ToString("G"));
                TestContext.WriteLine("adding file: {0}", filenames[1]);
                zip0.Save();
            }


            // pass 3 - verify the filenames
            TestContext.WriteLine("Verify zip...");
            using (var zip0 = ZipFile.Read(zipFileToCreate, options))
            {
                foreach (string f in filenames)
                {
                    Assert.AreEqual<string>(f, zip0[f].FileName, "The entry FileName was not expected");
                }
            }
        }



        [TestMethod]
        public void Unicode_AddDirectoryByName_wi8984()
        {
            Directory.SetCurrentDirectory(TopLevelDir);
            string format = "弹出应用程序{0:D3}.dir"; // Chinese characters

            for (int n = 1; n <= 10; n++)
            {
                var dirsAdded = new System.Collections.Generic.List<String>();
                string zipFileToCreate = Path.Combine(TopLevelDir, String.Format("Test_AddDirectoryByName{0:N2}.zip", n));
                using (ZipFile zip1 = new ZipFile(zipFileToCreate))
                {
                    zip1.UseUnicodeAsNecessary = true;
                    for (int i = 0; i < n; i++)
                    {
                        // create an arbitrary directory name, add it to the zip archive
                        string dirName = String.Format(format, i);
                        zip1.AddDirectoryByName(dirName);
                        dirsAdded.Add(dirName + "/");
                    }
                    zip1.Save();
                }


                string extractDir = String.Format("extract{0:D3}", n);
                int dirCount = 0;
                using (ZipFile zip2 = ZipFile.Read(zipFileToCreate))
                {
                    foreach (var e in zip2)
                    {
                        TestContext.WriteLine("dir: {0}", e.FileName);
                        Assert.IsTrue(dirsAdded.Contains(e.FileName), "Cannot find the expected entry ({0})", e.FileName);
                        Assert.IsTrue(e.IsDirectory);
                        e.Extract(extractDir);
                        dirCount++;
                    }
                }
                Assert.AreEqual<int>(n, dirCount);
            }
        }




    }
}
