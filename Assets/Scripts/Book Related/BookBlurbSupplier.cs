using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class BookBlurbSupplier : MonoBehaviour
// handles associated word dictionaries and templates used to generate blurbs 
// dictionary structure:
//   setting_words   = { key: settingType   -> SettingWordSet }
//   character_words = { key: characterType -> CharacterWordSet }
//   names           = { key: gender        -> [names] }
//   templates       = { key: genre         -> [BookBlurbTemplate] }
{
    [SerializeField] private TextAsset blurbCSV;
    Dictionary<string, SettingWordSet> setting_words;
    Dictionary<string, CharacterWordSet> character_words;
    Dictionary<Gender, List<string>> names;
    Dictionary<string, List<BookBlurbTemplate>> templates;

    void Awake()
    {
        setting_words = new Dictionary<string, SettingWordSet>();
        character_words = new Dictionary<string, CharacterWordSet>();
        names = new Dictionary<Gender, List<string>>();
        templates = new Dictionary<string, List<BookBlurbTemplate>>();

        // BuildSampleData(); // todo: replace with CSV retrieval
        BuildDataFromCSV();
    }

    // getters
    public List<string> GetAdjectives(string key)   => character_words[key].adjectives;
    public List<string> GetCatchphrases(string key) => character_words[key].catchphrases;

    public List<string> GetPeople(string key) => setting_words[key].people;
    public List<string> GetPlaces(string key) => setting_words[key].places;
    public List<string> GetThings(string key) => setting_words[key].things;

    public List<string> GetNames(Gender key) => names[key];

    public List<BookBlurbTemplate> GetTemplates(string key) => templates[key];


    void addCharacterData(string value,string adjectives, string catchphrases)
    {
        //(1) parse into lists
        List<string> adj = new List<string>(adjectives.Split('|'));
        List<string> cp = new List<string>(catchphrases.Split('|'));
        //(2) add data to dictionary
        character_words.Add(value, new CharacterWordSet{
            adjectives = adj,
            catchphrases = cp    
        });
        // Debug.Log($"{value}\nadjs: {string.Join(", ",GetAdjectives(value))} \ncps: {string.Join(", ", GetCatchphrases(value))}\n");
    }
    void addSettingData(string value, string people, string places, string things)
    {
        //(1) parse into lists
        List<string> peop = new List<string>(people.Split('|'));
        List<string> pla = new List<string>(places.Split('|'));
        List<string> thi = new List<string>(things.Split('|'));
        //(2) add data to dictionary
        setting_words.Add(value, new SettingWordSet
        {
           people = peop,
           places = pla,
           things = thi 
        });
        // Debug.Log($"{value}\npeople: {string.Join(", ",GetPeople(value))} \nplaces: {string.Join(", ", GetPlaces(value))}\nthings: {string.Join(", ", GetThings(value))}\n");
        
    }
    void addTemplateData(string value, string templates)
    {
        //(1) parse into lists
        List<string> temp = new List<string>();
        foreach (Match match in Regex.Matches(templates, "\".*?\""))
        {// regex match for quotations
            temp.Add(match.Value);
        }

    }

    void BuildDataFromCSV()
    {
        /* This function parses through the CSV and adds the info to the corresponding dictionary. First goes through some error checking, then goes through each line and switches to the correct addData function. */
        if (blurbCSV == null)
            {
                UnityEngine.Debug.LogError("You gotta attach the CSV file to the supplier object");
                return;
            }

        /* CSV file is structured as the following: 
        Value, Story Type, Adjectives, Catchphrases, People, Places, Things, Template */
        string[] lines = blurbCSV.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries); // Line Separation
        int num_headers = lines[0].Split(',').Length;
        Debug.Log("BLURB CSV Number of headers " + num_headers + ", total number of entries " + lines.Length);
        Debug.Log($"Last line raw: '{lines[lines.Length - 1]}'");
        
        // starts at i1 bc i0 are headers
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            // Debug.Log($"i{i} {line}");
            if (string.IsNullOrEmpty(line))
            { // check if the line is empty
                UnityEngine.Debug.LogError("There is an empty line? perhaps at the bottom, please delete it.");
                continue;
            }
            string[] columns = line.Split(','); // Column Separation
            if (columns.Length < num_headers)
            { // check for missing column
                UnityEngine.Debug.LogError("Please fill all the data, seems like you are missing something? -_-");
                continue;                
            }

            string value = columns[0].Trim();
            string storyType = columns[1].Trim();

            switch (storyType)
            // grab relevant columns and call addfunction
            {
                case "Setting":
                string people = columns[4].Trim();
                string places = columns[5].Trim();
                string things = columns[6].Trim();
                // all the adding is done here
                addSettingData(value, people, places, things);
                break;
                case "Character":
                string adjectives = columns[2].Trim();
                string catchphrases = columns[3].Trim();
                // all the adding is done here
                addCharacterData(value, adjectives, catchphrases);
                break;
                case "Genre":
                string templates = columns[7].Trim();
                // all the adding is done here
                addTemplateData(value, templates);
                break;
                default:
                Debug.Log("No CSV case found");
                break;
            }
        }
        
        // TEMPLATES -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
        templates.Add("Title", new List<BookBlurbTemplate>
        // note title's don't have access to original genre
        {
            new BookBlurbTemplate("{a} {thing} for a {thing}", new List<TemplateSlot>{new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"thing"), new TemplateSlot("thing", WordType.Thing)},"Title"),
            new BookBlurbTemplate("You Took my {heart} (I was Sleeping)", new List<TemplateSlot>{new TemplateSlot("heart", WordType.Thing)},"Title"),
            new BookBlurbTemplate("Super-{something}", new List<TemplateSlot>{new TemplateSlot("something", WordType.Thing)},"Title"),
            new BookBlurbTemplate("{a} {character} and {their} {thing}", new List<TemplateSlot>
            {
                new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"character"),
                new TemplateSlot("their", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
                new TemplateSlot("thing", WordType.Thing)
            },"Title"),
            new BookBlurbTemplate( "{character}'s {adjective} Story",
                new List<TemplateSlot>
                {
                    new TemplateSlot("adjective", WordType.Adjective)
                },"Title"),
        
            new BookBlurbTemplate("The {character}’s Journey", new List<TemplateSlot>{}, "Title"),
            new BookBlurbTemplate("{a} {setting} Story", new List<TemplateSlot>{new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"setting")}, "Title"),
            new BookBlurbTemplate("{a} {character}’s Guide to {thing1} and {thing2}", new List<TemplateSlot>{
                new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"character"),
                new TemplateSlot("thing1", WordType.Thing, plural:true),
                new TemplateSlot("thing2", WordType.Thing, plural:true)
                }, "Title"),
            new BookBlurbTemplate("{a} {adjective} Summer with You",
                new List<TemplateSlot>{
                new TemplateSlot("a", WordType.IndefiniteArticle, parentId:"adjective"), 
                new TemplateSlot("adjective", WordType.Adjective)}, 
                "Title"),
            new BookBlurbTemplate("{adjective} {character}: The Saga",
                new List<TemplateSlot>
                {
                    new TemplateSlot("adjective", WordType.Adjective)
                },"Title"),
            new BookBlurbTemplate("When two {people} Meet", new List<TemplateSlot>{new TemplateSlot("people", WordType.Person, plural:true)}, "Title"),
            new BookBlurbTemplate("The Cadence of {adjective} {people}", new List<TemplateSlot>{new TemplateSlot("adjective", WordType.Adjective), new TemplateSlot("people", WordType.Person, plural:true)}, "Title"),
            new BookBlurbTemplate("You Lie in the {place}", new List<TemplateSlot>{new TemplateSlot("place", WordType.Place)}, "Title"),
            new BookBlurbTemplate("The End of the World (for {adjective} {people})", new List<TemplateSlot>{new TemplateSlot("adjective", WordType.Adjective), new TemplateSlot("people", WordType.Person, plural:true)},"Title"),
            new BookBlurbTemplate("The Song of the {character}", new List<TemplateSlot>{}, "Title")
        });
        templates.Add("Romance", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("“Follow me,” {mc_a} moves {their_a} arm from {mc_b}’s shoulder to tug on the {person_b}’s arm. {mc_b} shoots {them_a} {a1} {adj1} glance, but follows {mc_a} nonetheless.\nAt the claw machine, {mc_a} lets go of {mc_b} to stand in front of {them_b} and raises {their_a} arms presenter style.\n“{mc_b}… Wait, what's your last name?“\n“{thing_b},“ {mc_b} laughed. {mc_a} liked {their_b} laugh. It was {adj} and you had to pay special attention to hear it.\n“{mc_b} {thing_b}: I, {mc_a}, hereby present to you the formidable… claw machine!“\n{mc_b} brings {their_b} hands to {their_b} face, hiding the tint of pink spreading over {their_b} face.\n“I will now display my superior gaming skills in a valiant battle against the wretched claw to bring forth to my princess the— oh I don’t fucking know, I’m winning you one of those stupid looking stuffed {thing1}, okay?“\n{mc_b} dragged {their_b} hands down enough that {mc_a} could see the {person_b}’s eyes.\n“Why am I a princess now?“ {mc_b} tried to fake annoyance, but {mc_a} caught the quirk of a smile {mc_b} was attempting to hide under {their_b} hands.\n“Cause you’re pretty like one.“\n“You’re so embarrassing.“ {mc_b} groaned.",
            new List<TemplateSlot>
            {
              new TemplateSlot("mc_a", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("person_b", WordType.Person),
              new TemplateSlot("mc_b", WordType.Name, parentId:"person_b"),
              new TemplateSlot("they_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Subject),
              new TemplateSlot("them_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Object),
              new TemplateSlot("their_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.PossessiveAdj), 
              new TemplateSlot("adj1", WordType.Adjective),
              new TemplateSlot("thing_b", WordType.Thing),
              new TemplateSlot("adj", WordType.Adjective),
              new TemplateSlot("thing1", WordType.Thing, plural:true),
              new TemplateSlot("a1", WordType.IndefiniteArticle, parentId:"adj1")
            },
            "Romance"),
            new BookBlurbTemplate("The pair made their way through the {place}, admiring the {thing1} as they passed. {mc_a} struggled to look away from {mc_b}. {they_b} {was} just so pretty, it made {them_a} a little crazy, {mc_a} thought. {they_a} really wanted to touch {mc_b}’s {adj1} chestnut hair. And when {they_a} stood just close enough that {mc_b} had to look up at {mc_a}--  god {their_b} eyes. {mc_a} lost {their_a} train of thought every time {mc_b} looked up at {them_a} with those {adj} doe eyes. That’s when {they_a}’d start spewing nonsense and making a fool of {them_a}self; something {they_a} had a natural gift for.",
            new List<TemplateSlot>
            {
                new TemplateSlot("mc_a", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("person_b", WordType.Person),
              new TemplateSlot("mc_b", WordType.Name, parentId:"person_b"),
              new TemplateSlot("they_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Subject),
              new TemplateSlot("them_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Object),
              new TemplateSlot("their_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.PossessiveAdj), 
              new TemplateSlot("place", WordType.Place),
              new TemplateSlot("thing1", WordType.Thing, plural:true),
              new TemplateSlot("adj1", WordType.Adjective),
              new TemplateSlot("adj", WordType.Adjective),
              new TemplateSlot("was", WordType.Verb, parentId:"person_b")
            },"Romance")
        });
        templates.Add("Action", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("{mc} did not move. {they_a} held {their_a} stance despite {their_a} body’s shaking. {they_a} listened for any sound of movement, but even the rustle of {thing1} in the wind had dwindled. \nThe lone hanging lightbulb of the {place1} flickered. Then it went dark. \nEyes still adjusting to the sudden darkness, {mc} sensed the {person1}’s presence. In a matter of seconds {they_a} had the rifle propped once again against {their_a} shoulder. Trusting {their_a} instincts, {mc} steadied {them_a}self, aimed into the darkness, and shot. The recoil shuddered through {their_a} {adj1} body. {they_a} blinked and made out the shape of the {adj} {person1} in front of {them_a}-- now hunched and producing a sort of whining sound. \n{mc} ran.\n{their_a} feet were sore, there were {thing2} stuck in {their_a} shoes from the {place1}. {their_a} shoulder ached from the pressure of the gunshot. Despite every protest of {their_a} body, {mc} ran. {they_a} pushed past {their_a} tiring muscles and panicked thoughts. {they_a} let {their_a} legs take {them_a} into the {place_b}, to the place {they_a} always ended up when he ran away.", 
            new List<TemplateSlot>
            {
              new TemplateSlot("mc", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("thing1", WordType.Thing, plural:true),
              new TemplateSlot("place1", WordType.Place),
              new TemplateSlot("person1", WordType.Person),
              new TemplateSlot("adj1", WordType.Adjective),
              new TemplateSlot("adj", WordType.Adjective),
              new TemplateSlot("thing2", WordType.Thing, plural:true),
              new TemplateSlot("place_b", WordType.Place)

            }, "Action"),
            new BookBlurbTemplate("{mc}, still paralyzed-- clearly, both fight and flight responses were forgone-- stared at the retreating {person_b}. {they_b} had lifted {their_b} grip off {mc}, though {they_b} still pinned {them_a} down by the legs, and tilted {their_b} head back and forth – as if searching for the source of the sound. A second bang rang out, and this time {mc} saw the {thing1} rip straight through the {person_b}’s open mouth, getting lodged in {their_b} cheek.\nBefore the {person_b} could follow through with {their_b} instinct to retaliate, {their_b} arms snap backwards in an unnatural way. {mc} feels the pressure on {their_a} legs alleviate as the {person_b} is lifted slightly in the air. With an awful cracking sound, the {person_b}’s head snaps backwards at an impossible angle. {mc} stares, mouth agape, at the {person_b} suspended above {them_a}, before {they_b} come falling back down. {mc} rolls off to the side just a fraction of time before the {person_b}’s now limp body meets the ground {they_a} had just occupied with a thud.", 
            new List<TemplateSlot>
            {
              new TemplateSlot("mc", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("person_b", WordType.Person),
              new TemplateSlot("they_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Subject),
              new TemplateSlot("them_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.Object),
              new TemplateSlot("their_b", WordType.Pronoun, parentId:"person_b", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("thing1", WordType.Thing)

            }, "Action")
        });
        templates.Add("Comedy", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("“Hey {mc_a}!” {mc_b} speaks up from where {they_b}’d been leaning against a wall between {mc_c} and {mc_d} by the {place1}, “You ready for our face off, or do you need time to prepare before I bruise your ego?”\n{mc_a} turns from {mc_c} and pretends to size {mc_b} up, “{phrase}, {mc_b}!”\n{mc_d} giggles, “Oh you are so screwed. {mc_b} is the best.”\n“Clearly none of you have experienced the god of {thing1}-fighter {them_a}self: {mc_a} Mc{mc_a}. I am about to rock your worlds just like I rocked your mom’s–”\n{mc_d} cringes, “Ugh, shut up {mc_a}.”\n“Awe don’t be like that, it’s just a little trash talk {mc_d}!”\n“You’ve gotta stop making sex jokes about your own {person1}.”\n“That was barely even aimed at you.”\n“Don’t care. You’re annoying. And {adj}”\n“God {mc_d}, if that stick goes any farther up your ass you're gonna start moaning.”\nWhile {mc_d} and {mc_a} bickered, {mc_b} had already started towards the {thing1}-fighter box.\n“Are you two coming?” {mc_c} trails behind {mc_b}.\n{mc_d} rolls {their_d} eyes at {mc_a} then falls into step next to {mc_c}.", 
            new List<TemplateSlot>
            {
              new TemplateSlot("mc_a", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("b", WordType.Person),
              new TemplateSlot("mc_b", WordType.Name, parentId:"b"),
              new TemplateSlot("they_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.Subject),
              new TemplateSlot("them_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.Object),
              new TemplateSlot("their_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("c", WordType.Person),
              new TemplateSlot("mc_c", WordType.Name, parentId:"c"),
              new TemplateSlot("they_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.Subject),
              new TemplateSlot("them_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.Object),
              new TemplateSlot("their_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("d", WordType.Person),
              new TemplateSlot("mc_d", WordType.Name, parentId:"d"),
              new TemplateSlot("they_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.Subject),
              new TemplateSlot("them_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.Object),
              new TemplateSlot("their_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("place1", WordType.Thing),
              new TemplateSlot("phrase", WordType.Catchphrase),
              new TemplateSlot("thing1", WordType.Thing),
              new TemplateSlot("person1", WordType.Person),
              new TemplateSlot("adj",WordType.Adjective)
            }, "Comedy")
        });
        templates.Add("Adventure", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("The thin road stretches out in front of {mc}, it was dark enough now that the juncture of its eventual turn was obscured in shadow. {mc} and {their_a} friends called the road {adj1}-{thing1} road because of the way the {thing0} on either side seemed to crowd in around you as you traveled down it. The road was especially “{adj1}-{thing1}”-y at night. It was easy to imagine {adj3} {person1} and {adj2} {person2} crawling out of the {place1} after the sun set. It was too far out of town for there to be any lights. {mc} relied on the glow cast down by the moon to navigate to the {place2}. Though, even the moonlight seemed to have a personality of its own on {adj1}-{thing1} road. Spindly, arm like, shadows stretched across the gravel, swaying and grabbing at {them_a} as {they_a} walked along the path. {mc} glanced up at the {thing2} rocking in the autumn breeze. {they_a} should’ve worn a thicker jacket like {their_a} {person3} told {them_a} to.", 
            new List<TemplateSlot>
            {
              new TemplateSlot("mc", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("adj1", WordType.Adjective),
              new TemplateSlot("thing1", WordType.Thing),
              new TemplateSlot("place1", WordType.Place),
              new TemplateSlot("adj3", WordType.Adjective),
              new TemplateSlot("adj2", WordType.Adjective),
              new TemplateSlot("person1", WordType.Person, plural:true),
              new TemplateSlot("person2", WordType.Person, plural:true),
              new TemplateSlot("place2", WordType.Place),
              new TemplateSlot("thing2", WordType.Thing, plural:true),
              new TemplateSlot("person3", WordType.Person),
              new TemplateSlot("thing0", WordType.Thing, plural:true)
            }, "Adventure")
        });
        templates.Add("Slice-of-Life", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("With a sigh, {mc_a} tucked {their_a} {thing1} back into {their_a} pocket. The door then opened, and {they_a} {was} greeted by {an} {adj1} {person_b} {mc_b} ushering {them_a} in through the door. {mc_b}’s house was exactly as cookie-cutter suburban on the inside as it was on the outside. {mc_a} felt like {they_a} had walked into a tv show set. The family's shoes were tucked away neatly in the entry-way closet, instead of scattered haphazardly on the doormat like they would be in {their_b} own home. The scent of something homecooked— {thing2} and {thing3}— wafted in from the kitchen.\n“It’s so good to see you again, {mc_a}!” {mc_b} wrapped {their_b} arms around the {adj} {person_a} as {they_a} set down {their_a} bags, “{phrase}”", 
            new List<TemplateSlot>
            {
                new TemplateSlot("person_a", WordType.Person),
                new TemplateSlot("person_b", WordType.Person),
                new TemplateSlot("mc_b", WordType.Name, parentId:"character"),
                new TemplateSlot("they_b", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
                new TemplateSlot("them_b", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
                new TemplateSlot("their_b", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
                new TemplateSlot("a", WordType.Person),
                new TemplateSlot("mc_a", WordType.Name, parentId:"a"),
                new TemplateSlot("they_a", WordType.Pronoun, parentId:"a", conjugation:Conjugation.Subject),
                new TemplateSlot("them_a", WordType.Pronoun, parentId:"a", conjugation:Conjugation.Object),
                new TemplateSlot("their_a", WordType.Pronoun, parentId:"a", conjugation:Conjugation.PossessiveAdj),
                new TemplateSlot("thing1", WordType.Thing),
                new TemplateSlot("adj1", WordType.Adjective),
                new TemplateSlot("thing2", WordType.Thing),
                new TemplateSlot("thing3", WordType.Thing),
                new TemplateSlot("adj", WordType.Adjective),
                new TemplateSlot("phrase", WordType.Catchphrase),
                new TemplateSlot("was", WordType.Verb, parentId:"a"),
                new TemplateSlot("an", WordType.IndefiniteArticle, parentId:"adj1")

            }, "Slice-of-Life"),
            new BookBlurbTemplate("Eventually, finally, the group settled on rewatching their usual movie night flick: “{person1}s and {thing2}s”. They pass the {thing3}s around and dig into the pizza: Hawaiian because {mc_a} picked it. {mc_b} grumbles as {they_b} picks off the pineapple from {their_b} slices, silently putting them on {mc_c}’s plate. {mc_c} piles them onto {their_c} pizza.\nAbout ten minutes into the movie, {mc_a} looks over to the couch, “So, what brings you to our {place1}, {mc_d}?”\n{mc_d} tenses, “Ah, my parents thought it’d be good for me. Let me find some new people to bother. Y’know… ‘{phrase}’ or whatever” {they_d} finishes with a noncommittal shrug.\n“Well, you’re doing a great job of that,” {mc_b}, {adj1}, mumbles.\n{mc_c} side eyes {mc_b}, “You’re being real {adj2} for a replaceable friend group member, {mc_b}.”\n“The fuck do you mean ‘replacable’?”\n“Just saying, goofy over here’s more likeable than {adj3}”\n{mc_c} snorts and tries to hide {their_c} grin behind {their_c} hand when {mc_b} shoots {them_c} a betrayed look.", 
            new List<TemplateSlot>
            {
              new TemplateSlot("mc_a", WordType.Name, parentId:"character"),
              new TemplateSlot("they_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
              new TemplateSlot("them_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Object),
              new TemplateSlot("their_a", WordType.Pronoun, parentId:"character", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("b", WordType.Person),
              new TemplateSlot("mc_b", WordType.Name, parentId:"b"),
              new TemplateSlot("they_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.Subject),
              new TemplateSlot("them_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.Object),
              new TemplateSlot("their_b", WordType.Pronoun, parentId:"b", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("c", WordType.Person),
              new TemplateSlot("mc_c", WordType.Name, parentId:"c"),
              new TemplateSlot("they_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.Subject),
              new TemplateSlot("them_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.Object),
              new TemplateSlot("their_c", WordType.Pronoun, parentId:"c", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("d", WordType.Person),
              new TemplateSlot("mc_d", WordType.Name, parentId:"d"),
              new TemplateSlot("they_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.Subject),
              new TemplateSlot("them_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.Object),
              new TemplateSlot("their_d", WordType.Pronoun, parentId:"d", conjugation:Conjugation.PossessiveAdj),
              new TemplateSlot("person1", WordType.Person),
              new TemplateSlot("thing2", WordType.Thing),
              new TemplateSlot("thing3", WordType.Thing),
              new TemplateSlot("place1", WordType.Place),
              new TemplateSlot("phrase", WordType.Catchphrase),
              new TemplateSlot("adj1", WordType.Adjective),
              new TemplateSlot("adj2", WordType.Adjective),
              new TemplateSlot("adj3", WordType.Adjective)
            }, "Slice-of-Life")
        });
        templates.Add("Test", new List<BookBlurbTemplate>
        {
            new BookBlurbTemplate("{they} {is} so annoying. {they} {has} so many {thing}, and {does} nothing with them!", 
            new List<TemplateSlot>
            {
                new TemplateSlot("they", WordType.Pronoun, parentId:"character", conjugation:Conjugation.Subject),
                new TemplateSlot("is", WordType.Verb, parentId:"character"),
                new TemplateSlot("thing", WordType.Thing, plural:true),
                new TemplateSlot("has", WordType.Verb, parentId:"character"),
                new TemplateSlot("does", WordType.Verb, parentId:"character")
            }
            ,"Test"
            ),
            
        });
        
        
        names.Add(Gender.feminine, new List<string> { "Mary", "Lottie", "Amelia", "Pauline", "Molly", "Harriet", "Leah", "Astrid", "Loren", "Avery", "Stardust", "Kitty", "Caitlyn", "Meghan", "Maggie", "Maryam", "Renaissa", "Charlotte", "Ingrid", "Mary", "Faith", "Precious", "Sophia", "Vivian", "Sofia", "Anita", "Gloria"});
        names.Add(Gender.masculine, new List<string> { "Bob", "Reggie", "Reginald", "Barty", "John", "Maverick", "Nicholas", "Xavier", "Alex", "Steel Lightning", "Faiaz", "Brian", "William", "Mitch", "Lucas", "Ryan", "Todd", "Richard", "Bartemius", "Reggie", "James", "Temi", "Oscar", "Nico", "Zafir", "Rafael", "Lee"});
        names.Add(Gender.nonbinary, new List<string> { "Alex", "Loren", "Avery", "Stardust", "Steel Lightning" });
        Debug.Log("I have populated all the data i Hope");
    }
// end of BookBLurnSupplier Class
}



[System.Serializable]
public class SettingWordSet
{
    public List<string> people = new();
    public List<string> places = new();
    public List<string> things = new();
}

[System.Serializable]
public class CharacterWordSet
{
    public List<string> adjectives = new();
    public List<string> catchphrases = new();
}
public enum Gender
{
    feminine,
    masculine,
    nonbinary
}