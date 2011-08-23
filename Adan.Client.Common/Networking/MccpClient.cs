// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MccpClient.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Networking
{
    using System;
    using System.IO;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Ionic.Zlib;

    /// <summary>
    /// Class to handle MCCP compression logic.
    /// </summary>
    public class MccpClient : TelnetClient
    {
        private const byte _iacCode = 0xFF;
        private const byte _willCode = 0xFB;
        private const byte _willNotCode = 0xFC;
        private const byte _doCode = 0xFD;
        private const byte _compressCode = 0x55;
        private const byte _subNegotioationStartCode = 0xFA;
        private const byte _subNegotioationEndCode = 0xF0;
        private const byte _goaheadCode = 0xF9;

        private readonly byte[] _buffer = new byte[32767];
        private readonly MemoryStream _compressedDataStream;
        private readonly byte[] _inputPostProcessingBuffer = new byte[32767];
        private readonly byte[] _pingBackBuffer = new byte[] { _iacCode, _doCode, 0 };
        private readonly byte[] _postprocessingBuffer = new byte[32767];
        private readonly byte[] _unpackBuffer = new byte[32767];
        private readonly ZlibStream _zlibDecompressionStream;

        private bool _compressionEnabled;
        private bool _compressionInProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MccpClient"/> class.
        /// </summary>
        public MccpClient()
        {
            _compressedDataStream = new MemoryStream(_buffer);
            _zlibDecompressionStream = new ZlibStream(_compressedDataStream, CompressionMode.Decompress, true);
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
        /// This method is called each time when some data is received from server.
        /// </summary>
        /// <param name="sender">The source of event.</param>
        /// <param name="e">Arguments of event.</param>
        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            int offset = e.Offset;
            int bytesReceived = e.BytesReceived;
            byte[] data = e.GetData();
            if (!_compressionEnabled)
            {
                if (bytesReceived >= 3
                    && data[offset] == _iacCode
                    && data[offset + 1] == _willCode
                    && data[offset + 2] == _compressCode)
                {
                    // negotiation packet received IAC WILL COMPRESS
                    // will send responce IAC DO COMPRESS
                    _compressionEnabled = true;
                    base.Send(new[] { _iacCode, _doCode, _compressCode }, 0, 3);
                    offset += 3;
                }
            }

            if (_compressionEnabled && !_compressionInProgress)
            {
                // Checking for subnegotiation packet IAC SB COMPRESS WILL SE
                if (e.BytesReceived >= 5
                    && data[offset] == _iacCode
                    && data[offset + 1] == _subNegotioationStartCode
                    && data[offset + 2] == _compressCode
                    && data[offset + 3] == _willCode
                    && data[offset + 4] == _subNegotioationEndCode)
                {
                    _compressionInProgress = true;
                    offset += 5;
                }
            }

            if (bytesReceived >= 3
                && data[offset] == _iacCode
                && data[offset + 1] == _willCode)
            {
                _pingBackBuffer[2] = data[offset + 2];
                base.Send(_pingBackBuffer, 0, 3);
                offset += 3;
            }

            if (bytesReceived - offset > 0)
            {
                if (_compressionInProgress)
                {
                    _compressedDataStream.Seek(0, SeekOrigin.Begin);
                    _compressedDataStream.Write(data, offset, bytesReceived);
                    _compressedDataStream.SetLength(bytesReceived);
                    _compressedDataStream.Seek(0, SeekOrigin.Begin);
                    bytesReceived = _zlibDecompressionStream.Read(_unpackBuffer, 0, _unpackBuffer.Length);
                    offset = 0;
                    PostProcess(bytesReceived, offset, _unpackBuffer);
                }
                else
                {
                    PostProcess(bytesReceived, offset, data);
                }
            }
        }

        /// <summary>
        /// Posts the process received data (removing double Iac and processing ping requests).
        /// </summary>
        /// <param name="bytesReceived">
        /// The bytes received.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="data">
        /// The data from server.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Invalid Iac sequence detected.
        /// </exception>
        private void PostProcess(int bytesReceived, int offset, [NotNull] byte[] data)
        {
            Validate.ArgumentNotNull(data, "data");
            int currentInDataBuffer = offset;
            int currentInWorkingBuffer = 0;
            bool isInIacSequence = false;
            bool isInIacWillSequence = false;
            bool isInIacWillNotSequence = false;
            bool isGoageadReceived = false;

            while (currentInDataBuffer < bytesReceived)
            {
                if (isInIacSequence)
                {
                    if (data[currentInDataBuffer] == _iacCode)
                    {
                        _postprocessingBuffer[currentInWorkingBuffer] = _iacCode;
                        isInIacSequence = false;
                        currentInWorkingBuffer++;
                    }
                    else if (data[currentInDataBuffer] == _willCode)
                    {
                        isInIacSequence = false;
                        isInIacWillSequence = true;
                    }
                    else if (data[currentInDataBuffer] == _willNotCode)
                    {
                        isInIacSequence = false;
                        isInIacWillNotSequence = true;
                    }
                    else if (data[currentInDataBuffer] == _goaheadCode)
                    {
                        if (currentInDataBuffer == bytesReceived - 1)
                        {
                            isGoageadReceived = true;
                        }

                        isInIacSequence = false;
                    }
                    else
                    {
                        isInIacSequence = false;
                    }
                }
                else if (isInIacWillSequence)
                {
                    _pingBackBuffer[2] = data[currentInDataBuffer];
                    base.Send(_pingBackBuffer, 0, 3);
                    isInIacWillSequence = false;
                }
                else if (isInIacWillNotSequence)
                {
                    isInIacWillNotSequence = false;
                }
                else if (data[currentInDataBuffer] == _iacCode)
                {
                    isInIacSequence = true;
                }
                else
                {
                    _postprocessingBuffer[currentInWorkingBuffer] = data[currentInDataBuffer];
                    currentInWorkingBuffer++;
                }

                currentInDataBuffer++;
            }

            if (currentInWorkingBuffer > 0)
            {
                base.OnDataReceived(this, new DataReceivedEventArgs(currentInWorkingBuffer, 0, _postprocessingBuffer, isGoageadReceived));
            }
        }
    }
}