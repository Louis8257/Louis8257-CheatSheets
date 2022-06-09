using CheatSheets.Stack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets {
    public class CheatSheetWindow : EditorWindow {

        const string UXML_RESOURCE_PATH = "CheatSheets/Core/CheatSheetWindow";

        [SerializeField]
        internal CheatSheetStack stack = new CheatSheetStack();

        [SerializeField]
        internal CustomCheatSheetAttribute _selectedPage;

        public CustomCheatSheetAttribute selectedPage {
            get {
                return this._selectedPage;
            }
            set {
                if (this.selectedPage != null && this.selectedPage.element != null && this.selectedPage.element._rootVisualElement != null) {
                    this.selectedPage.element.Destroy();
                }

                this._selectedPage = value;

                if ( this.scrollView != null ) {
                    this.RegenerateCheatSheetVisualElement();    // When window fully loaded
                }
            }
        }

        internal CheatSheetSelectorWindow selectorWindow;

        ScrollView scrollView;
        Label      pageNumberLabel;
        Button     cheatSheetSelectorBtn;
        Button     previousCheatSheetBtn;
        Button     nextCheatSheetBtn;

        [MenuItem("Window/Cheat Sheets", false, 100)]
        static void ShowUI () {
            CheatSheetWindow window = EditorWindow.GetWindow<CheatSheetWindow>();
            window.Show();
        }

        public void OnEnable () {
            this.stack.Build();

            this.titleContent = new GUIContent("Cheat Sheets");

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>(UXML_RESOURCE_PATH);

            if ( tree == null ) {
                Debug.LogError("[Cheat Sheets] Can't create window! The resource CheatSheetWindowTree is missing!");
                this.Close();
            }

            if ( this.selectedPage != null ) {
                CustomCheatSheetAttribute foundAttribute = this.stack.loadedAttributes.Find(o => o.cheatSheetName == this.selectedPage.cheatSheetName);
                if ( foundAttribute != null ) {
                    this.selectedPage = foundAttribute;
                }
            }

            this.rootVisualElement.Add(tree.CloneTree());

            // Get VisualElements from visual tree
            this.scrollView            = rootVisualElement.Q<ScrollView>("ScrollView");
            this.cheatSheetSelectorBtn = rootVisualElement.Q<Button>("CheatSheetSelectorBtn");
            this.previousCheatSheetBtn = rootVisualElement.Q<Button>("PreviousCheatSheetBtn");
            this.nextCheatSheetBtn     = rootVisualElement.Q<Button>("NextCheatSheetBtn");
            this.pageNumberLabel       = rootVisualElement.Q<Label>("PageNumberLabel");

            this.cheatSheetSelectorBtn.clickable.clicked += this.OnCheatSheetSelectorClick;
            this.previousCheatSheetBtn.clickable.clicked += this.OnPreviousPageClick;
            this.nextCheatSheetBtn.clickable.clicked     += this.OnNextPageClick;

            this.RegenerateCheatSheetVisualElement();
        }

        public void OnDisable () {
            if ( this.selectedPage != null && this.selectedPage.element._rootVisualElement != null ) {
                this.selectedPage.element.Destroy();
            }
        }

        void OnGUI () {
            this.cheatSheetSelectorBtn.text = this.selectedPage != null ? this.selectedPage.cheatSheetName : "Select a cheat sheet...";
            this.previousCheatSheetBtn.SetEnabled(this.selectedPage != null && this.selectedPage.HasPreviousPage());
            this.nextCheatSheetBtn.SetEnabled(this.selectedPage != null && this.selectedPage.HasNextPage());
            this.pageNumberLabel.text = this.selectedPage != null && this.selectedPage.HasPageNumber() ?
                                        string.Format("Page {0}/{1}", this.selectedPage.pageNumber, this.selectedPage.GetLastPageNumber())
                                      : "Page -/-";

            if ( this.selectedPage != null && this.selectedPage.element._rootVisualElement != null ) {
                this.selectedPage.element.OnGUI();
            }
        }

        void OnPreviousPageClick () {
            CustomCheatSheetAttribute page = this.selectedPage.GetPreviousPage();
            if ( page == null ) {
                return;
            }

            this.selectedPage = page;
        }

        void OnNextPageClick () {
            CustomCheatSheetAttribute page = this.selectedPage.GetNextPage();
            if ( page == null ) {
                return;
            }

            this.selectedPage = page;
        }

#pragma warning disable CS0168
        internal void OnNewCheatSheetSelection () {
            scrollView.Clear();

            TemplateContainer clonedContainer = null;
            try {
                clonedContainer = (TemplateContainer)this.selectedPage.element.CloneTree(this);
            } catch ( MissingReferenceException e ) {
                Debug.LogWarning("[Cheat Sheets] Something went wrong while cloning the tree from " /*+*/ );
                this.stack.Build();
                clonedContainer = this.selectedPage.element.visualTreeAsset.CloneTree();
            }

            scrollView.Add(clonedContainer);
            Repaint();
        }

        void OnCheatSheetSelectorClick () {
            this.selectorWindow        = EditorWindow.GetWindow<CheatSheetSelectorWindow>();
            this.selectorWindow.parent = this;
            Vector2 worldPos           = this.position.position + Event.current.mousePosition;
            this.selectorWindow.Show();
            this.selectorWindow.RegenerateContainerTree(this.stack.tree);
        }

        void RegenerateCheatSheetVisualElement () {
            scrollView.Clear();

            if ( this.selectedPage == null ) {
                return;
            }

            TemplateContainer clonedContainer = null;
            try {
                clonedContainer = (TemplateContainer)this.selectedPage.element.CloneTree(this);
                scrollView.Add(clonedContainer);
            }
            catch ( MissingReferenceException e ) {
                Debug.LogWarning("[Cheat Sheets] The selected cheat sheet doesn't exist anymore!" );
            }
        }
#pragma warning restore CS0168
    }
}