namespace DiskPreclear;

internal class InitObjectState {

    public InitObjectState(DiskWalker walker, RandomKind randomKind) {
        Walker = walker;
        RandomKind = randomKind;
    }

    public DiskWalker Walker { get; init; }
    public RandomKind RandomKind { get; init; }

}
