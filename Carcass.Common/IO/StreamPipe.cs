using System;
using System.IO;

namespace Carcass.Common.IO
{
    public class StreamPipe
    {
        public const int DefaultBufferSize = 1048576;

        /// <summary>
        /// Transfers the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="buffSize">Size of the buff.</param>
        public static void Transfer(Stream source, Stream destination, int bufferSize = DefaultBufferSize)
        {
            if (!source.CanRead)
            {
                throw new ArgumentException("Source stream is not readable", "source");
            }
            
            if (!destination.CanWrite)
            {
                throw new ArgumentException("Destination stream is not writable", "destination");
            }

            var buffer = new byte[bufferSize];

            var length = 0;
            while ((length = source.Read(buffer, 0, bufferSize)) > 0)
            {
                destination.Write(buffer, 0, length);
            }
        }
    }
}