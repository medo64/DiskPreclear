using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskPreclear;

[DebuggerDisplay("{Name} at {Path}")]
internal sealed class LogicalVolume {

    internal LogicalVolume(string name) {
        Name = name;
    }


    /// <summary>
    /// Gets volume name.
    /// </summary>
    public string Name { get; private set; }

    private string VolumeNameWithoutSlash {
        get {
            return RemoveLastBackslash(Name);
        }
    }

    /// <summary>
    /// Returns drive letter with colon (:) but without trailing backslash (\), volume path, or null if volume has no drive assigned.
    /// </summary>
    public string? Path {
        get {
            var volumePaths = new StringBuilder(4096);
            if (NativeMethods.GetVolumePathNamesForVolumeName(Name, volumePaths, volumePaths.Capacity, out var _)) {
                foreach (var path in volumePaths.ToString().Split('\0')) {
                    if (path.Length == 3) {
                        return path[..2];  // remove trailing backslash
                    } else if (path.Length > 0) {
                        return path;
                    }
                }
            }
            return null;
        }
    }


    private int? _physicalDiskNumber;
    public int? PhysicalDiskNumber {
        get {
            FillExtentInfo();
            return _physicalDiskNumber;
        }
    }

    private long? _physicalDiskExtentOffset;
    public long? PhysicalDiskExtentOffset {
        get {
            FillExtentInfo();
            return _physicalDiskExtentOffset;
        }
    }

    private long? _physicalDriveExtentLength;
    public long? PhysicalDriveExtentLength {
        get {
            FillExtentInfo();
            return _physicalDriveExtentLength;
        }
    }

    public int? VolumeIndex { get; private set; }


    public override bool Equals(object? obj) {
        return (obj is LogicalVolume other) && Name.Equals(other.Name);
    }

    public override int GetHashCode() {
        return Name.GetHashCode();
    }


    #region Static

    public static IList<LogicalVolume> GetAllVolumes() {
        var volumes = new List<LogicalVolume>();

        var sb = new StringBuilder(50);
        var volumeSearchHandle = NativeMethods.FindFirstVolume(sb, sb.Capacity);
        if (volumeSearchHandle.IsInvalid == false) {
            do {
                volumes.Add(new LogicalVolume(sb.ToString()));
            } while (NativeMethods.FindNextVolume(volumeSearchHandle, sb, sb.Capacity));
        }
        volumeSearchHandle.Close();

        volumes.Sort(
            delegate (LogicalVolume item1, LogicalVolume item2) {
                if (item1.PhysicalDiskNumber < item2.PhysicalDiskNumber) {
                    return -1;
                } else if (item1.PhysicalDiskNumber > item2.PhysicalDiskNumber) {
                    return 1;
                } else if (item1.PhysicalDiskExtentOffset < item2.PhysicalDiskExtentOffset) {
                    return -1;
                } else if (item1.PhysicalDiskExtentOffset > item2.PhysicalDiskExtentOffset) {
                    return 1;
                } else {
                    return 0;
                }
            });


        var volumeIndex = 0;
        int? lastDrive = null;
        foreach (var volume in volumes) {
            if (lastDrive != volume.PhysicalDiskNumber) {
                volumeIndex = 0;
                lastDrive = volume.PhysicalDiskNumber;
            } else {
                volumeIndex += 1;
            }
            volume.VolumeIndex = volumeIndex;
        }

        return volumes.AsReadOnly();
    }


    private static string? ParseDriveLetter(string driveLetter) {
        if (driveLetter == null) { return null; }
        driveLetter = driveLetter.Trim().ToUpperInvariant();
        if (!(driveLetter.EndsWith("\\", StringComparison.Ordinal))) { driveLetter += "\\"; }
        if ((driveLetter.Length != 3) || (driveLetter[0] < 'A') || (driveLetter[0] > 'Z') || (driveLetter[1] != ':') || (driveLetter[2] != '\\')) { return null; }
        return driveLetter;
    }

    private static string RemoveLastBackslash(string text) {
        if (text.EndsWith("\\", StringComparison.Ordinal)) {
            return text.Remove(text.Length - 1);
        } else {
            return text;
        }
    }

    #endregion


    #region Helper

