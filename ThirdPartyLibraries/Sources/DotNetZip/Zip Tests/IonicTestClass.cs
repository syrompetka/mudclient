// IonicTestClass.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa.
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
// Time-stamp: <2010-February-10 11:33:43>
//
// ------------------------------------------------------------------
//
// This module defines the base class for DotNetZip test classes.
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using System.IO;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ionic.Zip.Tests.Utilities
{
    public class IonicTestClass
    {
        protected System.Random _rnd;
        protected System.Collections.Generic.List<string> _FilesToRemove;
        protected static string CurrentDir = null;
        protected string TopLevelDir = null;
        private string _wzunzip = null;
        private string _wzzip = null;
        private string _sevenzip = null;
        private bool? _WinZipIsPresent;
        private bool? _SevenZipIsPresent;


        public IonicTestClass()
        {
            _rnd = new System.Random();
            _FilesToRemove = new System.Collections.Generic.List<string>();
        }

        #region Context
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion



        #region Test Init and Cleanup
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void BaseClassInitialize(TestContext testContext)
        {
            CurrentDir = Directory.GetCurrentDirectory();
            Assert.AreNotEqual<string>(Path.GetFileName(CurrentDir), "Temp", "at startup");
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //


        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            if (CurrentDir == null) CurrentDir = Directory.GetCurrentDirectory();
            TestUtilities.Initialize(out TopLevelDir);
            _FilesToRemove.Add(TopLevelDir);
            Directory.SetCurrentDirectory(TopLevelDir);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestUtilities.Cleanup(CurrentDir, _FilesToRemove);
        }

        #endregion



        internal string Exec(string program, string args)
        {
            return Exec(program, args, true);
        }

        internal string Exec(string program, string args, bool waitForExit)
        {
            return Exec(program, args, waitForExit, true);
        }

        internal string Exec(string program, string args, bool waitForExit, bool emitOutput)
        {
            if (args == null)
                throw new ArgumentException("args");

            if (program == null)
                throw new ArgumentException("program");

            // Microsoft.VisualStudio.TestTools.UnitTesting
            this.TestContext.WriteLine("running command: {0} {1}", program, args);

            string output;
            int rc = TestUtilities.Exec_NoContext(program, args, waitForExit, out output);

            if (rc != 0)
                throw new Exception(String.Format("Exception running app {0}: {1}", program, output));

            if (emitOutput)
                this.TestContext.WriteLine("output: {0}", output);
            else
                this.TestContext.WriteLine("A-OK. (output suppressed)");

            return output;
        }



        protected string sevenZip
        {
            get
            {
                if (_sevenzip == null)
                {
                    string progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                    _sevenzip = Path.Combine(progfiles, "7-zip\\7z.exe");
                    Assert.IsTrue(File.Exists(_sevenzip), "exe ({0}) does not exist", _sevenzip);
                }
                return _sevenzip;
            }
        }



        protected string wzzip
        {
            get
            {
                if (_wzzip == null)
                {
                    string progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                    _wzzip = Path.Combine(progfiles, "winzip\\wzzip.exe");
                    Assert.IsTrue(File.Exists(_wzzip), "exe ({0}) does not exist", _wzzip);
                }
                return _wzzip;
            }
        }


        protected string wzunzip
        {
            get
            {
                if (_wzunzip == null)
                {
                    string progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                    _wzunzip = Path.Combine(progfiles, "winzip\\wzunzip.exe");
                    Assert.IsTrue(File.Exists(_wzunzip), "exe ({0}) does not exist", _wzunzip);
                }
                return _wzunzip;
            }
        }

        protected bool WinZipIsPresent
        {
            get
            {
                if (_WinZipIsPresent == null)
                {
                    string progfiles = null;
                    if (_wzunzip == null)
                    {
                        progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                        _wzunzip = Path.Combine(progfiles, "winzip\\wzunzip.exe");
                    }
                    if (_wzzip == null)
                    {
                        if (progfiles == null)
                            progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                        _wzzip = Path.Combine(progfiles, "winzip\\wzzip.exe");
                    }
                    _WinZipIsPresent = new Nullable<bool>(File.Exists(_wzunzip) && File.Exists(_wzzip));
                }
                return _WinZipIsPresent.Value;
            }
        }

        protected bool SevenZipIsPresent
        {
            get
            {
                if (_SevenZipIsPresent == null)
                {
                    string progfiles = null;
                    if (_sevenzip == null)
                    {
                        progfiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                        _sevenzip = Path.Combine(progfiles, "7-zip\\7z.exe");
                    }
                    _SevenZipIsPresent = new Nullable<bool>(File.Exists(_sevenzip));
                }
                return _SevenZipIsPresent.Value;
            }
        }


        internal void BasicVerifyZip(string zipfile)
        {
            BasicVerifyZip(zipfile, null);
        }


        internal void BasicVerifyZip(string zipfile, string password)
        {
            BasicVerifyZip(zipfile, password, true);
        }

        internal void BasicVerifyZip(string zipfile, string password, bool emitOutput)
        {
            // basic verification of the zip file - can it be extracted?
            // The extraction tool will verify checksums and passwords, as appropriate
            if (WinZipIsPresent)
            {
                TestContext.WriteLine("Verifying zip file {0} with WinZip", zipfile);
                string args = (password == null)
                    ? String.Format("-t {0}", zipfile)
                    : String.Format("-s{0} -t {1}", password, zipfile);

                string wzunzipOut = this.Exec(wzunzip, args, true, emitOutput);
            }
            else
            {
                TestContext.WriteLine("Verifying zip file {0} with DotNetZip", zipfile);
                ReadOptions options = new ReadOptions();
                if (emitOutput)
                    options.StatusMessageWriter = new StringWriter();

                string extractDir = "verify";
                int c = 0;
                while (Directory.Exists(extractDir + c)) c++;
                extractDir += c;

                using (ZipFile zip2 = ZipFile.Read(zipfile, options))
                {
                    zip2.Password = password;
                    zip2.ExtractAll(extractDir);
                }
                // emit output, as desired
                if (emitOutput)
                    TestContext.WriteLine(options.StatusMessageWriter.ToString());
            }
        }



        internal static void CreateFilesAndChecksums(string subdir,
                                                     out string[] filesToZip,
                                                     out Dictionary<string, byte[]> checksums)
        {
            CreateFilesAndChecksums(subdir, 0, 0, out filesToZip, out checksums);
        }


        internal static void CreateFilesAndChecksums(string subdir,
                                                     int numFiles,
                                                     int baseSize,
                                                     out string[] filesToZip,
                                                     out Dictionary<string, byte[]> checksums)
        {
            // create a bunch of files
            filesToZip = TestUtilities.GenerateFilesFlat(subdir, numFiles, baseSize);
            DateTime atMidnight = new DateTime(DateTime.Now.Year,
                                               DateTime.Now.Month,
                                               DateTime.Now.Day);
            DateTime fortyFiveDaysAgo = atMidnight - new TimeSpan(45, 0, 0, 0);

            // get checksums for each one
            checksums = new Dictionary<string, byte[]>();

            var rnd = new System.Random();
            foreach (var f in filesToZip)
            {
                if (rnd.Next(3) == 0)
                    File.SetLastWriteTime(f, fortyFiveDaysAgo);
                else
                    File.SetLastWriteTime(f, atMidnight);

                var key = Path.GetFileName(f);
                var chk = TestUtilities.ComputeChecksum(f);
                checksums.Add(key, chk);
            }
        }

        protected static void CreateLargeFilesWithChecksums(string subdir, int numFiles, out string[] filesToZip, out Dictionary<string, byte[]> checksums)
        {
            // create a bunch of files
            filesToZip = TestUtilities.GenerateFilesFlat(subdir, numFiles, 256 * 1024, 3 * 1024 * 1024);
            DateTime atMidnight = new DateTime(DateTime.Now.Year,
                                               DateTime.Now.Month,
                                               DateTime.Now.Day);
            DateTime fortyFiveDaysAgo = atMidnight - new TimeSpan(45, 0, 0, 0);

            // get checksums for each one
            checksums = new Dictionary<string, byte[]>();

            var rnd = new System.Random();
            foreach (var f in filesToZip)
            {
                if (rnd.Next(3) == 0)
                    File.SetLastWriteTime(f, fortyFiveDaysAgo);
                else
                    File.SetLastWriteTime(f, atMidnight);

                var key = Path.GetFileName(f);
                var chk = TestUtilities.ComputeChecksum(f);
                checksums.Add(key, chk);
            }
        }



        protected void VerifyChecksums(string extractDir,
            System.Collections.Generic.IEnumerable<String> filesToCheck,
            Dictionary<string, byte[]> checksums)
        {
            int count = 0;
            foreach (var fqPath in filesToCheck)
            {
                var f = Path.GetFileName(fqPath);
                var extractedFile = Path.Combine(extractDir, f);
                Assert.IsTrue(File.Exists(extractedFile), "File does not exist ({0})", extractedFile);
                var chk = TestUtilities.ComputeChecksum(extractedFile);
                Assert.AreEqual<String>(TestUtilities.CheckSumToString(checksums[f]),
                                        TestUtilities.CheckSumToString(chk),
                                        String.Format("Checksums for file {0} do not match.", f));
                count++;
            }

            if (checksums.Count < count)
            {
                TestContext.WriteLine("There are {0} more extracted files than checksums", count - checksums.Count);
                foreach (var file in filesToCheck)
                {
                    if (!checksums.ContainsKey(file))
                    {
                        TestContext.WriteLine("Missing: {0}", Path.GetFileName(file));
                    }
                }
            }

            if (checksums.Count > count)
            {
                TestContext.WriteLine("There are {0} more checksums than extracted files", checksums.Count - count);
                foreach (var file in checksums.Keys)
                {
                    var selection = from f in filesToCheck where Path.GetFileName(f).Equals(file) select f;

                    if (selection.Count() == 0)
                    {
                        TestContext.WriteLine("Missing: {0}", Path.GetFileName(file));
                    }
                }
            }


            Assert.AreEqual<Int32>(checksums.Count, count, "There's a mismatch between the checksums and the extracted files.");
        }
    }


}
