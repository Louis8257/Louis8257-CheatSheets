using CheatSheets.Creator;
using CheatSheets.Stack;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets {
    internal sealed class CheatSheetSelectorWindow : EditorWindow {

        public const string UXML_RESOURCE_PATH_ROOT = "CheatSheets/Core/Selector/";

        const string UXML_RESOURCE_PATH = UXML_RESOURCE_PATH_ROOT + "CheatSheetSelectorWindow";

        internal CheatSheetWindow parent;

        ScrollView scrollView;

        TextField searchBar;

        Label searchBarHint;

        [SerializeField]
        VisualTreeAsset sectionTemplate;

        [SerializeField]
        VisualTreeAsset itemTemplate;

        Button createBtn;

        public void OnEnable () {
            this.titleContent = new GUIContent("Cheat Sheet Selector");

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>(UXML_RESOURCE_PATH);

            if ( tree == null ) {
                Debug.LogError("[Cheat Sheets] Can't create window! The resource CheatSheetWindowTree is missing!");
                this.Close();
            }
            this.rootVisualElement.Add(tree.CloneTree());
            this.scrollView    = this.rootVisualElement.Q<ScrollView>("ScrollView");
            this.searchBar     = this.rootVisualElement.Q<TextField>("SearchBar");
            this.searchBarHint = this.searchBar.Q<Label>("SearchBarHint");
            this.createBtn     = this.rootVisualElement.Q<Button>("CreateBtn");

            this.searchBar.RegisterCallback<KeyUpEvent>(e => OnSearchBarInput());
            this.createBtn.clickable.clicked += CheatSheetCreator.CreateAndShow;
        }

        public void OnGUI () {
            this.searchBarHint.text = string.IsNullOrEmpty(this.searchBar.text) ? "Search keywords..." : "";
            this.scrollView.style.height = this.position.height;
        }

        void OnSearchBarInput () {
            this.SearchAndShowTree(this.searchBar.text.Split(' '));
        }

        internal void RegenerateContainerTree ( CheatSheetTree tree ) {
            float hueIncrementation = 1f / (tree.subSections.Count + tree.attributes.Count);

            this.scrollView.Clear();
            this.scrollView.Add(tree.GetVisualElement(this.scrollView, null, 0.0f, hueIncrementation));
            tree.UnfoldSections(this.parent.selectedPage);
            tree.HighlightItem(this.parent.selectedPage);
        }

        internal void SearchAndShowTree ( string[] keywords ) {
            List<CustomCheatSheetAttribute> searchResult = new List<CustomCheatSheetAttribute>();
            foreach ( CustomCheatSheetAttribute attribute in this.parent.stack.loadedAttributes ) {
                bool isSearchedAttribute = false;
                foreach ( string keyword in keywords ) {
                    isSearchedAttribute = attribute.ContainsKeyword(keyword);
                    if ( !isSearchedAttribute ) {
                        break;
                    }
                }
                if ( isSearchedAttribute ) {
                    searchResult.Add(attribute);
                }
            }

            CheatSheetTree tree = new CheatSheetTree(searchResult);
            RegenerateContainerTree(tree);
        }
    }
}
