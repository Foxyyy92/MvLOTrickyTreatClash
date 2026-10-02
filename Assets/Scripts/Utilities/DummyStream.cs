using System.IO;

namespace NSMB.Utilities {
    /// <summary>
    /// Just keeps track of length- used to calculate the size of BinaryReplayFiles on WebGL 
    /// without having to actually write to a MemoryStream (since a FileStream won't work)
    /// </summary>
    public class DummyStream : Stream {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => length;
        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        private long length;

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value) {
            length = value;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            length += count;
        }
    }
}
