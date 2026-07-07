/*
 Hi.  Let's take a moment to talk about this class.
 1) It is serializable, which means it can be shown in the unity editor
 2) It's a class, which means you can pull a ref from a list, and change it
    and have the copy in the list update.
 */

using System.Collections.Generic;

public class MutableTuple<Item1, Item2>
{
    public Item1 item1;
    public Item2 item2;

    public MutableTuple() { }

    public MutableTuple(Item1 i1, Item2 i2)
    {
        this.item1 = i1;
        this.item2 = i2;
    }

    public static List<KeyValuePair<Item1, Item2>> ToKeyValuePair(List<MutableTuple<Item1, Item2>> list)
    {
        List<KeyValuePair<Item1, Item2>> newList = new List<KeyValuePair<Item1, Item2>>();
        foreach (MutableTuple<Item1, Item2> item in list)
        {
            newList.Add(new KeyValuePair<Item1, Item2>(item.item1, item.item2));
        }
        return newList;
    }
}

public class MutableTuple<Item1, Item2, Item3>
{
    public Item1 item1;
    public Item2 item2;
    public Item3 item3;

    public MutableTuple() { }

    public MutableTuple(Item1 i1, Item2 i2, Item3 i3)
    {
        this.item1 = i1;
        this.item2 = i2;
        this.item3 = i3;
    }
}