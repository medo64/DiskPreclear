using System;

namespace DiskPreclear;

internal sealed class DiskWalker : IDisposable {

    public DiskWalker(PhysicalDisk disk, int blockSizeInMB) {
        Disk = disk;
        BlockSize = (ulong)blockSizeInMB * 1024 * 1024;

        DiskSize = disk.Size;
        if (DiskSize % BlockSize == 0) {
            BlockCount = (int)(DiskSize / BlockSize);
        } else {
            BlockCount = (int)(DiskSize / BlockSize) + 1;
        }

        using var lw = new Medo.Diagnostics.LifetimeWatch("Block randomization");

        BlockIndices = new int[BlockCount];
        for (var i = 0; i < BlockCount; i++) {
            BlockIndices[i] = i;
        }

        var rnd = Random.Shared;
        for (var i = 0; i < BlockCount; i++) {
            var j = rnd.Next(BlockCount);
            (BlockIndices[i], BlockIndices[j]) = (BlockIndices[j], BlockIndices[i]);  // swap
        }
    }

    private readonly ulong BlockSize;

    private readonly int[] BlockIndices;
    private readonly PhysicalDisk Disk;
    private readonly ulong DiskSize;

    /// <summary>
    /// Gets total block count.
    /// </summary>
    public int BlockCount { get; init; }

    /// <summary>
    /// Gets maximum block size.
    /// </summary>
    public int MaxBlockSize => (int)BlockSize;

    /// <summary>
    /// Gets/sets index used to calculate offset.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets block at given index.
    /// </summary>
    public int BlockIndex => BlockIndices[Index];

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
    /// Gets/sets if reads are allowed.
    /// </summary>
    public bool AllowRead { get; set; }

    /// <summary>
    /// Gets sets if writes are allowed.
    /// </summary>
    public bool AllowWrite { get; set; }

    /// <summary>
    /// Opens disk for access operations.
    /// </summary>
    public bool Open(bool allowRead, bool allowWrite) {
        return Disk.Open(allowRead, allowWrite);
    }

    /// <summary>
    /// Closes disk access.
    /// </summary>
    public bool Close() {
        return Disk.Close();
    }

    /// <summary>
    /// Writes given data.
    /// </summary>
    /// <param name="data">Random data.</param>
    public bool Write(byte[] data) {
        if (!AllowWrite) { return false; }
        return Disk.Write(data, OffsetStart, OffsetLength);
    }

    /// <summary>
    /// Returns data that was read.
    /// </summary>
    public bool Read(byte[] data) {
        if (!AllowRead) { return false; }
        return Disk.Read(data, OffsetStart, OffsetLength);
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


    /// <summary>
    /// Dispose walker resources.
    /// </summary>
    public void Dispose() {
        Close();
        GC.SuppressFinalize(this);
    }

}
