using System;
using System.Security.Cryptography;

namespace DiskPreclear;

internal sealed class DiskWalker {

    public DiskWalker(PhysicalDisk disk) {
        Disk = disk;
        DiskSize = disk.Size;
        if (DiskSize % BlockSize == 0) {
            BlockCount = (long)(DiskSize / BlockSize);
        } else {
            BlockCount = (long)(DiskSize / BlockSize) + 1;
        }
    }

    private const long Step = 2147483647;
    private const ulong BlockSize = 8 * 1024 * 1024;  // 8 MB

    private readonly PhysicalDisk Disk;
    private readonly ulong DiskSize;

    /// <summary>
    /// Gets total block count.
    /// </summary>
    public long BlockCount { get; init; }

    private long _index;
    /// <summary>
    /// Gets/sets index used to calculate offset.
    /// </summary>
    public long Index {
        get { return _index; }
        set {
            if (value > _index) {  // go forward
                while (value > _index) {
                    OffsetBlock = (OffsetBlock + Step) % BlockCount;
                    _index += 1;
                }
            } else if (value < _index) {  // go in reverse

            }
        }
    }

    /// <summary>
    /// Gets offset block.
    /// </summary>
    public long OffsetBlock {
        get; private set;
    }

    /// <summary>
    /// Gets start offset for current block.
    /// </summary>
    public ulong OffsetStart {
        get { return (ulong)OffsetBlock * BlockSize; }
    }

    /// <summary>
    /// Gets data length for current index.
    /// </summary>
    public int OffsetLength {
        get {
            if (OffsetStart + BlockSize > DiskSize) {
                return (int)(DiskSize - OffsetStart);
            } else {
                return (int)BlockSize;
            }
        }
    }


    /// <summary>
    /// Writes given data.
    /// </summary>
    /// <param name="data">Random data.</param>
    public bool Write(byte[] data) {
        if (data == null) { return false; }
        if (data.Length != OffsetLength) { return false; }
        return Disk.Write(OffsetStart, data);
    }

    /// <summary>
    /// Returns data that was read.
    /// </summary>
    public bool Read(byte[] data) {
        if (data == null) { return false; }
        if (data.Length != OffsetLength) { return false; }
        return Disk.Read(OffsetStart, data);
    }

    public static bool Validate(byte[] bytesWritten, byte[] bytesRead) {
        if (bytesWritten == null) { return false; }
        if (bytesRead == null) { return false; }
        if (bytesWritten.Length != bytesRead.Length) { return false; }

        var len = bytesWritten.Length;
        for (var i = 0; i < len; i++) {
            if (bytesWritten[i] != bytesRead[i]) { return false; }
        }
        return true;
    }


    private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

    /// <summary>
    /// Returns random data for given offset.
    /// </summary>
    /// <returns></returns>
    public byte[] GetRandomData() {
        var bytes = new byte[OffsetLength];
        Rnd.GetBytes(bytes);
        return bytes;
    }

}
