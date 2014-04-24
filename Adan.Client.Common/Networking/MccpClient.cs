// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MccpClient.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Adan.Client.Common.Conveyor;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Ionic.Zlib;

    /// <summary>
    /// Class to handle MCCP compression logic.
    /// </summary>
    public class MccpClient : TelnetClient
    {
        private readonly byte[] _buffer = new byte[32767];
        private readonly MemoryStream _compressedDataStream;
        private readonly byte[] _inputPostProcessingBuffer = new byte[32767];
        private readonly byte[] _unpackBuffer = new byte[32767];
        private ZlibStream _zlibDecompressionStream;

        private bool _compressionEnabled;
        private bool _customProtocolEnabled;
        private bool _compressionInProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MccpClient"/> class.
        /// </summary>
        public MccpClient()
        {
            _compressedDataStream = new MemoryStream(_buffer);
        }

        /// <summary>
        /// Gets the number of bytes recieved.
        /// </summary>
        public uint TotalBytesReceived
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bytes decompressed.
        /// </summary>
        public uint BytesDecompressed
        {
            get;
            private set;
        }

        /// <summary>
        /// Connects to the specified host and port.
        /// </summary>
        /// <param name="host">
        /// The host to connect to.
        /// </param>
        /// <param name="port">
        /// The port to use.
        /// </param>
        public override void Connect([NotNull] string host, int port)
        {
            Validate.ArgumentNotNull(host, "host");

            _compressionEnabled = false;
            _compressionInProgress = false;
            _customProtocolEnabled = false;
            _zlibDecompressionStream = null;
            TotalBytesReceived = 0;
            BytesDecompressed = 0;

            base.Connect(host, port);
        }

        /// <summary>
        /// Sends the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing data to send.</param>
        /// <param name="offset">The zero-based position in <paramref name="buffer"/> at which to begin sending data.</param>
        /// <param name="length">The number of bytes to send.</param>
        public override void Send(byte[] buffer, int offset, int length)
        {
            Assert.ArgumentNotNull(buffer, "buffer");

            // need to replace byte with code 255 (equal to Iac) to double 255,255
            int currentInProcessingBuffer = 0;
            int currentInBuffer = offset;
            while (currentInBuffer - offset < length)
            {
                if (buffer[currentInBuffer] == 255)
                {
                    _inputPostProcessingBuffer[currentInProcessingBuffer] = 255;
                    currentInProcessingBuffer++;
                }

                _inputPostProcessingBuffer[currentInProcessingBuffer] = buffer[currentInBuffer];
                currentInProcessingBuffer++;
                currentInBuffer++;
            }

            base.Send(_inputPostProcessingBuffer, 0, currentInProcessingBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Disconnect()
        {
            base.Disconnect();
        }

        /// <summary>
        /// This method is called each time when some data is received from server.
        /// </summary>
        /// <param name="sender">The source of event.</param>
        /// <param name="e">Arguments of event.</param>
        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            TotalBytesReceived += (uint)e.BytesReceived;
            byte[] data = e.GetData();

            if (!_compressionInProgress)
            {
                ProcessData(data, e.Offset, e.BytesReceived);
            }
            else
            {
                _compressedDataStream.Seek(0, SeekOrigin.Begin);
                _compressedDataStream.Write(data, e.Offset, e.BytesReceived);
                _compressedDataStream.SetLength(e.BytesReceived);
                _compressedDataStream.Seek(0, SeekOrigin.Begin);

                int bytesDecompresed = _zlibDecompressionStream.Read(_unpackBuffer, 0, _unpackBuffer.Length);
                if (bytesDecompresed < 0)
                {
                    _compressionInProgress = false;
                    _zlibDecompressionStream.Dispose();
                    _zlibDecompressionStream = null;
                }
                else
                {
                    BytesDecompressed += (uint)bytesDecompresed;
                    ProcessData(_unpackBuffer,0, bytesDecompresed);
                }
            }
        }

        private void ProcessData(byte[] data, int startOffset, int bytesCount)
        {
            int lastOffset = startOffset + bytesCount;
            int offset = startOffset;
            offset = FindIAC(data, offset, lastOffset - offset);
            if (!_compressionEnabled || !_compressionInProgress || !_customProtocolEnabled)
            {
                while (offset >= 0)
                {
                    int codeLength = ProcessIAC(data, offset, lastOffset - offset);
                    if (codeLength > 0)
                    {
                        if (offset != startOffset)
                        {
                            base.OnDataReceived(this, new DataReceivedEventArgs(offset - startOffset, startOffset, data));
                        }

                        if (lastOffset > offset + codeLength)
                            this.OnDataReceived(this, new DataReceivedEventArgs(lastOffset - (offset + codeLength), offset + codeLength, data));

                        return;
                    }

                    offset++;
                    offset = FindIAC(data, offset, lastOffset - offset);
                }
            }

            base.OnDataReceived(this, new DataReceivedEventArgs(bytesCount, startOffset, data));
        }

        private int FindIAC(byte[] data, int offset, int bytesCount)
        {
            int endOffset = offset + bytesCount;
            while (offset < endOffset)
            {
                if (data[offset] == TelnetConstants.InterpretAsCommandCode)
                    return offset;

                offset++;
            }

            return -1;
        }

        private int ProcessIAC(byte[] data, int offset, int bytesCount)
        {
            if (!_compressionEnabled && bytesCount >= 3
                && data[offset] == TelnetConstants.InterpretAsCommandCode
                        && data[offset + 1] == TelnetConstants.WillCode
                        && data[offset + 2] == TelnetConstants.CompressCode)
            {
                // negotiation packet received IAC WILL COMPRESS
                // will send responce IAC DO COMPRESS
                _compressionEnabled = true;
                base.Send(new[] { TelnetConstants.InterpretAsCommandCode, TelnetConstants.DoCode, TelnetConstants.CompressCode }, 0, 3);
                return 3;
            }
            else if (!_customProtocolEnabled && bytesCount >= 3
                && data[offset] == TelnetConstants.InterpretAsCommandCode
                && data[offset + 1] == TelnetConstants.WillCode
                && data[offset + 2] == TelnetConstants.CustomProtocolCode)
            {
                _customProtocolEnabled = true;
                base.Send(new[] { TelnetConstants.InterpretAsCommandCode, TelnetConstants.DoCode, TelnetConstants.CustomProtocolCode }, 0, 3);
                return 3;
            }
            else if (_compressionEnabled && bytesCount >= 5
                                && data[offset] == TelnetConstants.InterpretAsCommandCode
                 && data[offset + 1] == TelnetConstants.SubNegotiationStartCode
                 && data[offset + 2] == TelnetConstants.CompressCode
                 && data[offset + 3] == TelnetConstants.WillCode
                 && data[offset + 4] == TelnetConstants.SubNegotiationEndCode)
            {
                _compressionInProgress = true;
                _zlibDecompressionStream = new ZlibStream(_compressedDataStream, CompressionMode.Decompress, true);
                return 5;
            }

            return -1;
        }
    }
}