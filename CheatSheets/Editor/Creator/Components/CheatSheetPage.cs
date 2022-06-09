using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Creator {
    [Serializable]
    internal class CheatSheetPage : VisualItem, ICloneable {

        const string UXML_RESOURCE_PATH = "CheatSheets/Core/Creator/CheatSheetCreatorPageItem";

        static VisualTreeAsset asset;

        Foldout foldout;

        /// <summary>
        /// <para>Will use the two parameters constructor of <see cref="CustomCheatSheetAttribute"/>.</para>
        /// <para>The first represents its tree path with the page number if wanted. The second one represents the resource path.
        /// The third are extra options such as showing the page number and other fields.</para>
        /// </summary>
        const string ATTRIBUTE_LINE = "[CustomCheatSheet({0}, {1}{2})]";

        CheatSheet parent;

        public Tuple<bool, Toggle>        useNumber      = new Tuple<bool, Toggle>(false);
        public Tuple<int, BaseField<int>> number         = new Tuple<int, BaseField<int>>(1);
        public Tuple<bool, Toggle>        showPageNumber = new Tuple<bool, Toggle>(true);
        public Tuple<string, TextField>   keywords       = new Tuple<string, TextField>();

        public CheatSheetPage ( CheatSheetPage original ) {
            this.parent = original.parent;
            this.useNumber.SetValue(original.useNumber.GetValue());
            this.number.SetValue(original.number.GetValue() + 1);
            this.showPageNumber.SetValue(original.showPageNumber.GetValue());
            this.keywords.SetValue(original.keywords.GetValue());
        }

        public CheatSheetPage ( CheatSheet parent ) {
            this.parent = parent;
        }

        public override void RegenerateVisualElement () {
            if ( CheatSheetPage.asset == null ) {
                CheatSheetPage.asset = Resources.Load<VisualTreeAsset>(UXML_RESOURCE_PATH);
            }

            if ( string.IsNullOrEmpty(this.name.GetValue()) ) {
                this.name.SetValue("A Page");
            }

            this.root    = CheatSheetPage.asset.CloneTree();
            this.foldout = this.root.Q<Foldout>("Foldout");

            this.name.SetVisualElement(this.root.Q<TextField>("PageName"));
            this.useNumber.SetVisualElement(this.root.Q<Toggle>("UseNumber"));
            this.number.SetVisualElement(this.root.Q<BaseField<int>>("PageNumber"));
            this.showPageNumber.SetVisualElement(this.root.Q<Toggle>("ShowPageNumber"));
            this.keywords.SetVisualElement(this.root.Q<TextField>("Keywords"));

            this.name.GetVisualElement().RegisterCallback<KeyUpEvent>(e => this.OnNameChanged());
            this.useNumber.GetVisualElement().RegisterValueChangedCallback(e => this.UpdateEnableState());

            this.foldout.RegisterValueChangedCallback(e => this.UpdateSerializedFoldout());
            this.foldout.value = this.serializedFoldout;

            this.parent.pageListContainer.Add(this.root);

            this.UpdateEnableState();
            base.RegenerateVisualElement();
        }

        /*public override void ApplyValuesToVisualElements () {
            
        }*/

        /// <summary>
        /// <para>Will create a string representing this <see cref="CheatSheetPage"/> as an <see cref="CustomCheatSheetAttribute"/>.</para>
        /// </summary>
        /// <returns></returns>
        public string GetAttributeLine () {
            #region "Tree Path"

            // Sanitize treePath
            while ( this.parent.treePath.GetValue().EndsWith("/") ) {
                string oldTreePath = this.parent.treePath.GetValue();
                this.parent.treePath.SetValue(oldTreePath.Substring(0, oldTreePath.Length - 1).Trim());
            }

            string treePath = parent.treePath.GetValue().Trim() + "/"
                           + (this.parent.useNameAsSection.GetValue() ?  this.parent.name.GetValue() + "/" : "") 
                           + (parent.useNameInPageName.GetValue() ? parent.name.GetValue() : "");
            if ( this.useNumber.GetValue() ) {
                treePath += "#" + this.number.GetValue() + " " + this.name.GetValue();
            }
            treePath = "\"" + treePath + "\"";
            #endregion

            #region "Resource Path"
            string resourcePath = "";
            string[] splitedPath = parent.resourcePath.GetValue().Trim().Split(Path.DirectorySeparatorChar);
            bool isInResourceFolder = false;
            for ( int ind = 0; ind < splitedPath.Length; ind++ ) {
                string s = splitedPath[ind];
                if (isInResourceFolder) {
                    resourcePath += s + "/";
                } else {
                    isInResourceFolder = s.Equals("Resources");
                }
            }
            resourcePath += parent.GetCleanName();
            resourcePath = "\"" + resourcePath + "/" + this.GetCleanName(false) + "\"";
            #endregion

            #region "Extra fields + Show Page Number"
            string extraFields = "";
            // Show Page Number
            if ( !this.showPageNumber.GetValue() ) {
                extraFields = ", showPageNumber=false";
            }

            // Keywords
            string formatKeywords = ", keywords = new string[]{{{0}}}";
            string keywordsValue = this.keywords.GetValue();
            if ( !string.IsNullOrEmpty(keywordsValue) ) {
                string s = "";
                string[] keywords = keywordsValue.Split(',');
                for (int ind = 0; ind < keywords.Length; ind++) {
                    s += "\"" + keywords[ind].Trim() + "\"" + (ind == keywords.Length - 1 ? "" : ", ");
                }

                extraFields += string.Format(formatKeywords, s);
            }

            // Page name
            if ( !this.useNumber.GetValue() && !string.IsNullOrEmpty(this.name.GetValue()) ) {
                extraFields += string.Format(", pageName=\"{0}\"", this.name.GetValue());
            }

            #endregion

            return string.Format(ATTRIBUTE_LINE, treePath, resourcePath, extraFields);
        }

        public override void UpdateFoldoutText () {
            string name = this.name.GetVisualElement().text;
            this.foldout.text = string.IsNullOrEmpty(name) ? " " : name;
        }

        public override void UpdateSerializedFoldout () {
            this.serializedFoldout = this.foldout.value;
        }

        void UpdateEnableState () {
            bool state = this.useNumber.GetValue();
            this.showPageNumber.GetVisualElement().SetEnabled(state);
            this.number.GetVisualElement().SetEnabled(state);
        }

        public object Clone () {
            return new CheatSheetPage(this);
        }
    }
}