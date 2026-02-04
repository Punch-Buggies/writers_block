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
    Name,

    //for grammar words (pronouns, a/an, etc) : slotId is of the independant word this word depends on
    Pronoun, // she/he/they
    IndefiniteArticle //    a/an
}
public enum Perspective
{
    firstPerson,
    secondPerson,
    thirdPerson
}

[System.Serializable]
public class TemplateSlot
{
    public string slotId;   // character1, character2, etc
    public WordType type;   // Person, Place, etc
    public string parentId; //for dependant words only
    public Perspective perspective; //for gendered pronouns only

    public TemplateSlot(string id, WordType type, string parentId=null, Perspective perspective=Perspective.thirdPerson)
    {
        if (string.IsNullOrEmpty(id))
        {
            
            throw new System.ArgumentException("slotId cannot be null or empty");
        }
        slotId = id;
        this.type = type;
        this.perspective = perspective;
        // Dependent word types must reference a parent slot
        if (type == WordType.Pronoun || type == WordType.IndefiniteArticle)
        {
             if (string.IsNullOrEmpty(parentId))
            {
                throw new System.ArgumentException(
                    $"Dependent slot '{id}' must define a parentId."
                );
            }
            this.parentId = parentId;
        }
        else
        {
            // Independent words reference themselves
            this.parentId = id;
        }
    }
}

