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
    [CustomCheatSheet("Cheat Sheets Documentation/#3 Script & Custom Attribute", "CheatSheets/Core/Home/Home_3",
        keywords = new string[] { "Cheat Sheet", "Pages", "Tree", "Sections", "Serialization", "Keywords", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#4 Mindset",                     "CheatSheets/Core/Home/Home_4",
        keywords = new string[] { "Share", "Open source", "Documentation" })]
    [CustomCheatSheet("Cheat Sheets Documentation/#5 Create A Cheat Sheet",        "CheatSheets/Core/Home/Home_5",
        keywords = new string[] {"Pages", "Cheat Sheet", "Custom Attributes", "Scripts", "Documentation", "Styles", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#6 Using UI Builder",            "CheatSheets/Core/Home/Home_6",
        keywords = new string[] {"Window", "User Friendly", "UXML"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#7 Share your cheat sheets",    "CheatSheets/Core/Home/Home_7",
        keywords = new string[] {"Open Source", "Share", "Mindset", "Documentation", "GitHub", "Asset Store"})]
    [CustomCheatSheet("Cheat Sheets Documentation/#8 Credits & Contacts",         "CheatSheets/Core/Home/Home_8",
        keywords = new string[] {"Louis8257", "Louis-Pierre Aubert", "louis.8257@hotmail.com"})]
    public class HomeCheatSheet : CheatSheetElement {

        Button testBtn;

        enum Page {
            Introduction        = 1,
            HowDoesItWorks      = 2,
            ScriptAndAttributes = 3,
            Mindset             = 4,
            CreateCheatSheet    = 5,
            UIBuilder           = 6,
            Share               = 7,
            Credits             = 8
        }

        public override void OnEnable () {
            this.testBtn = this.rootVisualElement.Q<Button>("TestBtn");
            if ( this.testBtn != null ) {
                this.testBtn.clicked += () => { this.window.selectedPage = this.window.stack.loadedAttributes[0]; };
            }

            Page index = (Page)this.attribute.pageNumber;
            switch ( index ) {
                case Page.CreateCheatSheet: OnEnablePage5(); break;
                case Page.UIBuilder:        OnEnablePage6(); break;
                case Page.Share:            OnEnablePage7(); break;
            }
        }

        #region "Page 5"
        void OnEnablePage5 () {
            Button goToPage3Btn = this.rootVisualElement.Q<Button>("GoToPage3Btn");

            goToPage3Btn.clicked += GoToPage3Btn_clicked;
        }

        void GoToPage3Btn_clicked () {
            this.window.selectedPage = this.window.stack.loadedAttributes.Find(o => o.pageNumber == 3);
        }
        #endregion

        #region "Page 6"
        void OnEnablePage6 () {
            Button uiBuilderInstallDocBtn = this.rootVisualElement.Q<Button>("UiBuilderInstallDocBtn");

            uiBuilderInstallDocBtn.clicked += UiBuilderInstallDocBtn_clicked;
        }

        private void UiBuilderInstallDocBtn_clicked () {
            Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.ui.builder@1.0/manual/index.html");
        }
        #endregion

        #region "Page 7"
        void OnEnablePage7 () {
            Button gitHubLinkBtn = this.rootVisualElement.Q<Button>("GitHubLinkBtn");
            Button ccZeroLinkBtn = this.rootVisualElement.Q<Button>("CcZeroLinkBtn");

            gitHubLinkBtn.clicked += GitHubLinkBtn_clicked;
            ccZeroLinkBtn.clicked += CcZeroLinkBtn_clicked;
        }

        private void GitHubLinkBtn_clicked () {
            //Application.OpenURL();
        }

        private void CcZeroLinkBtn_clicked () {
            Application.OpenURL("https://creativecommons.org/publicdomain/zero/1.0/");
        }
        #endregion
    }
}