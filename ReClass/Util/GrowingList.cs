namespace ReClass.Util;

public class GrowingList<T> {
    private readonly List<T> list;

    public T DefaultValue { get; set; }

    public int Count => list.Count;

    public T this[int index] {
        get {
            CheckIndex(index);

            return list[index];
        }
        set {
            CheckIndex(index);

            list[index] = value;
        }
    }

    public GrowingList() {
        list = [];
    }

    public GrowingList(T defaultValue)
        : this() {
        DefaultValue = defaultValue;
    }

    private void GrowToSize(int size) {
        list.Capacity = size;

        for (var i = list.Count; i <= size; ++i) {
            list.Add(DefaultValue);
        }
    }

    private void CheckIndex(int index) {
        if (index >= list.Count) {
            GrowToSize(index);
        }
    }
}
