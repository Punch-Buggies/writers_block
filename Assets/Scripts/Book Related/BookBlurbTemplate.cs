using UnityEngine;
using System.Collections.Generic;

public class BookBlurbTemplate
{
    public string baseText; //raw string
    public List<TemplateSlot> slots; //list of variables

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
    //independants:
    Adjective,
    Catchphrase,
    Person,
    Place,
    Thing,
    //dependants:
    Name, // gendered
    Pronoun, // gendered pronouns only
    IndefiniteArticle //    a/an

    // note gendered dependants must depend on an independant word that has a gender (character, WordType.Person)
}
public enum Conjugation
{
    Subject, // she/he/they
    Object,//   her/him/them
    PossessivePro,//    hers/his/theirs
    PossessiveAdj//     her/his/their
}

[System.Serializable]
public class TemplateSlot
{
    public string slotId;   // unique id
    public WordType type;   // Person, Place, etc
    public string parentId; //for dependant words only (id of word it depends on, must be an independant word)
    public Conjugation perspective; //for pronouns only

    public TemplateSlot(string id, WordType type, string parentId=null, Conjugation perspective=Conjugation.PossessivePro)
    {
        if (string.IsNullOrEmpty(id))
        {
            
            throw new System.ArgumentException("slotId cannot be null or empty");
        }
        slotId = id;
        this.type = type;
        this.perspective = perspective;
        // Dependent word types must reference a parent slot
        if (type == WordType.Pronoun || type == WordType.IndefiniteArticle || type == WordType.Name)
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

