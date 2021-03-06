﻿/*
 * This file is part of the CatLib package.
 *
 * (c) CatLib <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io/
 */

using CatLib.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CatLib.IO
{
    /// <summary>
    /// The wrapper stream.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WrapperStream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperStream"/> class.
        /// </summary>
        public WrapperStream()
        {
            BaseStream = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperStream"/> class.
        /// </summary>
        /// <param name="stream">The base stream.</param>
        public WrapperStream(Stream stream)
        {
            Guard.Requires<ArgumentNullException>(stream != null);
            BaseStream = stream;
        }

        /// <summary>
        /// Gets the base stream.
        /// </summary>
        public Stream BaseStream { get; }

        /// <inheritdoc />
        public override bool CanRead => BaseStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => BaseStream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => BaseStream.CanWrite;

        /// <inheritdoc />
        public override long Position
        {
            get => BaseStream.Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <inheritdoc />
        public override long Length => BaseStream.Length;

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <inheritdoc />
        public override void Flush()
        {
            BaseStream.Flush();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }
    }
}
