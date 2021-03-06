using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace DiskPreclear;

[DebuggerDisplay("{Number}: {SizeInGB} GB")]
internal sealed class PhysicalDisk : IDisposable {

    private PhysicalDisk(int number) {
        Number = number;
    }

    /// <summary>
    /// Gets disk number.
    /// </summary>
    public int Number { get; init; }

    /// <summary>
    /// Gets disk path.
    /// </summary>
    public string Path {
        get {
            return @"\\.\PhysicalDrive" + Number.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets disk size or 0 if size cannot be determined.
    /// </summary>
    public ulong Size {
        get {
            var diskHandle = NativeMethods.CreateFile(Path,
                                                      0,
                                                      NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE,
                                                      IntPtr.Zero,
                                                      NativeMethods.OPEN_EXISTING,
                                                      0,
                                                      IntPtr.Zero);
            if (!diskHandle.IsInvalid) {
                var diskGeometry = new NativeMethods.DISK_GEOMETRY_EX();
                if (NativeMethods.DeviceIoControl(diskHandle, NativeMethods.IOCTL_DISK_GET_DRIVE_GEOMETRY_EX, IntPtr.Zero, 0, ref diskGeometry, Marshal.SizeOf(diskGeometry), out var bytesReturned, IntPtr.Zero)) {
                    if (bytesReturned > 0) {
                        return diskGeometry.DiskSize;
                    }
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// Gets disk size in GB.
    /// </summary>
    public int SizeInGB {
        get {
            var size = Size;
            if (size > 8001563222016) {  // larger than 8000 GB
                return (int)(size / 1000000000);  // proper formula is too annoying
            } else if (size > 80026361856) {  // between 80 and 8000 GB
                return (int)((size / 4096 - 12212046) / 244188 + 50);
            } else {  // just divide for smaller disks
                return (int)(size / 1000000000);
            }
        }
    }

    public IList<LogicalVolume> GetLogicalVolumes() {
        var volumes = new List<LogicalVolume>();
        foreach (var volume in LogicalVolume.GetAllVolumes()) {
            if (volume.PhysicalDiskNumber == Number) {
                volumes.Add(volume);
            }
        }
        return volumes.AsReadOnly();
    }

    public IList<string> GetLogicalVolumePaths() {
        var paths = new List<string>();
        foreach (var volume in GetLogicalVolumes()) {
            var path = volume.Path;
            if (path != null) {
                paths.Add(path);
            }
        }
        return paths.AsReadOnly();
    }

    /// <summary>
    /// Returns text describing the object.
    /// </summary>
    public override string ToString() {
        var sbPaths = new StringBuilder();
        foreach (var path in GetLogicalVolumePaths()) {
            if (path != null) {
                if (sbPaths.Length > 0) { sbPaths.Append(", "); }
                sbPaths.Append(path);
            }
        }

        if (sbPaths.Length > 0) {
            return $"Physical Disk {Number}: {SizeInGB:#,##0} GB ({sbPaths})";
        } else {
            return $"Physical Disk {Number}: {SizeInGB:#,##0} GB";
        }
    }


    private SafeFileHandle? DiskHandle = null;

    /// <summary>
    /// Opens disk for access operations.
    /// </summary>
    public void Open(bool allowRead, bool allowWrite) {
        if (DiskHandle == null) {
            uint accessMode = 0;
            if (allowRead) { accessMode |= NativeMethods.GENERIC_READ; }
            if (allowWrite) { accessMode |= NativeMethods.GENERIC_WRITE; }

            uint shareMode = 0;
            if (allowRead && allowWrite) {
                shareMode |= NativeMethods.FILE_SHARE_READ;
            } else if (allowRead) {
                shareMode |= NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE;
            } else if (allowWrite) {
                shareMode |= NativeMethods.FILE_SHARE_READ;
            }

            uint flags = NativeMethods.FILE_FLAG_NO_BUFFERING | NativeMethods.FILE_FLAG_WRITE_THROUGH;

            var diskHandle = NativeMethods.CreateFile(Path,
                                                      accessMode,
                                                      shareMode,
                                                      IntPtr.Zero,
                                                      NativeMethods.OPEN_EXISTING,
                                                      flags,
                                                      IntPtr.Zero);
            if (diskHandle.IsInvalid) { throw new Win32Exception(); }
            DiskHandle = diskHandle;
        } else {
            throw new InvalidOperationException("Disk already open.");
        }
    }

    /// <summary>
    /// Closes disk access.
    /// </summary>
    public void Close() {
        if (DiskHandle != null) {
            DiskHandle.Dispose();
            DiskHandle = null;
        } else {
            throw new InvalidOperationException("Disk already closed.");
        }
    }

    /// <summary>
    /// Writes bytes to the specified location.
    /// </summary>
    /// <param name="buffer">Buffer.</param>
    /// <param name="offset">Start offset.</param>
    /// <param name="length">Buffer lenght.</param>
    /// <returns></returns>
    public IOStatus Write(byte[] buffer, ulong offset, int length) {
        if (DiskHandle == null) { return IOStatus.InternalError; }

        var okMove = NativeMethods.SetFilePointerEx(DiskHandle,
                                                    offset,
                                                    out var _,
                                                    NativeMethods.FILE_BEGIN);
        if (!okMove) {
            Trace.WriteLine("Win32 write error (move): " + Marshal.GetLastWin32Error());
            return IOStatus.IOError; ;
        }

        var ok = NativeMethods.WriteFile(DiskHandle,
                                         buffer,
                                         length,
                                         out var _,
                                         IntPtr.Zero);
        if (ok) {
            return IOStatus.Ok;
        } else {
            Trace.WriteLine("Win32 write error: " + Marshal.GetLastWin32Error());
            return IOStatus.IOError;
        }
    }

    /// <summary>
    /// Reads bytes fromt the specified location.
    /// </summary>
    /// <param name="buffer">Buffer.</param>
    /// <param name="offset">Start offset.</param>
    /// <param name="length">Buffer lenght.</param>
    public IOStatus Read(byte[] buffer, ulong offset, int length) {
        if (DiskHandle == null) { return IOStatus.InternalError; }

        var okMove = NativeMethods.SetFilePointerEx(DiskHandle,
                                                    offset,
                                                    out var _,
                                                    NativeMethods.FILE_BEGIN);
        if (!okMove) {
            Trace.WriteLine("Win32 read error (move): " + Marshal.GetLastWin32Error());
            return IOStatus.IOError;
        }

        var ok = NativeMethods.ReadFile(DiskHandle,
                                        buffer,
                                        length,
                                        out var _,
                                        IntPtr.Zero);
        if (ok) {
            return IOStatus.Ok;
        } else {
            Trace.WriteLine("Win32 read error: " + Marshal.GetLastWin32Error());
            return IOStatus.IOError;
        }
    }


    /// <summary>
    /// Dispose disk resources.
    /// </summary>
    public void Dispose() {
        Close();
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Returns list of all detected disks.
    /// </summary>
    public static IList<PhysicalDisk> GetAllDisks() {
        var disks = new List<PhysicalDisk>();
        for (var number = 0; number < 64; number++) {
            using var diskHandle = NativeMethods.CreateFile(@"\\.\PhysicalDrive" + number.ToString(CultureInfo.InvariantCulture),
                                                            0,
                                                            NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE,
                                                            IntPtr.Zero,
                                                            NativeMethods.OPEN_EXISTING,
                                                            0,
                                                            IntPtr.Zero);
            if (!diskHandle.IsInvalid) {
                disks.Add(new PhysicalDisk(number));
            }
        }
        return disks.AsReadOnly();
    }


    private static class NativeMethods {

        public const UInt32 FILE_SHARE_READ = 0x01;
        public const UInt32 FILE_SHARE_WRITE = 0x02;
        public const UInt32 OPEN_EXISTING = 0x03;

        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;
        public const UInt32 FILE_FLAG_WRITE_THROUGH = 0x80000000;

        public const UInt32 FILE_BEGIN = 0x00;

        public const UInt32 IOCTL_DISK_GET_DRIVE_GEOMETRY_EX = 0x000700A0;


        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_GEOMETRY_EX {
            public DISK_GEOMETRY Geometry;
            public UInt64 DiskSize;
            public Byte Data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_GEOMETRY {
            public UInt64 Cylinders;
            public UInt32 MediaType;
            public UInt32 TracksPerCylinder;
            public UInt32 SectorsPerTrack;
            public UInt32 BytesPerSector;
        }


        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(
            [In][MarshalAs(UnmanagedType.LPWStr)] String lpFileName,
            [In] UInt32 dwDesiredAccess,
            [In] UInt32 dwShareMode,
            [In] IntPtr lpSecurityAttributes,
            [In] UInt32 dwCreationDisposition,
            [In] UInt32 dwFlagsAndAttributes,
            [In] IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DeviceIoControl(
            [In] SafeFileHandle hDevice,
            [In] UInt32 dwIoControlCode,
            [In] IntPtr lpInBuffer,
            [In] Int32 nInBufferSize,
            [In, Out] ref DISK_GEOMETRY_EX lpOutBuffer,
            [In] Int32 nOutBufferSize,
            [Out] out Int32 lpBytesReturned,
            [In] IntPtr lpOverlapped
        );

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean ReadFile(
            SafeFileHandle hFile,
            [Out] Byte[] lpbuffer,
            [In] Int32 nNumberOfBytesToRead,
            [Out] out Int32 lpNumberOfBytesRead,
            [In, Out] IntPtr lpOverlapped
        );

        [DllImport("kernel32.dll", EntryPoint = "WriteFile", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean WriteFile(
            SafeFileHandle hFile,
            [In] Byte[] lpBuffer,
            [In] Int32 nNumberOfBytesToWrite,
            [Out] out Int32 lpNumberOfBytesWritten,
            [In, Out] IntPtr lpOverlapped
        );

        [DllImport("kernel32.dll", EntryPoint = "SetFilePointerEx", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetFilePointerEx(
            SafeFileHandle hFile,
            [In] UInt64 liDistanceToMove,
            [Out] out UInt64 lpNewFilePointer,
            [In] UInt32 dwMoveMethod
        );

    }

}
