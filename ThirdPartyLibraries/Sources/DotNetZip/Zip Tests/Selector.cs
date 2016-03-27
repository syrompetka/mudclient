// Selector.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009-2010 Dino Chiesa.
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
// Time-stamp: <2010-February-12 17:55:33>
//
// ------------------------------------------------------------------
//
// This module defines tests for the File and Entry Selection stuff in
// DotNetZip.
//
// ------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ionic.Zip;
using Ionic.Zip.Tests.Utilities;
using System.IO;

namespace Ionic.Zip.Tests
{
    /// <summary>
    /// Summary description for Selector
    /// </summary>
    [TestClass]
    public class Selector : IonicTestClass
    {
        public Selector() : base() { }

        Ionic.CopyData.Transceiver _txrx;


        [ClassInitialize]
        public static void ClassInit(TestContext a)
        {
            CurrentDir = Directory.GetCurrentDirectory();
            var txrx= SetupProgressMonitor("selector-Setup");
            _InternalSetupFiles(txrx);
            txrx.Send("stop");
        }


        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            CleanDirectory(fodderDirectory, null);
        }



        private static void CleanDirectory(string dirToClean, Ionic.CopyData.Transceiver txrx)
        {
            if (dirToClean == null) return;

            if (!Directory.Exists(dirToClean)) return;

            var dirs = Directory.GetDirectories(dirToClean, "*.*", SearchOption.AllDirectories);

            if (txrx!=null)
                txrx.Send("pb 1 max " + dirs.Length.ToString());

            foreach (var d in dirs)
            {
                CleanDirectory(d, txrx);
                if (txrx!=null)
                    txrx.Send("pb 1 step");
            }

            // Some of the files are marked as ReadOnly/System, and
            // before deleting the dir we must strip those attrs.
            var files = Directory.GetFiles(dirToClean, "*.*", SearchOption.AllDirectories);
            if (txrx!=null)
                txrx.Send("pb 1 max " + files.Length.ToString());

            foreach (var f in files)
            {
                var a = File.GetAttributes(f);
                // must do ReadOnly bit first - to allow setting other bits.
                if ((a & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    a &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(f, a);
                }
                if (((a & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                    ((a & FileAttributes.System) == FileAttributes.System))
                {
                    a &= ~FileAttributes.Hidden;
                    a &= ~FileAttributes.System;
                    File.SetAttributes(f, a);
                }
                File.Delete(f);
                if (txrx!=null)
                   txrx.Send("pb 1 step");
            }

            // Delete the directory with delay and retry.
            // Sometimes I have a console window in the directory
            // and I want it to not give up so easily.
            int tries =0;
            bool success = false;
            do
            {
                try
                {
                    Directory.Delete(dirToClean, true);
                    success = true;
                }
                catch
                {
                    System.Threading.Thread.Sleep(600);
                }
                tries++;
            } while (tries < 100 && !success);
        }



        [TestCleanup]
        public void MyTestCleanupEx()
        {
            if (_txrx != null)
            {
                try
                {
                    _txrx.Send("stop");
                    _txrx = null;
                }
                catch { }
            }
        }


        [TestMethod]
        public void Selector_EdgeCases()
        {
            string Subdir = Path.Combine(TopLevelDir, "A");

            Ionic.FileSelector ff = new Ionic.FileSelector("name = *.txt");
            var list = ff.SelectFiles(Subdir);

            ff.SelectionCriteria = "name = *.bin";
            list = ff.SelectFiles(Subdir);
        }



        private static DateTime twentyPlusDaysAgo;
        private static DateTime today;
        private static DateTime tomorrow;
        private static DateTime threeDaysAgo;
        private static DateTime yesterday;
        private static string fodderDirectory;

        private Object LOCK = new Object();
        private int numFodderFiles, numFodderDirs;

        private string SetupFiles()
        {
            lock (LOCK)
            {
                if (numFodderFiles <= 0)
                {
                    var fodderFiles = Directory.GetFiles(fodderDirectory, "*.*", SearchOption.AllDirectories);
                    numFodderFiles = fodderFiles.Length;
                    var fodderDirs = Directory.GetDirectories(fodderDirectory, "*.*", SearchOption.AllDirectories);
                    numFodderDirs = fodderDirs.Length;
                }

                if (numFodderFiles <= 0)
                    _InternalSetupFiles(_txrx);

                if (numFodderFiles <= 0)
                    throw new Exception();

                return fodderDirectory;
            }
        }


        private static void DeleteOldFodderDirectories( Ionic.CopyData.Transceiver txrx )
        {
            // Before creating the directory for the current run, Remove old directories.
            // For some reason the test cleanup code tends to leave these directories??
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            var oldDirs = Directory.GetDirectories(tempDir, "*.SelectorTests");
            if (oldDirs.Length > 0)
            {
                if (txrx != null)
                {
                    txrx.Send("status deleting old directories...");
                    txrx.Send(String.Format("pb 0 max {0}", oldDirs.Length));
                }

                foreach (var dir in oldDirs)
                {
                    CleanDirectory(dir, txrx);
                    if (txrx != null) txrx.Send("pb 0 step");
                }
            }
        }



        private static void _InternalSetupFiles( Ionic.CopyData.Transceiver txrx )
        {
            var rnd = new System.Random();
            DeleteOldFodderDirectories(txrx);

            int fileCount = rnd.Next(95) + 95;
            if (txrx != null)
            {
                txrx.Send("status creating files...");
                txrx.Send(String.Format("pb 0 max {0}", fileCount));
            }

            fodderDirectory = TestUtilities.GenerateUniquePathname("SelectorTests");

            // remember this directory so we can restore later
            string originalDir = Directory.GetCurrentDirectory();

            int entriesAdded = 0;

            // get the base directory for tests:
            Directory.SetCurrentDirectory(CurrentDir);
            Directory.CreateDirectory(fodderDirectory);
            Directory.SetCurrentDirectory(fodderDirectory);

            twentyPlusDaysAgo = DateTime.Now - new TimeSpan(20, 12, 13, 14);
            today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            tomorrow = today + new TimeSpan(1, 0, 0, 0);
            threeDaysAgo = today - new TimeSpan(3, 0, 0, 0);
            yesterday = today - new TimeSpan(1, 0, 0, 0);

            string[] nameFormats =
                {
                    "file{0:D3}",
                    "{0:D3}",
                    "PrettyLongFileName-{0:D3}",
                    "Extremely-Long-Filename-{0:D3}-with-a-repeated-segment-{0:D3}-{0:D3}-{0:D3}-{0:D3}",

                };

            string[] dirs =
                {
                    "dir1",
                    "dir1\\dirA",
                    "dir1\\dirB",
                    "dir2"
                };


            foreach (string s in dirs)
                Directory.CreateDirectory(s);

            for (int j = 0; j < fileCount; j++)
            {
                // select the size
                int sz = 0;
                if (j % 5 == 0) sz = rnd.Next(15000) + 150000;
                else if (j % 17 == 1) sz = rnd.Next(50 * 1024) + 1024 * 1024;
                else if (rnd.Next(13) == 0) sz = 8080; // exactly
                else sz = rnd.Next(5000) + 5000;

                // randomly select the format of the file name
                int n = rnd.Next(4);

                // binary or text
                string filename = null;
                if (rnd.Next(2) == 0)
                {
                    filename = Path.Combine(fodderDirectory, String.Format(nameFormats[n], j) + ".txt");
                    TestUtilities.CreateAndFillFileText(filename, sz);
                }
                else
                {
                    filename = Path.Combine(fodderDirectory, String.Format(nameFormats[n], j) + ".bin");
                    TestUtilities.CreateAndFillFileBinary(filename, sz);
                }

                // select whether to backdate mtime or not
                if (rnd.Next(2) == 0)
                    TouchFile(filename, WhichTime.mtime, twentyPlusDaysAgo);

                // select whether to backdate ctime or not
                if (rnd.Next(2) == 0)
                    TouchFile(filename, WhichTime.ctime, threeDaysAgo);

                // select whether to backdate atime or not
                if (rnd.Next(2) == 0)
                    TouchFile(filename, WhichTime.atime, yesterday);

                // set the last mod time to "a long time ago" on 1/14th of the files
                if (j % 14 == 0)
                {
                    DateTime x = new DateTime(1998, 4, 29);
                    File.SetLastWriteTime(filename, x);
                }

                // maybe move to a subdir
                n = rnd.Next(6);
                if (n < 4)
                {
                    string newFilename = Path.Combine(dirs[n], Path.GetFileName(filename));
                    File.Move(filename, newFilename);
                    filename = newFilename;
                }

                // mark some of the files as hidden, system, readonly, etc
                if (j % 9 == 0)
                    File.SetAttributes(filename, FileAttributes.Hidden);
                if (j % 14 == 0)
                    File.SetAttributes(filename, FileAttributes.ReadOnly);
                if (j % 13 == 0)
                    File.SetAttributes(filename, FileAttributes.System);
                if (j % 11 == 0)
                    File.SetAttributes(filename, FileAttributes.Archive);

                entriesAdded++;

                if (txrx != null)
                {
                    txrx.Send("pb 0 step");
                    if (entriesAdded % 8 == 0)
                        txrx.Send(String.Format("status creating files ({0}/{1})", entriesAdded, fileCount));
                }
            }
            // restore the cwd
            Directory.SetCurrentDirectory(originalDir);
        }



        class Trial
        {
            public string Label;
            public string C1;
            public string C2;
        }




        [TestMethod]
        public void Selector_SelectFiles()
        {
            Directory.SetCurrentDirectory(TopLevelDir);

            Trial[] trials = new Trial[]
                {
                    new Trial { Label = "name", C1 = "name = *.txt", C2 = "name = *.bin" },
                    new Trial { Label = "name (shorthand)", C1 = "*.txt", C2 = "*.bin" },
                    new Trial { Label = "size", C1 = "size < 7500", C2 = "size >= 7500" },
                    new Trial { Label = "size", C1 = "size = 8080", C2 = "size != 8080" },
                    new Trial { Label = "name & size",
                        C1 = "name = *.bin AND size > 7500",
                        C2 = "name != *.bin  OR  size <= 7500",
                        },
                    new Trial { Label = "name XOR name",
                        C1 = "name = *.bin XOR name = *4.*",
                        C2 = "(name != *.bin OR name = *4.*) AND (name = *.bin OR name != *4.*)",
                        },
                    new Trial { Label = "name XOR size",
                        C1 = "name = *.bin XOR size > 100k",
                        C2 = "(name != *.bin OR size > 100k) AND (name = *.bin OR size <= 100k)",
                        },
                    new Trial
                    {
                        Label = "mtime",
                        C1 = String.Format("mtime < {0}", twentyPlusDaysAgo.ToString("yyyy-MM-dd")),
                        C2 = String.Format("mtime >= {0}", twentyPlusDaysAgo.ToString("yyyy-MM-dd")),
                        },
                    new Trial
                    {
                        Label = "ctime",
                        C1 = String.Format("mtime < {0}", threeDaysAgo.ToString("yyyy-MM-dd")),
                        C2 = String.Format("mtime >= {0}", threeDaysAgo.ToString("yyyy-MM-dd")),
                        },
                    new Trial
                    {
                        Label = "atime",
                        C1 = String.Format("mtime < {0}", yesterday.ToString("yyyy-MM-dd")),
                        C2 = String.Format("mtime >= {0}", yesterday.ToString("yyyy-MM-dd")),
                        },
                    new Trial { Label = "size (100k)", C1="size > 100k", C2="size <= 100kb", },
                    new Trial { Label = "size (1mb)", C1="size > 1m", C2="size <= 1mb", },
                    new Trial { Label = "size (1gb)", C1="size > 1g", C2="size <= 1gb", },
                    new Trial { Label = "attributes (Hidden)", C1 = "attributes = H", C2 = "attributes != H" },
                    new Trial { Label = "attributes (ReadOnly)", C1 = "attributes = R", C2 = "attributes != R" },
                    new Trial { Label = "attributes (System)", C1 = "attributes = S", C2 = "attributes != S" },
                    new Trial { Label = "attributes (Archive)", C1 = "attributes = A", C2 = "attributes != A" },

                };


            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectFiles.zip");
            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            int count1, count2;
            //String filename = null;

            SetupFiles();
            var topLevelFiles = Directory.GetFiles(fodderDirectory, "*.*", SearchOption.TopDirectoryOnly);

            for (int m = 0; m < trials.Length; m++)
            {
                Ionic.FileSelector ff = new Ionic.FileSelector(trials[m].C1);
                var list = ff.SelectFiles(fodderDirectory);
                TestContext.WriteLine("=======================================================");
                TestContext.WriteLine("Selector: " + ff.ToString());
                TestContext.WriteLine("Criteria({0})", ff.SelectionCriteria);
                TestContext.WriteLine("Count({0})", list.Count);
                count1 = 0;
                foreach (string s in list)
                {
                    switch (m)
                    {
                        case 0:
                        case 1:
                            Assert.IsTrue(s.EndsWith(".txt"));
                            break;
                        case 2:
                            {
                                FileInfo fi = new FileInfo(s);
                                Assert.IsTrue(fi.Length < 7500);
                            }
                            break;
                        case 4:
                            {
                                FileInfo fi = new FileInfo(s);
                                bool x = s.EndsWith(".bin") && fi.Length > 7500;
                                Assert.IsTrue(x);
                            }
                            break;
                    }
                    count1++;
                }

                ff = new Ionic.FileSelector(trials[m].C2);
                list = ff.SelectFiles(fodderDirectory);
                TestContext.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                TestContext.WriteLine("Criteria({0})", ff.SelectionCriteria);
                TestContext.WriteLine("Count({0})", list.Count);
                count2 = 0;
                foreach (string s in list)
                {
                    switch (m)
                    {
                        case 0:
                        case 1:
                            Assert.IsTrue(s.EndsWith(".bin"));
                            break;
                        case 2:
                            {
                                FileInfo fi = new FileInfo(s);
                                Assert.IsTrue(fi.Length >= 7500);
                            }
                            break;
                        case 4:
                            {
                                FileInfo fi = new FileInfo(s);
                                bool x = !s.EndsWith(".bin") || fi.Length <= 7500;
                                Assert.IsTrue(x);
                            }
                            break;
                    }
                    count2++;
                }
                Assert.AreEqual<Int32>(topLevelFiles.Length, count1 + count2);
            }
        }




        private static Ionic.CopyData.Transceiver SetupProgressMonitor(string label)
        {
            string testBin = TestUtilities.GetTestBinDir(CurrentDir);
            string progressMonitorTool = Path.Combine(testBin, "Resources\\UnitTestProgressMonitor.exe");
            string requiredDll = Path.Combine(testBin, "Resources\\Ionic.CopyData.dll");

            Assert.IsTrue(File.Exists(progressMonitorTool), "progress monitor tool does not exist ({0})", progressMonitorTool);
            Assert.IsTrue(File.Exists(requiredDll), "required DLL does not exist ({0})", requiredDll);

            string progressChannel = label;
            // start the progress monitor
            string ignored;
            TestUtilities.Exec_NoContext(progressMonitorTool, String.Format("-channel {0}", progressChannel), false,
                                         out ignored);

            System.Threading.Thread.Sleep(800);
            Ionic.CopyData.Transceiver txrx = new Ionic.CopyData.Transceiver();

            txrx.Channel = progressChannel;
            txrx.Send("test " + label);
            System.Threading.Thread.Sleep(120);
            txrx.Send(String.Format("pb 0 max {0}", 3));
            return txrx;
        }




        [TestMethod, Timeout(7200000)]
        public void Selector_AddSelectedFiles()
        {
            Directory.SetCurrentDirectory(TopLevelDir);

            Trial[] trials = new Trial[]
                {
                    new Trial { Label = "name", C1 = "name = *.txt", C2 = "name = *.bin" },
                    new Trial { Label = "name (shorthand)", C1 = "*.txt", C2 = "*.bin" },
                    new Trial { Label = "attributes (Hidden)", C1 = "attributes = H", C2 = "attributes != H" },
                    new Trial { Label = "attributes (ReadOnly)", C1 = "attributes = R", C2 = "attributes != R" },
                    new Trial { Label = "mtime", C1 = "mtime < 2007-01-01", C2 = "mtime > 2007-01-01" },
                    new Trial { Label = "atime", C1 = "atime < 2007-01-01", C2 = "atime > 2007-01-01" },
                    new Trial { Label = "ctime", C1 = "ctime < 2007-01-01", C2 = "ctime > 2007-01-01" },
                    new Trial { Label = "size", C1 = "size < 7500", C2 = "size >= 7500" },

                    new Trial { Label = "name & size",
                        C1 = "name = *.bin AND size > 7500",
                        C2 = "name != *.bin  OR  size <= 7500",
                        },

                    new Trial { Label = "name, size & attributes",
                        C1 = "name = *.bin AND size > 8kb and attributes = H",
                        C2 = "name != *.bin  OR  size <= 8kb or attributes != H",
                        },

                    new Trial { Label = "name, size, time & attributes.",
                        C1 = "name = *.bin AND size > 7k and mtime < 2007-01-01 and attributes = H",
                        C2 = "name != *.bin  OR  size <= 7k or mtime > 2007-01-01 or attributes != H",
                        },
                };

            _txrx= SetupProgressMonitor("AddSelectedFiles");

            string[] zipFileToCreate = {
                Path.Combine(TopLevelDir, "Selector_AddSelectedFiles-1.zip"),
                Path.Combine(TopLevelDir, "Selector_AddSelectedFiles-2.zip")
            };

            Assert.IsFalse(File.Exists(zipFileToCreate[0]), "The zip file '{0}' already exists.", zipFileToCreate[0]);
            Assert.IsFalse(File.Exists(zipFileToCreate[1]), "The zip file '{0}' already exists.", zipFileToCreate[1]);

            int count1, count2;

            _txrx.Send("test AddSelectedFiles");
            SetupFiles();
            var topLevelFiles = Directory.GetFiles(fodderDirectory, "*.*", SearchOption.TopDirectoryOnly);

            string currentDir = Directory.GetCurrentDirectory();
            _txrx.Send(String.Format("pb 0 max {0}", 2 * (trials.Length + 1)));

            _txrx.Send("pb 0 step");

            for (int m = 0; m < trials.Length; m++)
            {
                _txrx.Send("test AddSelectedFiles");
                _txrx.Send("pb 1 max 4");
                _txrx.Send(String.Format("status test {0}/{1}: creating zip #1/2",
                                        m + 1, trials.Length));
                TestContext.WriteLine("===============================================");
                TestContext.WriteLine("AddSelectedFiles() [{0}]", trials[m].Label);
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.AddSelectedFiles(trials[m].C1, fodderDirectory, "");
                    zip1.Save(zipFileToCreate[0]);
                }
                count1 = TestUtilities.CountEntries(zipFileToCreate[0]);
                TestContext.WriteLine("C1({0}) Count({1})", trials[m].C1, count1);
                _txrx.Send("pb 1 step");
                System.Threading.Thread.Sleep(100);
                _txrx.Send("pb 0 step");

                _txrx.Send(String.Format("status test {0}/{1}: creating zip #2/2",
                                        m + 1, trials.Length));
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.AddSelectedFiles(trials[m].C2, fodderDirectory, "");
                    zip1.Save(zipFileToCreate[1]);
                }
                count2 = TestUtilities.CountEntries(zipFileToCreate[1]);
                TestContext.WriteLine("C2({0}) Count({1})", trials[m].C2, count2);
                Assert.AreEqual<Int32>(topLevelFiles.Length, count1 + count2);
                _txrx.Send("pb 1 step");

                /// =======================================================
                /// Now, select entries from that ZIP
                _txrx.Send(String.Format("status test {0}/{1}: selecting zip #1/2",
                                        m + 1, trials.Length));
                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate[0]))
                {
                    var selected1 = zip1.SelectEntries(trials[m].C1);
                    Assert.AreEqual<Int32>(selected1.Count, count1);
                }
                _txrx.Send("pb 1 step");

                _txrx.Send(String.Format("status test {0}/{1}: selecting zip #2/2",
                                        m + 1, trials.Length));
                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate[1]))
                {
                    var selected2 = zip1.SelectEntries(trials[m].C2);
                    Assert.AreEqual<Int32>(selected2.Count, count2);
                }
                _txrx.Send("pb 1 step");

                _txrx.Send("pb 0 step");
            }

            _txrx.Send("stop");
        }


        [TestMethod]
        public void Selector_AddSelectedFiles_2()
        {
            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_AddSelectedFiles_2.zip");

            Directory.SetCurrentDirectory(TopLevelDir);
            string dirToZip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var files = TestUtilities.GenerateFilesFlat(dirToZip);
            var txtFiles = Directory.GetFiles(dirToZip, "*.txt", SearchOption.AllDirectories);

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddSelectedFiles("*.txt");
                zip1.Save(zipFileToCreate);
            }

            Assert.AreEqual<Int32>(0, TestUtilities.CountEntries(zipFileToCreate));

            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddSelectedFiles("*.txt", true);
                zip1.Save(zipFileToCreate);
            }

            Assert.AreEqual<Int32>(txtFiles.Length, TestUtilities.CountEntries(zipFileToCreate));
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddSelectedFiles("*.txt", ".", true);
                zip1.Save(zipFileToCreate);
            }

            Assert.AreEqual<Int32>(txtFiles.Length, TestUtilities.CountEntries(zipFileToCreate));


        }



        private enum WhichTime
        {
            atime,
            mtime,
            ctime,
        }



        private static void TouchFile(string strFile, WhichTime which, DateTime stamp)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(strFile);
            if (which == WhichTime.atime)
                fi.LastAccessTime = stamp;
            else if (which == WhichTime.ctime)
                fi.CreationTime = stamp;
            else if (which == WhichTime.mtime)
                fi.LastWriteTime = stamp;
            else throw new System.ArgumentException("which");
        }



        [TestMethod]
        public void Selector_SelectEntries_ByTime()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectEntries.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            _txrx= SetupProgressMonitor("SelectFiles-ByTime");
            SetupFiles();

            _txrx.Send("status seleting files by time...");

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(fodderDirectory, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(numFodderFiles, TestUtilities.CountEntries(zipFileToCreate), "A");

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, SelectEntries() by date...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                string crit = String.Format("mtime >= {0}", today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                TestContext.WriteLine("Criteria({0})", crit);
                var selected1 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected1.Count);

                crit = String.Format("mtime < {0}", today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                var selected2 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected2.Count);

                Assert.AreEqual<Int32>(numFodderFiles+numFodderDirs, selected1.Count + selected2.Count, "B");

                crit = String.Format("ctime >= {0}", threeDaysAgo.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                var selected3 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected3.Count);

                Assert.AreEqual<Int32>(numFodderFiles+numFodderDirs, selected3.Count, "C");

                // none of the files should be stamped as created between those two times
                crit = String.Format("ctime > {0}  and  ctime < {1}",
                                     threeDaysAgo.ToString("yyyy-MM-dd"),
                                     today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                var selected4 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected4.Count);
                Assert.AreEqual<Int32>(0, selected4.Count, "D");

                // those created 3 days ago, plus those created today = all entries
                crit = String.Format("ctime >= {0}  and  ctime < {1}",
                                     threeDaysAgo.ToString("yyyy-MM-dd"),
                                     today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                var selected5 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.IsTrue(selected5.Count > 0, "E");

                crit = String.Format("ctime >= {0}", today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                var selected6 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.AreEqual<Int32>(numFodderFiles+numFodderDirs, selected5.Count + selected6.Count, "F");

                // those accessed yesterday, plus those accessed today = all entries
                crit = String.Format("atime >= {0}  and  atime < {1}",
                                     yesterday.ToString("yyyy-MM-dd"),
                                     today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                selected5 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.IsTrue(selected5.Count > 0, "G");

                crit = String.Format("atime >= {0}",
                                     today.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                selected6 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.AreEqual<Int32>(numFodderFiles+numFodderDirs, selected5.Count + selected6.Count, "H");

                // those accessed *exactly* at midnight yesterday, plus those NOT = all entries
                crit = String.Format("atime = {0}",
                                     yesterday.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                selected5 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.IsTrue(selected5.Count > 0, "I");

                crit = String.Format("atime != {0}",
                                     yesterday.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                selected6 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.AreEqual<Int32>(numFodderFiles+numFodderDirs, selected5.Count + selected6.Count, "J");

                // those accessed three days ago or more == empty set
                crit = String.Format("atime <= {0}",
                                     threeDaysAgo.ToString("yyyy-MM-dd"));
                _txrx.Send("status " + crit);
                selected5 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected5.Count);
                Assert.AreEqual<Int32>(0, selected5.Count, "K");
            }
            _txrx.Send("stop");
        }



        [TestMethod]
        public void Selector_ExtractSelectedEntries()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_ExtractSelectedEntries.zip");

            SetupFiles();

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(fodderDirectory, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(numFodderFiles, TestUtilities.CountEntries(zipFileToCreate));

            string extractDir = "extract";

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, ExtractSelectedEntries() by date...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                string crit = String.Format("mtime >= {0}", today.ToString("yyyy-MM-dd"));
                TestContext.WriteLine("Criteria({0})", crit);
                zip1.ExtractSelectedEntries(crit, null, extractDir);
            }

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, ExtractSelectedEntries() by date, with overwrite...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                string crit = String.Format("mtime >= {0}", today.ToString("yyyy-MM-dd"));
                TestContext.WriteLine("Criteria({0})", crit);
                zip1.ExtractSelectedEntries(crit, null, extractDir, ExtractExistingFileAction.OverwriteSilently);
            }


            // workitem 9174: test ExtractSelectedEntries using a directoryPathInArchive
            List<String> dirs = new List<String>();
            // first, get the list of directories used by all entries
            TestContext.WriteLine("Reading zip, ...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                foreach (var e in zip1)
                {
                    TestContext.WriteLine("entry {0}", e.FileName);
                    string p = Path.GetDirectoryName(e.FileName.Replace("/", "\\"));
                    if (!dirs.Contains(p)) dirs.Add(p);
                }
            }

            // with or without trailing slash
            for (int i = 0; i < 2; i++)
            {
                int grandTotal = 0;
                extractDir = String.Format("extract.{0}", i);
                for (int j = 0; j < dirs.Count; j++)
                {
                    string d = dirs[j];
                    if (i == 1) d += "\\";
                    TestContext.WriteLine("====================================================");
                    TestContext.WriteLine("Reading zip, ExtractSelectedEntries() by name, with directoryInArchive({0})...", d);
                    using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                    {
                        string crit = "name = *.bin";
                        TestContext.WriteLine("Criteria({0})", crit);
                        var s = zip1.SelectEntries(crit, d);
                        TestContext.WriteLine("  {0} entries", s.Count);
                        grandTotal += s.Count;
                        zip1.ExtractSelectedEntries(crit, d, extractDir, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                TestContext.WriteLine("====================================================");
                TestContext.WriteLine("Total for all dirs: {0} entries", grandTotal);

                var extracted = Directory.GetFiles(extractDir, "*.bin", SearchOption.AllDirectories);

                Assert.AreEqual<Int32>(grandTotal, extracted.Length);
            }
        }




        [TestMethod]
        public void Selector_SelectEntries_ByName()
        {
            // Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectEntries.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            //int count1, count2;
            int entriesAdded = 0;
            String filename = null;

            string subDir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(subDir);

            int fileCount = _rnd.Next(33) + 33;
            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Files being added to the zip:");
            for (int j = 0; j < fileCount; j++)
            {
                // select binary or text
                if (_rnd.Next(2) == 0)
                {
                    filename = Path.Combine(subDir, String.Format("file{0:D3}.txt", j));
                    TestUtilities.CreateAndFillFileText(filename, _rnd.Next(5000) + 5000);
                }
                else
                {
                    filename = Path.Combine(subDir, String.Format("file{0:D3}.bin", j));
                    TestUtilities.CreateAndFillFileBinary(filename, _rnd.Next(5000) + 5000);
                }
                TestContext.WriteLine(Path.GetFileName(filename));
                entriesAdded++;
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(subDir, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entriesAdded, TestUtilities.CountEntries(zipFileToCreate));



            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, SelectEntries() by name...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                var selected1 = zip1.SelectEntries("name = *.txt");
                var selected2 = zip1.SelectEntries("name = *.bin");
                var selected3 = zip1.SelectEntries("name = *.bin OR name = *.txt");
                TestContext.WriteLine("Found {0} text files, {0} bin files.", selected1.Count, selected2.Count);
                TestContext.WriteLine("Text files:");
                foreach (ZipEntry e in selected1)
                {
                    TestContext.WriteLine(e.FileName);
                }
                Assert.AreEqual<Int32>(entriesAdded, selected1.Count + selected2.Count);
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, SelectEntries() using shorthand filters...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                var selected1 = zip1.SelectEntries("*.txt");
                var selected2 = zip1.SelectEntries("*.bin");
                TestContext.WriteLine("Text files:");
                foreach (ZipEntry e in selected1)
                {
                    TestContext.WriteLine(e.FileName);
                }
                Assert.AreEqual<Int32>(entriesAdded, selected1.Count + selected2.Count);
            }

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, SelectEntries() again ...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                string crit = "name = *.txt AND name = *.bin";
                // none of the entries should match this:
                var selected1 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected1.Count);
                Assert.AreEqual<Int32>(0, selected1.Count);

                // all of the entries should match this:
                crit = "name = *.txt XOR name = *.bin";
                var selected2 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected2.Count);
                Assert.AreEqual<Int32>(entriesAdded, selected2.Count);

                // try an compound criterion with XOR
                crit = "name = *.bin XOR name = *2.*";
                var selected3 = zip1.SelectEntries(crit);
                Assert.IsTrue(selected3.Count > 0);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected3.Count);

                // factor out the XOR
                crit = "(name = *.bin AND name != *2.*) OR (name != *.bin AND name = *2.*)";
                var selected4 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected4.Count);
                Assert.AreEqual<Int32>(selected3.Count, selected4.Count);

                // take the negation of the XOR criterion
                crit = "(name != *.bin OR name = *2.*) AND (name = *.bin OR name != *2.*)";
                var selected5 = zip1.SelectEntries(crit);
                TestContext.WriteLine("Criteria({0})  count({1})", crit, selected4.Count);
                Assert.IsTrue(selected5.Count > 0);
                Assert.AreEqual<Int32>(entriesAdded, selected3.Count + selected5.Count);
            }
        }



        [TestMethod]
        public void Selector_SelectEntries_ByName_NamesWithSpaces()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectEntries_Spaces.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            //int count1, count2;
            int entriesAdded = 0;
            String filename = null;

            string subDir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(subDir);

            int fileCount = _rnd.Next(44) + 44;
            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Files being added to the zip:");
            for (int j = 0; j < fileCount; j++)
            {
                string space = (_rnd.Next(2) == 0) ? " " : "";
                if (_rnd.Next(2) == 0)
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.txt", j, space));
                    TestUtilities.CreateAndFillFileText(filename, _rnd.Next(5000) + 5000);
                }
                else
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.bin", j, space));
                    TestUtilities.CreateAndFillFileBinary(filename, _rnd.Next(5000) + 5000);
                }
                TestContext.WriteLine(Path.GetFileName(filename));
                entriesAdded++;
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(subDir, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entriesAdded, TestUtilities.CountEntries(zipFileToCreate));



            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                var selected1 = zip1.SelectEntries("name = *.txt");
                var selected2 = zip1.SelectEntries("name = *.bin");
                TestContext.WriteLine("Text files:");
                foreach (ZipEntry e in selected1)
                {
                    TestContext.WriteLine(e.FileName);
                }
                Assert.AreEqual<Int32>(entriesAdded, selected1.Count + selected2.Count);
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, using name patterns that contain spaces...");
            string[] selectionStrings = { "name = '* *.txt'",
                                          "name = '* *.bin'",
                                          "name = *.txt and name != '* *.txt'",
                                          "name = *.bin and name != '* *.bin'",
            };
            int count = 0;
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                foreach (string selectionCriteria in selectionStrings)
                {
                    var selected1 = zip1.SelectEntries(selectionCriteria);
                    count += selected1.Count;
                    TestContext.WriteLine("  For criteria ({0}), found {1} files.", selectionCriteria, selected1.Count);
                }
            }
            Assert.AreEqual<Int32>(entriesAdded, count);

        }


        [TestMethod]
        public void Selector_RemoveSelectedEntries_Spaces()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_RemoveSelectedEntries_Spaces.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            //int count1, count2;
            int entriesAdded = 0;
            String filename = null;

            string subDir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(subDir);

            int fileCount = _rnd.Next(44) + 44;
            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Files being added to the zip:");
            for (int j = 0; j < fileCount; j++)
            {
                string space = (_rnd.Next(2) == 0) ? " " : "";
                if (_rnd.Next(2) == 0)
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.txt", j, space));
                    TestUtilities.CreateAndFillFileText(filename, _rnd.Next(5000) + 5000);
                }
                else
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.bin", j, space));
                    TestUtilities.CreateAndFillFileBinary(filename, _rnd.Next(5000) + 5000);
                }
                TestContext.WriteLine(Path.GetFileName(filename));
                entriesAdded++;
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(subDir, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entriesAdded, TestUtilities.CountEntries(zipFileToCreate));


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, using name patterns that contain spaces...");
            string[] selectionStrings = { "name = '* *.txt'",
                                          "name = '* *.bin'",
                                          "name = *.txt and name != '* *.txt'",
                                          "name = *.bin and name != '* *.bin'",
            };
            foreach (string selectionCriteria in selectionStrings)
            {
                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                {
                    var selected1 = zip1.SelectEntries(selectionCriteria);
                    zip1.RemoveEntries(selected1);
                    TestContext.WriteLine("for pattern {0}, Removed {1} entries", selectionCriteria, selected1.Count);
                    zip1.Save();
                }

            }

            Assert.AreEqual<Int32>(0, TestUtilities.CountEntries(zipFileToCreate));
        }


        [TestMethod]
        public void Selector_RemoveSelectedEntries2()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_RemoveSelectedEntries2.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            //int count1, count2;
            int entriesAdded = 0;
            String filename = null;

            string subDir = Path.Combine(TopLevelDir, "A");
            Directory.CreateDirectory(subDir);

            int fileCount = _rnd.Next(44) + 44;
            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Files being added to the zip:");
            for (int j = 0; j < fileCount; j++)
            {
                string space = (_rnd.Next(2) == 0) ? " " : "";
                if (_rnd.Next(2) == 0)
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.txt", j, space));
                    TestUtilities.CreateAndFillFileText(filename, _rnd.Next(5000) + 5000);
                }
                else
                {
                    filename = Path.Combine(subDir, String.Format("file{1}{0:D3}.bin", j, space));
                    TestUtilities.CreateAndFillFileBinary(filename, _rnd.Next(5000) + 5000);
                }
                TestContext.WriteLine(Path.GetFileName(filename));
                entriesAdded++;
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip...");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddDirectory(subDir, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entriesAdded, TestUtilities.CountEntries(zipFileToCreate));


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Reading zip, using name patterns that contain spaces...");
            string[] selectionStrings = { "name = '* *.txt'",
                                          "name = '* *.bin'",
                                          "name = *.txt and name != '* *.txt'",
                                          "name = *.bin and name != '* *.bin'",
            };
            foreach (string selectionCriteria in selectionStrings)
            {
                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                {
                    var selected1 = zip1.SelectEntries(selectionCriteria);
                    ZipEntry[] entries = new ZipEntry[selected1.Count];
                    selected1.CopyTo(entries, 0);
                    string[] names = Array.ConvertAll(entries, x => x.FileName);
                    zip1.RemoveEntries(names);
                    TestContext.WriteLine("for pattern {0}, Removed {1} entries", selectionCriteria, selected1.Count);
                    zip1.Save();
                }

            }

            Assert.AreEqual<Int32>(0, TestUtilities.CountEntries(zipFileToCreate));
        }



        [TestMethod]
        public void Selector_SelectEntries_subDirs()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectFiles_subDirs.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            int count1, count2;

            string fodder = Path.Combine(TopLevelDir, "fodder");
            Directory.CreateDirectory(fodder);


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating files...");
            int entries = 0;
            int i = 0;
            int subdirCount = _rnd.Next(17) + 9;
            //int subdirCount = _rnd.Next(3) + 2;
            var FileCount = new Dictionary<string, int>();

            var checksums = new Dictionary<string, string>();
            // I don't actually verify the checksums in this method...


            for (i = 0; i < subdirCount; i++)
            {
                string subDirShort = new System.String(new char[] { (char)(i + 65) });
                string subDir = Path.Combine(fodder, subDirShort);
                Directory.CreateDirectory(subDir);

                int filecount = _rnd.Next(8) + 8;
                //int filecount = _rnd.Next(2) + 2;
                FileCount[subDirShort] = filecount;
                for (int j = 0; j < filecount; j++)
                {
                    string filename = String.Format("file{0:D4}.x", j);
                    string fqFilename = Path.Combine(subDir, filename);
                    TestUtilities.CreateAndFillFile(fqFilename, _rnd.Next(1000) + 1000);

                    var chk = TestUtilities.ComputeChecksum(fqFilename);
                    var s = TestUtilities.CheckSumToString(chk);
                    var t1 = Path.GetFileName(fodder);
                    var t2 = Path.Combine(t1, subDirShort);
                    var key = Path.Combine(t2, filename);
                    key = TestUtilities.TrimVolumeAndSwapSlashes(key);
                    TestContext.WriteLine("chk[{0}]= {1}", key, s);
                    checksums.Add(key, s);
                    entries++;
                }
            }

            Directory.SetCurrentDirectory(TopLevelDir);

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip ({0} entries in {1} subdirs)...", entries, subdirCount);
            // add all the subdirectories into a new zip
            using (ZipFile zip1 = new ZipFile())
            {
                // add all of those subdirectories (A, B, C...) into the root in the zip archive
                zip1.AddDirectory(fodder, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entries, TestUtilities.CountEntries(zipFileToCreate));


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Selecting entries by directory...");

            for (int j = 0; j < 2; j++)
            {
                count1 = 0;
                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                {
                    for (i = 0; i < subdirCount; i++)
                    {
                        string dirInArchive = new System.String(new char[] { (char)(i + 65) });
                        if (j == 1) dirInArchive += "\\";
                        var selected1 = zip1.SelectEntries("*.*", dirInArchive);
                        count1 += selected1.Count;
                        TestContext.WriteLine("--------------\nfiles in dir {0} ({1}):",
                                              dirInArchive, selected1.Count);
                        foreach (ZipEntry e in selected1)
                            TestContext.WriteLine(e.FileName);
                    }
                    Assert.AreEqual<Int32>(entries, count1);
                }
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Selecting entries by directory and size...");
            count1 = 0;
            count2 = 0;
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                for (i = 0; i < subdirCount; i++)
                {
                    string dirInArchive = new System.String(new char[] { (char)(i + 65) });
                    var selected1 = zip1.SelectEntries("size > 1500", dirInArchive);
                    count1 += selected1.Count;
                    TestContext.WriteLine("--------------\nfiles in dir {0} ({1}):",
                                          dirInArchive, selected1.Count);
                    foreach (ZipEntry e in selected1)
                        TestContext.WriteLine(e.FileName);
                }

                var selected2 = zip1.SelectEntries("size <= 1500");
                count2 = selected2.Count;
                Assert.AreEqual<Int32>(entries, count1 + count2 - subdirCount);
            }

        }



        [TestMethod]
        public void Selector_SelectEntries_Fullpath()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectFiles_Fullpath.zip");

            Assert.IsFalse(File.Exists(zipFileToCreate), "The zip file '{0}' already exists.", zipFileToCreate);

            int count1, count2;

            string fodder = Path.Combine(TopLevelDir, "fodder");
            Directory.CreateDirectory(fodder);


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating files...");
            int entries = 0;
            int i = 0;
            int subdirCount = _rnd.Next(17) + 9;
            //int subdirCount = _rnd.Next(3) + 2;
            var FileCount = new Dictionary<string, int>();

            var checksums = new Dictionary<string, string>();
            // I don't actually verify the checksums in this method...


            for (i = 0; i < subdirCount; i++)
            {
                string subDirShort = new System.String(new char[] { (char)(i + 65) });
                string subDir = Path.Combine(fodder, subDirShort);
                Directory.CreateDirectory(subDir);

                int filecount = _rnd.Next(8) + 8;
                //int filecount = _rnd.Next(2) + 2;
                FileCount[subDirShort] = filecount;
                for (int j = 0; j < filecount; j++)
                {
                    string filename = String.Format("file{0:D4}.x", j);
                    string fqFilename = Path.Combine(subDir, filename);
                    TestUtilities.CreateAndFillFile(fqFilename, _rnd.Next(1000) + 1000);

                    var chk = TestUtilities.ComputeChecksum(fqFilename);
                    var s = TestUtilities.CheckSumToString(chk);
                    var t1 = Path.GetFileName(fodder);
                    var t2 = Path.Combine(t1, subDirShort);
                    var key = Path.Combine(t2, filename);
                    key = TestUtilities.TrimVolumeAndSwapSlashes(key);
                    TestContext.WriteLine("chk[{0}]= {1}", key, s);
                    checksums.Add(key, s);
                    entries++;
                }
            }

            Directory.SetCurrentDirectory(TopLevelDir);

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip ({0} entries in {1} subdirs)...", entries, subdirCount);
            // add all the subdirectories into a new zip
            using (ZipFile zip1 = new ZipFile())
            {
                // add all of those subdirectories (A, B, C...) into the root in the zip archive
                zip1.AddDirectory(fodder, "");
                zip1.Save(zipFileToCreate);
            }
            Assert.AreEqual<Int32>(entries, TestUtilities.CountEntries(zipFileToCreate));


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Selecting entries by full path...");
            count1 = 0;
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                for (i = 0; i < subdirCount; i++)
                {
                    string dirInArchive = new System.String(new char[] { (char)(i + 65) });
                    var selected1 = zip1.SelectEntries(Path.Combine(dirInArchive, "*.*"));
                    count1 += selected1.Count;
                    TestContext.WriteLine("--------------\nfiles in dir {0} ({1}):",
                                          dirInArchive, selected1.Count);
                    foreach (ZipEntry e in selected1)
                        TestContext.WriteLine(e.FileName);
                }
                Assert.AreEqual<Int32>(entries, count1);
            }


            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Selecting entries by directory and size...");
            count1 = 0;
            count2 = 0;
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                for (i = 0; i < subdirCount; i++)
                {
                    string dirInArchive = new System.String(new char[] { (char)(i + 65) });
                    string pathCriterion = String.Format("name = {0}",
                                                         Path.Combine(dirInArchive, "*.*"));
                    string combinedCriterion = String.Format("size > 1500  AND {0}", pathCriterion);

                    var selected1 = zip1.SelectEntries(combinedCriterion, dirInArchive);
                    count1 += selected1.Count;
                    TestContext.WriteLine("--------------\nfiles in ({0}) ({1} entries):",
                                          combinedCriterion,
                                          selected1.Count);
                    foreach (ZipEntry e in selected1)
                        TestContext.WriteLine(e.FileName);
                }

                var selected2 = zip1.SelectEntries("size <= 1500");
                count2 = selected2.Count;
                Assert.AreEqual<Int32>(entries, count1 + count2 - subdirCount);
            }
        }




        [TestMethod]
        public void Selector_SelectEntries_NestedDirectories_wi8559()
        {
            //Directory.SetCurrentDirectory(TopLevelDir);
            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectFiles_NestedDirectories.zip");

            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Creating zip file...");

            int dirCount = _rnd.Next(4) + 3;
            using (var zip = new ZipFile())
            {
                for (int i = 0; i < dirCount; i++)
                {
                    String dir = new String((char)(65 + i), i + 1);
                    zip.AddEntry(Path.Combine(dir, "Readme.txt"), "This is the content for the Readme.txt in directory " + dir);
                    int subDirCount = _rnd.Next(3) + 2;
                    for (int j = 0; j < subDirCount; j++)
                    {
                        String subdir = Path.Combine(dir, new String((char)(90 - j), 3));
                        zip.AddEntry(Path.Combine(subdir, "Readme.txt"), "This is the content for the Readme.txt in directory " + subdir);
                    }
                }
                zip.Save(zipFileToCreate);
            }

            // this testmethod does not extract files, or verify checksums ...

            // just want to verify that selection of entries works in nested directories as
            // well as
            TestContext.WriteLine("====================================================");
            TestContext.WriteLine("Selecting entries by path...");
            using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
            {
                for (int i = 0; i < dirCount; i++)
                {
                    String dir = new String((char)(65 + i), i + 1);
                    var selected1 = zip1.SelectEntries("*.txt", dir);
                    Assert.AreEqual<Int32>(1, selected1.Count);

                    selected1 = zip1.SelectEntries("*.txt", dir + "/ZZZ");
                    var selected2 = zip1.SelectEntries("*.txt", dir + "\\ZZZ");
                    Assert.AreEqual<Int32>(selected1.Count, selected2.Count);

                    selected1 = zip1.SelectEntries("*.txt", dir + "/YYY");
                    selected2 = zip1.SelectEntries("*.txt", dir + "\\YYY");
                    Assert.AreEqual<Int32>(selected1.Count, selected2.Count);
                }
            }
        }




        [TestMethod]
        public void Selector_SelectFiles_DirName_wi8245()
        {
            // workitem 8245
            //Directory.SetCurrentDirectory(TopLevelDir);
            SetupFiles();
            var ff = new Ionic.FileSelector("*.*");
            var result = ff.SelectFiles(fodderDirectory);
            Assert.IsTrue(result.Count > 1);
        }


        [TestMethod]
        public void Selector_SelectFiles_DirName_wi8245_2()
        {
            // workitem 8245
            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_SelectFiles_DirName_wi8245_2.zip");
            //Directory.SetCurrentDirectory(TopLevelDir);
            SetupFiles();

            var fodderFiles = Directory.GetFiles(fodderDirectory, "*.*", SearchOption.AllDirectories);

            TestContext.WriteLine("===============================================");
            TestContext.WriteLine("AddSelectedFiles()");
            using (ZipFile zip1 = new ZipFile())
            {
                zip1.AddSelectedFiles(fodderDirectory, null, "fodder", true);
                zip1.Save(zipFileToCreate);
            }

            Assert.AreEqual<Int32>(TestUtilities.CountEntries(zipFileToCreate), fodderFiles.Length,
                          "The Zip file has the wrong number of entries.");
        }



        [TestMethod]
        public void Selector_SelectFiles_DirName_wi9176()
        {
            // workitem 9176
            //Directory.SetCurrentDirectory(TopLevelDir);

            _txrx= SetupProgressMonitor("SelectFiles-DirName");

            SetupFiles();

            var binFiles = Directory.GetFiles(fodderDirectory, "*.bin", SearchOption.AllDirectories);

            int[] eCount = new int[2];
            _txrx.Send("pb 0 max 2");
            for (int i = 0; i < 2; i++)
            {
                string zipFileToCreate = Path.Combine(TopLevelDir,
                                                      String.Format("Selector_SelectFiles_DirName_wi9176-{0}.zip", i));
                _txrx.Send("pb 1 max 4");
                _txrx.Send("pb 1 value 0");
                string d = fodderDirectory;
                if (i == 1) d += "\\";
                TestContext.WriteLine("===============================================");
                TestContext.WriteLine("AddSelectedFiles(cycle={0})", i);
                using (ZipFile zip1 = new ZipFile())
                {
                    zip1.AddSelectedFiles("name = *.bin", d, "", true);
                    _txrx.Send("pb 1 step");
                    zip1.Save(zipFileToCreate);
                }
                _txrx.Send("pb 1 step");

                Assert.AreEqual<Int32>(TestUtilities.CountEntries(zipFileToCreate), binFiles.Length,
                                       "The Zip file has the wrong number of entries.");

                _txrx.Send("pb 2 step");

                using (ZipFile zip1 = ZipFile.Read(zipFileToCreate))
                {
                    foreach (var e in zip1)
                    {
                        if (e.FileName.Contains("/")) eCount[i]++;
                    }
                }
                _txrx.Send("pb 1 step");

                if (i==1)
                    Assert.AreEqual<Int32>(eCount[0], eCount[1],
                                           "Inconsistent results when the directory includes a path.", i);

                _txrx.Send("pb 0 step");
            }
            _txrx.Send("stop");
        }


        [TestMethod]
        public void Selector_SelectFiles_GoodSyntax01()
        {
            string[] criteria = {
                "type = D",
                "type = F",
                "attrs = HRS",
                "attrs = L",
                "name = *.txt  OR (size > 7800)",
                "name = *.harvey  OR  (size > 7800  and attributes = H)",
                "(name = *.harvey)  OR  (size > 7800  and attributes = H)",
                "(name = *.xls)  and (name != *.xls)  OR  (size > 7800  and attributes = H)",
                "(name = '*.xls')",
                "(name = Ionic.Zip.dll) or ((size > 1mb) and (name != *.zip))",
                "(name = Ionic.Zip.dll) or ((size > 1mb) and (name != *.zip)) or (name = Joe.txt)",
                "(name=Ionic.Zip.dll) or ((size>1mb) and (name!=*.zip)) or (name=Joe.txt)",
                "(name=Ionic.Zip.dll)or((size>1mb)and(name!=*.zip))or(name=Joe.txt)",
            };

            foreach (string s in criteria)
            {
                TestContext.WriteLine("Selector: " + s);
                var ff = new Ionic.FileSelector(s);
            }
        }


        [TestMethod]
        public void Selector_Twiddle_wi10153()
        {
            // workitem 10153:
            //
            // When calling AddSelectedFiles(String,String,String,bool), and when the
            // actual filesystem path uses mixed case, but the specified directoryOnDisk
            // argument is downcased, AND when the filename contains a ~ (weird, I
            // know), verify that the path replacement works as advertised, and entries
            // are rooted in the directoryInArchive specified path.

            string zipFileToCreate = Path.Combine(TopLevelDir, "Selector_Twiddle.zip");
            string dirToZip = "dirToZip";
            Directory.CreateDirectory(dirToZip);
            string filename = Path.Combine(dirToZip, String.Format("~{0}.txt", _rnd.Next(5)));
            TestUtilities.CreateAndFillFileText(filename, _rnd.Next(1000) + 500);

            using (ZipFile zip = new ZipFile())
            {
                // must use ToLower to force case mismatch
                zip.AddSelectedFiles("name != *.zip*", dirToZip.ToLower(), "", true);
                zip.Save(zipFileToCreate);
            }

            using (ZipFile zip = ZipFile.Read(zipFileToCreate))
            {
                foreach (var e in zip)
                {
                    Assert.IsFalse(e.FileName.Contains("/"), "The filename contains a path, but shouldn't");
                }
            }

            //BasicVerifyZip(zipFileToCreate);
        }



        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadNoun()
        {
            new Ionic.FileSelector("fame = *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax01()
        {
            new Ionic.FileSelector("size = ");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax02()
        {
            new Ionic.FileSelector("name = *.txt and");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax03()
        {
            new Ionic.FileSelector("name = *.txt  URF ");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax04()
        {
            new Ionic.FileSelector("name = *.txt  OR (");
        }

        [TestMethod]
        [ExpectedException(typeof(System.FormatException))]
        public void Selector_SelectFiles_BadSyntax05()
        {
            new Ionic.FileSelector("name = *.txt  OR (size = G)");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax06()
        {
            new Ionic.FileSelector("name = *.txt  OR (size > )");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax07()
        {
            new Ionic.FileSelector("name = *.txt  OR (size > 7800");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax08()
        {
            new Ionic.FileSelector("name = *.txt  OR )size > 7800");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax09()
        {
            new Ionic.FileSelector("name = *.txt and  name =");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax10()
        {
            new Ionic.FileSelector("name == *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax10a()
        {
            new Ionic.FileSelector("name >= *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax11()
        {
            new Ionic.FileSelector("name ~= *.txt");
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax12()
        {
            new Ionic.FileSelector("name @ = *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax13()
        {
            new Ionic.FileSelector("name LIKE  *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax14()
        {
            new Ionic.FileSelector("name AND  *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax15()
        {
            new Ionic.FileSelector("name (AND  *.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax16()
        {
            new Ionic.FileSelector("mtime 2007-01-01");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax17()
        {
            new Ionic.FileSelector("size 1kb");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax18()
        {
            Ionic.FileSelector ff = new Ionic.FileSelector("");
            var list = ff.SelectFiles(".");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax19()
        {
            Ionic.FileSelector ff = new Ionic.FileSelector(null);
            var list = ff.SelectFiles(".");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax20()
        {
            new Ionic.FileSelector("attributes > HRTS");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax21()
        {
            new Ionic.FileSelector("attributes HRTS");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax22a()
        {
            new Ionic.FileSelector("attributes = HHHA");
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax22b()
        {
            new Ionic.FileSelector("attributes = SHSA");
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax22c()
        {
            new Ionic.FileSelector("attributes = AHA");
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax22d()
        {
            new Ionic.FileSelector("attributes = RRA");
        }
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax22e()
        {
            new Ionic.FileSelector("attributes = IRIA");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax23()
        {
            new Ionic.FileSelector("attributes = INVALID");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax24a()
        {
            new Ionic.FileSelector("type = I");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Selector_SelectFiles_BadSyntax24b()
        {
            new Ionic.FileSelector("type > D");
        }

    }
}
