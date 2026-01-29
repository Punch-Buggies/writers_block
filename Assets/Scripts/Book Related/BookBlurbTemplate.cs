using UnityEngine;
using System.Collections.Generic;

public class BookBlurbTemplate
{
    public string baseText;
    public List<TemplateSlot> slots;

    public string genre;

    public BookBlurbTemplate(string baseText, List<TemplateSlot> slots, string genre)
    {
        this.baseText = baseText;
        this.slots = slots;
        this.genre = genre;
    }
}

public enum WordType
{
    Adjective,
    Catchphrase,
    Person,
    Place,
    Thing,
    Name
}

[System.Serializable]
public class TemplateSlot
{
    public string slotId;   // character1, character2, etc
    public WordType type;   // Person, Place, etc

    public TemplateSlot(string id, WordType type)
    {
        slotId = id;
        this.type = type;
    }
}