    private bool _hasExtentInfo = false;
    private void FillExtentInfo() {
        if (_hasExtentInfo) { return; }

        if (GetExtentInfo(VolumeNameWithoutSlash, out var diskNumber, out var startingOffset, out var extentLength)) {
            _physicalDiskNumber = diskNumber;
            _physicalDiskExtentOffset = startingOffset;
            _physicalDriveExtentLength = extentLength;
        }

        _hasExtentInfo = true;
    }

    private static bool GetExtentInfo(string volumeNameWithoutSlash, out int diskNumber, out long startingOffset, out long extentLength) {
        var volumeHandle = NativeMethods.CreateFile(volumeNameWithoutSlash, 0, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
        if (volumeHandle.IsInvalid == false) {
            var de = new NativeMethods.VOLUME_DISK_EXTENTS {
                NumberOfDiskExtents = 1
            };
            int bytesReturned = 0;
            if (NativeMethods.DeviceIoControl(volumeHandle, NativeMethods.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, ref de, Marshal.SizeOf(de), ref bytesReturned, IntPtr.Zero)) {
                if (bytesReturned > 0) {
                    diskNumber = de.Extents.DiskNumber;
                    startingOffset = de.Extents.StartingOffset;
                    extentLength = de.Extents.ExtentLength;
                    return true;
                }
            }
        }

        diskNumber = 0;
        startingOffset = 0;
        extentLength = 0;
        return false;
    }

    #endregion


    private static class NativeMethods {

        public const UInt32 FILE_SHARE_READ = 0x1;
        public const UInt32 FILE_SHARE_WRITE = 0x2;
        public const UInt32 OPEN_EXISTING = 0x3;
        public const UInt32 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x560000;


        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_EXTENT {
            public Int32 DiskNumber;
            public Int64 StartingOffset;
            public Int64 ExtentLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VOLUME_DISK_EXTENTS {
            public Int32 NumberOfDiskExtents;
            public DISK_EXTENT Extents;
        }


        [DllImport("kernel32.dll", EntryPoint = "DeleteVolumeMountPointW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DeleteVolumeMountPoint(
            [In][MarshalAs(UnmanagedType.LPWStr)] String lpszVolumeMountPoint
        );

        [DllImport("kernel32.dll", EntryPoint = "GetVolumeNameForVolumeMountPointW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetVolumeNameForVolumeMountPoint(
            [In][MarshalAs(UnmanagedType.LPWStr)] String lpszVolumeMountPoint,
            [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName,
            Int32 cchBufferLength
        );

        [DllImport("kernel32.dll", EntryPoint = "GetVolumePathNamesForVolumeNameW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetVolumePathNamesForVolumeName(
            [In][MarshalAs(UnmanagedType.LPWStr)] String lpszVolumeName,
            [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumePathNames,
            Int32 cchBufferLength,
            [Out] out Int32 lpcchReturnLength);

        [DllImport("kernel32.dll", EntryPoint = "SetVolumeMountPointW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetVolumeMountPoint([In][MarshalAs(UnmanagedType.LPWStr)] String lpszVolumeMountPoint, [In][MarshalAs(UnmanagedType.LPWStr)] String lpszVolumeName);


        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
        public static extern SafeFileHandle CreateFile([In][MarshalAs(UnmanagedType.LPWStr)] String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, [In] IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, [In] IntPtr hTemplateFile);

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl([In] SafeFileHandle hDevice, UInt32 dwIoControlCode, [In] IntPtr lpInBuffer, Int32 nInBufferSize, ref VOLUME_DISK_EXTENTS lpOutBuffer, Int32 nOutBufferSize, ref Int32 lpBytesReturned, IntPtr lpOverlapped);


        [DllImport("kernel32.dll", EntryPoint = "FindFirstVolumeW", SetLastError = true)]
        public static extern SearchSafeHandle FindFirstVolume([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName, Int32 cchBufferLength);

        [DllImport("kernel32.dll", EntryPoint = "FindNextVolumeW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean FindNextVolume(
            SearchSafeHandle hFindVolume,
            [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName,
            Int32 cchBufferLength
        );


        #region SafeHandles

        public class SearchSafeHandle : SafeHandleMinusOneIsInvalid {

            public SearchSafeHandle()
                : base(true) { }


            protected override bool ReleaseHandle() {
                return FindVolumeClose(handle);
            }

            public override string ToString() {
                return handle.ToString();
            }

            [DllImport("kernel32.dll", EntryPoint = "FindVolumeClose")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FindVolumeClose([In] IntPtr hFindVolume);

        }

        #endregion

    }

}
