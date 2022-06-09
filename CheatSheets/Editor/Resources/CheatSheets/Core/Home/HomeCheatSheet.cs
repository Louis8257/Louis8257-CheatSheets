// ------------------------------------------------------------------------------------------------------------------------
// CheatSheets is made possible by Louis-Pierre Aubert | Louis8257 | https://www.louis8257.com/
//
// DO NOT DELETE THIS FILE!
// ------------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets {
    [CustomCheatSheet("Cheat Sheets Documentation/#1 Introduction",                "CheatSheets/Core/Home/Home_1",
        keywords = new string[]{"Welcome", "Pages", "Buttons"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#2 How does it works",           "CheatSheets/Core/Home/Home_2",
        keywords = new string[] {"Pages", "Life Cycle", "Tree", "Buttons", "Custom Attributes", "Serialization", "Keywords", "Styles", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#3 Custom Attribute",            "CheatSheets/Core/Home/Home_3",
        keywords = new string[] { "Cheat Sheet", "Pages", "Tree", "Sections", "Serialization", "Keywords", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#4 Sections",                    "CheatSheets/Core/Home/Home_4",
        keywords = new string[] { "Tree", "Colors", "Search Engine", "Pages", "Items", "Ergonomy" })]
    [CustomCheatSheet("Cheat Sheets Documentation/#5 Page Numbers",                "CheatSheets/Core/Home/Home_5",
        keywords = new string[] { "Ergonomy", "Pages", "Items", "Buttons" })]
    [CustomCheatSheet("Cheat Sheets Documentation/#6 Mindset",                     "CheatSheets/Core/Home/Home_6",
        keywords = new string[] { "Share", "Open source", "Documentation" })]
    [CustomCheatSheet("Cheat Sheets Documentation/#7 Create A Cheat Sheet",        "CheatSheets/Core/Home/Home_7",
        keywords = new string[] {"Pages", "Cheat Sheet", "Custom Attributes", "Scripts", "Documentation", "Styles", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#8 Using UI Builder",            "CheatSheets/Core/Home/Home_8",
        keywords = new string[] {"Window", "User Friendly", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#9 Create Several Cheat Sheets", "CheatSheets/Core/Home/Home_9",
        keywords = new string[] {"Automation", "Pages", "Custom Attributes", "Scripts"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#10 Share your cheat sheets",    "CheatSheets/Core/Home/Home_10",
        keywords = new string[] {"Open Source", "Share", "Mindset", "Documentation", "GitHub", "Asset Store"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#11 Credits & Contacts",         "CheatSheets/Core/Home/Home_11",
        keywords = new string[] {"Louis8257", "Louis-Pierre Aubert", "louis.8257@hotmail.com"})]
    public class HomeCheatSheet : CheatSheetElement {

        Button testBtn;

        public override void OnEnable () {
            this.testBtn = this.rootVisualElement.Q<Button>("TestBtn");
            if ( this.testBtn != null ) {
                this.testBtn.clicked += () => { this.window.selectedPage = this.window.stack.loadedAttributes[0]; };
            }
        }

    }
}