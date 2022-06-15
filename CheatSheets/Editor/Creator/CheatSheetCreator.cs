using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Creator {
    internal sealed class CheatSheetCreator : EditorWindow {

        const string UXML_RESOURCE_PATH = "CheatSheets/Core/Creator/CheatSheetCreator";

        ScrollView scrollView;

        VisualElement container;

        Button addBtn;
        Button removeBtn;

        Button createBtn;

        [SerializeField]
        List<CheatSheet> cheatSheets = new List<CheatSheet>();

        public static void CreateAndShow () {
            CheatSheetCreator window = EditorWindow.CreateInstance<CheatSheetCreator>();
            window.titleContent      = new GUIContent("Cheat Sheet Creator");
            window.Show(); // This one can be used for debug
            //window.ShowAuxWindow(); // This one will close when the "Open folder" dialog is closed.
        }

        void OnEnable () {
            this.rootVisualElement.Add(Resources.Load<VisualTreeAsset>(UXML_RESOURCE_PATH).CloneTree());
            this.scrollView = this.rootVisualElement.Q<ScrollView>("ScrollView");
            this.container  = this.scrollView.Q<VisualElement>("Container");
            this.addBtn     = this.rootVisualElement.Q<Button>("AddBtn");
            this.removeBtn  = this.rootVisualElement.Q<Button>("RemoveBtn");
            this.createBtn  = this.rootVisualElement.Q<Button>("CreateBtn");

            if ( this.cheatSheets.Count == 0 ) {
                this.cheatSheets.Add(new CheatSheet());
            }

            this.addBtn.clicked    += this.AddCheatSheet;
            this.removeBtn.clicked += this.RemoveLastCheatSheet;
            this.createBtn.clicked += this.OnClickCreateBtn;

            this.RegenerateVisualTree();
        }

        void RegenerateVisualTree () {
            this.container.Clear();
            foreach ( CheatSheet cheatSheet in this.cheatSheets ) {
                cheatSheet.RegenerateVisualElement();
                this.container.Add(cheatSheet.root);
            }
        }


        void OnGUI () {
            this.scrollView.style.height = this.position.height;
        }

        void AddCheatSheet() {
            CheatSheet cheatSheet = (CheatSheet)this.cheatSheets[this.cheatSheets.Count - 1].Clone();
            this.cheatSheets.Add(cheatSheet);
            this.RegenerateVisualTree();
        }

        void RemoveLastCheatSheet () {
            this.cheatSheets.RemoveAt(this.cheatSheets.Count - 1);
            this.RegenerateVisualTree();
        }

        void OnClickCreateBtn () {
            bool wasSuccessfull = ResourcesBuilder.Create(this.cheatSheets);
            if ( wasSuccessfull ) {
                this.Close();
            }
        }
    }
}