// RandomTextGenerator.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa
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
// Time-stamp: <2010-February-15 10:55:49>
//
// ------------------------------------------------------------------
//
// This module defines a class that generates random text sequences
// using a Markov chain.
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Ionic.Zip.Tests.Utilities
{

    public class RandomTextGenerator
    {
        static string[] uris = new string[]
            {
                // "Through the Looking Glass", by Lewis Carroll (~181k)
                "http://www.gutenberg.org/files/12/12.txt",

                // Decl of Independence (~16k)
                "http://www.gutenberg.org/files/16780/16780.txt",

                // The Naval War of 1812, by Theodore Roosevelt (968k)
                "http://www.gutenberg.org/dirs/etext05/7trnv10.txt",

                // On Prayer and the Contemplative Life, by Thomas Aquinas (440k)
                "http://www.gutenberg.org/files/22295/22295.txt",
            };

        SimpleMarkovChain markov;

        public RandomTextGenerator()
        {
            System.Random rnd = new System.Random();
            string uri = uris[rnd.Next(uris.Length)];
            string seedText = GetPageMarkup(uri);
            markov = new SimpleMarkovChain(seedText);
        }


        public string Generate(int length)
        {
            return markov.GenerateText(length);
        }


        private static string GetPageMarkup(string uri)
        {
            string pageData = null;
            using (WebClient client = new WebClient())
            {
                pageData = client.DownloadString(uri);
            }
            return pageData;
        }
    }


    /// <summary>
    /// Implements a simple Markov chain for text.
    /// </summary>
    ///
    /// <remarks>
    /// Uses a Markov chain starting with some base texts to produce
    /// random natural-ish text. This implementation is based on Pike's
    /// perl implementation, see
    /// http://cm.bell-labs.com/cm/cs/tpop/markov.pl
    /// </remarks>
    public class SimpleMarkovChain
    {
        Dictionary<String, List<String>> table = new Dictionary<String, List<String>>();
        System.Random rnd = new System.Random();

        public SimpleMarkovChain(string seed)
        {
            string NEWLINE = "\n";
            string key = NEWLINE;
            var sr = new StringReader(seed);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                foreach (var word in line.SplitByWords())
                {
                    var w = (word == "") ? NEWLINE : word; // newline
                    if (word == "\r") w = NEWLINE;

                    if (!table.ContainsKey(key)) table.Add(key, new List<string>());
                    table[key].Add(w);
                    key = w.ToLower().TrimPunctuation();
                }
            }
            if (!table.ContainsKey(key)) table.Add(key, new List<string>());
            table[key].Add(NEWLINE);
            key = NEWLINE;
        }


        internal void Diag()
        {
            Console.WriteLine("There are {0} keys in the table", table.Keys.Count);
            foreach (string s in table.Keys)
            {
                string x = s.Replace("\n", "�");
                var y = table[s].ToArray();
                Console.WriteLine("  {0}: {1}", x, String.Join(", ", y));
            }
        }

        internal void ShowList(string word)
        {
            string x = word.Replace("\n", "�");
            if (table.ContainsKey(word))
            {
                var y = table[word].ToArray();
                var z = Array.ConvertAll(y, x1 => x1.Replace("\n", "�"));
                Console.WriteLine("  {0}: {1}", x, String.Join(", ", z));
            }
            else
                Console.WriteLine("  {0}: -key not found-", x);
        }

        private List<string> _keywords;
        private List<string> keywords
        {
            get
            {
                if (_keywords == null)
                    _keywords = new List<String>(table.Keys);
                return _keywords;
            }
        }

        /// <summary>
        /// Generates random text with a minimum character length.
        /// </summary>
        ///
        /// <param name="minimumLength">
        /// The minimum length of text, in characters, to produce.
        /// </param>
        public string GenerateText(int minimumLength)
        {
            var chosenStartWord = keywords[rnd.Next(keywords.Count)];
            return _InternalGenerate(chosenStartWord, StopCriterion.NumberOfChars, minimumLength);
        }

        /// <summary>
        /// Generates random text with a minimum character length.
        /// </summary>
        ///
        /// <remarks>
        /// The first sentence will start with the given start word.
        /// </remarks>
        ///
        /// <param name="minimumLength">
        /// The minimum length of text, in characters, to produce.
        /// </param>
        /// <param name="start">
        /// The word to start with. If this word does not exist in the
        /// seed text, the generation will fail.
        /// </param>
        /// <seealso cref="GenerateText(int)"/>
        /// <seealso cref="GenerateWords(int)"/>
        /// <seealso cref="GenerateWords(string, int)"/>
        public string GenerateText(string start, int minimumLength)
        {
            return _InternalGenerate(start, StopCriterion.NumberOfChars, minimumLength);
        }

        /// <summary>
        /// Generate random text with a minimum number of words.
        /// </summary>
        ///
        /// <remarks>
        /// The first sentence will start with the given start word.
        /// </remarks>
        ///
        /// <param name="minimumWords">
        /// The minimum number of words of text to produce.
        /// </param>
        /// <param name="start">
        /// The word to start with. If this word does not exist in the
        /// seed text, the generation will fail.
        /// </param>
        /// <seealso cref="GenerateText(int)"/>
        /// <seealso cref="GenerateText(string, int)"/>
        /// <seealso cref="GenerateWords(int)"/>
        public string GenerateWords(string start, int minimumWords)
        {
            return _InternalGenerate(start, StopCriterion.NumberOfWords, minimumWords);
        }


        /// <summary>
        /// Generate random text with a minimum number of words.
        /// </summary>
        ///
        /// <param name="minimumWords">
        /// The minimum number of words of text to produce.
        /// </param>
        /// <seealso cref="GenerateText(int)"/>
        /// <seealso cref="GenerateWords(string, int)"/>
        public string GenerateWords(int minimumWords)
        {
            var chosenStartWord = keywords[rnd.Next(keywords.Count)];
            return _InternalGenerate(chosenStartWord, StopCriterion.NumberOfWords, minimumWords);
        }


        private string _InternalGenerate(string start, StopCriterion crit, int limit)
        {
            string w1 = start.ToLower();
            StringBuilder sb = new StringBuilder();
            sb.Append(start.Capitalize());

            int consecutiveNewLines = 0;
            string word = null;
            string priorWord = null;

            // About the stop criteria:
            // we keep going til we reach the specified number of words or chars, with the added
            // proviso that we have to complete the in-flight sentence when the limit is reached.

            for (int i = 0;
                 (crit == StopCriterion.NumberOfWords && i < limit) ||
                     (crit == StopCriterion.NumberOfChars && sb.Length < limit) ||
                     consecutiveNewLines == 0;
                 i++)
            {
                if (table.ContainsKey(w1))
                {
                    var list = table[w1];
                    int ix = rnd.Next(list.Count);
                    priorWord = word;
                    word = list[ix];
                    if (word != "\n")
                    {
                        // capitalize
                        if (consecutiveNewLines > 0)
                            sb.Append(word.Capitalize());
                        else
                            sb.Append(" ").Append(word);

                        // words that end sentences get a newline
                        if (word.EndsWith("."))
                        {
                            if (consecutiveNewLines == 0 || consecutiveNewLines == 1)
                                sb.Append("\n");
                            consecutiveNewLines++;
                        }
                        else consecutiveNewLines = 0;
                    }
                    w1 = word.ToLower().TrimPunctuation();
                }
            }
            return sb.ToString();
        }



        private enum StopCriterion
        {
            NumberOfWords,
            NumberOfChars
        }

    }



    public class RandomTextInputStream : Stream
    {
        RandomTextGenerator _rtg;
        Int64 _desiredLength;
        Int64 _bytesRead;
        bool _isDisposed;
        System.Text.Encoding _encoding;
        string[] _randomText;
        byte[] _src;
        int _nblocks, _p, _readyIndex, _slurps, _gnt;
        static readonly int _maxChunkSize = 1024 * 32;
        System.Threading.ManualResetEvent _producerDone;
        System.Threading.AutoResetEvent _needData, _haveData;

        public RandomTextInputStream(Int64 length)
        : this(length, System.Text.Encoding.GetEncoding("ascii"))
        {
        }

        public RandomTextInputStream(Int64 length, System.Text.Encoding encoding)
        : base()
        {
            _desiredLength = length;
            _rtg = new RandomTextGenerator();
            _encoding = encoding;
            _nblocks = 0;
            _p = 0;
            _isDisposed = false;
            _randomText = new string[2];
            _needData = new AutoResetEvent(false);
            _haveData = new AutoResetEvent(false);
            _producerDone = new ManualResetEvent(false);

            ThreadPool.QueueUserWorkItem(new WaitCallback(_Producer));
        }


        /// <summary>
        ///   Method for the background thread that generates random text strings
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The RandomTextInputStream is often used to generate
        ///     large texts or strings.  There's an advantage to using a
        ///     background thread to generating those texts while the
        ///     foreground is reading from the text
        ///   </para>
        /// </remarks>
        private void _Producer(object state)
        {
            int nowServing = 0;
            try
            {
                do
                {
                    _randomText[nowServing]= _rtg.Generate(_maxChunkSize);
                    _slurps++;
                    _needData.WaitOne();
                    if (_isDisposed) break;
                    _readyIndex = nowServing;
                    nowServing = 1-nowServing;
                    _haveData.Set();
                } while (true);
            }
            catch (System.Exception)
            {
            }

            _producerDone.Set();
        }

        /// <summary>
        ///   for diagnostic purposes only
        /// </summary>
        public int SlurpsCount
        {
            get
            {
                return _slurps;
            }
        }

        /// <summary>
        ///   for diagnostic purposes only
        /// </summary>
        public int GetNewTextCount
        {
            get
            {
                return _gnt;
            }
        }

        new public void  Dispose()
        {
             Dispose(true);
        }

        private void StopProducer()
        {
            _isDisposed = true;
            _needData.Set();
            _producerDone.WaitOne();        
        }

        /// <summary>The Dispose method</summary>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                StopProducer();
                // dispose managed resources
                _producerDone.Close();
                _haveData.Close();
                _needData.Close();
            }
        }

        private string GetNewText()
        {
            _gnt++;
            _needData.Set();
            _haveData.WaitOne();
            return _randomText[_readyIndex];
        }

        public Int64 BytesRead
        {
            get { return _bytesRead; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int remainingBytesToRead = count;
            if (_desiredLength - _bytesRead < remainingBytesToRead)
                remainingBytesToRead = unchecked((int)(_desiredLength - _bytesRead));

            int totalBytesToRead = remainingBytesToRead;

            while (remainingBytesToRead > 0)
            {
                if (_nblocks % 32 == 0)
                {
                    // after 32 cycles through the buffer, get a new one.
                    _src = _encoding.GetBytes(GetNewText());
                    _p = 0;
                }

                int bytesAvailable = _src.Length - _p;
                int chunksize = (remainingBytesToRead > bytesAvailable) ? bytesAvailable : remainingBytesToRead;

                Array.Copy(_src, _p, buffer, offset, chunksize);
                _p += chunksize;
                remainingBytesToRead -= chunksize;
                offset += chunksize;
                if (_p >= _src.Length)
                {
                    // reset pointer to beginning of buffer
                    _p = 0;
                    _nblocks++;
                }
            }
            _bytesRead += totalBytesToRead;

            return totalBytesToRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _desiredLength; }
        }

        public override long Position
        {
            get { return _desiredLength - _bytesRead; }
            set
            {
                Console.WriteLine("setting position to {0}", value);
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            if (value < _bytesRead)
                throw new NotImplementedException();
            _desiredLength = value;
        }

        public override void Flush()
        {
        }
    }



}
