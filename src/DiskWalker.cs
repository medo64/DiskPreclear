using System;
using System.Security.Cryptography;

namespace DiskPreclear;

internal sealed class DiskWalker {

    public DiskWalker(PhysicalDisk disk) {
        Disk = disk;
        DiskSize = disk.Size;
        if (DiskSize % BlockSize == 0) {
            BlockCount = (int)(DiskSize / BlockSize);
        } else {
            BlockCount = (int)(DiskSize / BlockSize) + 1;
        }

        BlockIndices = new long[BlockCount];
        long nextIndex = Random.Shared.Next(BlockCount);
        for (var i = 0; i < BlockCount; i++) {
            BlockIndices[i] = nextIndex;
            nextIndex = (nextIndex + Step) % BlockCount;
        }
    }

    private const long Step = 2147483647;  // just make it a prime
    private const ulong BlockSize = 8 * 1024 * 1024;  // 8 MB

    private readonly long[] BlockIndices;
    private readonly PhysicalDisk Disk;
    private readonly ulong DiskSize;

    /// <summary>
    /// Gets total block count.
    /// </summary>
    public int BlockCount { get; init; }

    /// <summary>
    /// Gets/sets index used to calculate offset.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets block at given index.
    /// </summary>
    public long BlockIndex => BlockIndices[Index];

    /// <summary>
    /// Gets start offset for current block.
    /// </summary>
    public ulong OffsetStart {
        get { return (ulong)BlockIndex * BlockSize; }
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
