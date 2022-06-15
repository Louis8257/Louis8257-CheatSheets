using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Creator {
    [Serializable]
    internal class CheatSheet : VisualItem, ICloneable {

        const string UXML_RESOURCE_PATH = "CheatSheets/Core/Creator/CheatSheetCreatorItem";

        #region "Error Messages"
        const string ERROR_NAME_INVALID        = "Your cheat sheet must have a name!";
        const string ERROR_TREE_PATH_INVALID   = "This cheat sheet has several separators with different characters for the tree path! Use '/' only as a separator only.";
        const string ERROR_PAGE_NUMBER_INVALID = "The page number must be superior than 0.";
        const string ERROR_PAGE_NAME_INVALID   = "The page name is empty!";
        #endregion

        static VisualTreeAsset asset;

        Foldout foldout;

        Button  openFolderButton;
        Button  addBtn;
        Button  removeBtn;
       
        public VisualElement pageListContainer;
        
        public Tuple<string, TextField>   treePath          = new Tuple<string, TextField>("Section 1/Section A");
        public Tuple<bool, Toggle>        useNameAsSection  = new Tuple<bool, Toggle>(true);
        public Tuple<bool, Toggle>        useNameInPageName = new Tuple<bool, Toggle>(false);
        public Tuple<string, TextField>   resourcePath      = new Tuple<string, TextField>();

        public List<CheatSheetPage> pages = new List<CheatSheetPage>();

        public CheatSheet() { }

        public CheatSheet ( CheatSheet original ) {
            this.useNameAsSection.SetValue(original.useNameAsSection.GetValue());
            this.useNameInPageName.SetValue(original.useNameInPageName.GetValue());
            this.resourcePath.SetValue(original.resourcePath.GetValue());
        }

        public override void RegenerateVisualElement () {
            if ( CheatSheet.asset == null ) {
                CheatSheet.asset = Resources.Load<VisualTreeAsset>(UXML_RESOURCE_PATH);
            }

            if ( string.IsNullOrEmpty(this.name.GetValue()) ) {
                this.name.SetValue("My Cheat Sheet");
            }

            this.root              = CheatSheet.asset.CloneTree();
            this.foldout           = this.root.Q<Foldout>("Foldout");
            this.pageListContainer = this.root.Q<VisualElement>("PageListContainer");

            this.name.SetVisualElement(this.root.Q<TextField>("Name"));
            this.treePath.SetVisualElement(this.root.Q<TextField>("TreePath"));
            this.useNameAsSection.SetVisualElement(this.root.Q<Toggle>("UseNameAsSection"));
            this.useNameInPageName.SetVisualElement(this.root.Q<Toggle>("UseNameInPageName"));
            this.resourcePath.SetVisualElement(this.root.Q<TextField>("ResourcePath"));

            this.addBtn             = this.root.Q<Button>("AddBtn");
            this.addBtn.clicked    += this.AddPage;
            this.removeBtn          = this.root.Q<Button>("RemoveBtn");
            this.removeBtn.clicked += this.RemoveLastPage;

            this.openFolderButton = this.root.Q<Button>("OpenFolderButton");
            this.openFolderButton.clicked += OnOpenFolderButtonClick;

            this.foldout.RegisterValueChangedCallback(e => this.UpdateSerializedFoldout());
            this.foldout.value = this.serializedFoldout;

            this.name.GetVisualElement().RegisterCallback<KeyUpEvent>(e => this.OnNameChanged());

            if ( this.pages.Count == 0 ) {
                this.pages.Add(new CheatSheetPage(this));
            }

            foreach ( CheatSheetPage page in this.pages ) {
                page.RegenerateVisualElement();
            }

            base.RegenerateVisualElement();
        }

        void OnOpenFolderButtonClick () {
            string path         = EditorUtility.OpenFolderPanel("Choose a resource folder", null, null);
            string projectPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);

            // Replace the current separator with the current OS one
            path = path.Replace('/', Path.DirectorySeparatorChar);

            if ( string.IsNullOrEmpty(path) ) {
                return;
            }

            if ( !path.Contains(projectPath) ) {
                Debug.LogError("[Cheat Sheets] The choosen path is not valid! It must be in the current project.");
                return;
            } else if ( !path.Contains("Resources") ) {
                Debug.LogError("[Cheat Sheets] The choosen path is not valid! It must be in a \"Resources\" folder.");
                return;
            }

            this.resourcePath.SetValue(path);
        }

        public override void UpdateFoldoutText () {
            string name = this.name.GetVisualElement().text;
            this.foldout.text = string.IsNullOrEmpty(name) ? " " : name;
        }

        public override void UpdateSerializedFoldout () {
            this.serializedFoldout = this.foldout.value;
        }

        void AddPage () {
            CheatSheetPage page = (CheatSheetPage)this.pages[this.pages.Count - 1].Clone();
            this.pages.Add(page);
            page.RegenerateVisualElement();
        }

        void RemoveLastPage () {
            this.pages.RemoveAt(this.pages.Count - 1);
            this.root.Clear();
            this.RegenerateVisualElement();
        }

        #region "Error Checks"
        /// <summary>
        /// <para>Will return strings according to the number of errors. null means no error.</para>
        /// </summary>
        public string[] CheckErrors () {
            List<string> errors = new List<string>();

            if ( !IsNameValid() ) {
                errors.Add("\t-" + ERROR_NAME_INVALID + "\n");
            }

            if ( !IsTreePathValid() ) {
                errors.Add("\t-" + ERROR_TREE_PATH_INVALID + "\n");
            }

            return errors.ToArray();
        }

        public bool IsNameValid () {
            return !string.IsNullOrEmpty(this.name.GetValue());
        }

        public bool IsTreePathValid () {
            string value = this.treePath.GetValue();
            return !string.IsNullOrEmpty(value);
        }

        public bool IsResourcePathValid () {
            string value = this.resourcePath.GetValue();
            return value.Contains(Application.dataPath) && value.Contains("Resources");
        }

        public bool IsKeywordsValid () {
            return true;
        }

        public object Clone () {
            return new CheatSheet(this);
        }
        #endregion

    }
}