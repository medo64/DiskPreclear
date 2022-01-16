namespace DiskPreclear;

internal enum BlockState {
    None = 0,
    Validated,
    ValidationError,
    AccessError,
}
