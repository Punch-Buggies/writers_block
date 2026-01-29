using UnityEngine;
using System.Collections.Generic;

public class BookBlurbTemplate
{
    public string baseText;
    public List<TemplateSlot> slots;

    public BookBlurbTemplate(string baseText, List<TemplateSlot> slots)
    {
        this.baseText = baseText;
        this.slots = slots;
    }
}

public enum WordType
{
    Adjective,
    Verb,
    Person,
    Place,
    Object,
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

