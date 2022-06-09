//#define DEBUG_MODE

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Stack {
    internal class CheatSheetTree : Section {

        public const string ROOT_TREE_NAME = "Root";

        private CheatSheetTree () {
            this.name = ROOT_TREE_NAME;
        }

        public CheatSheetTree ( List<CustomCheatSheetAttribute> attributes ) : base() {
            this.Build(attributes);
            this.Sort();
        }

        public void Build ( List<CustomCheatSheetAttribute> cheatSheets ) {
            ScanSections(this, cheatSheets);
#if DEBUG_MODE
            Debug.Log(string.Format("[Cheat Sheets] Regenerated {0} sections !", this.GetNumberOfSubSections(true)));
#endif
        }

        internal static void ScanSections ( CheatSheetTree root, List<CustomCheatSheetAttribute> cheatSheets ) {
            foreach ( CustomCheatSheetAttribute attribute in cheatSheets ) {
                // Get Sections
                Section parent = root;
                foreach ( string sectionName in attribute.sections ) {
                    Section subSection = parent.GetSection(sectionName);
                    if ( subSection == null ) {
                        subSection = new Section(sectionName);
                        parent.subSections.Add(subSection);
#if DEBUG_MODE
                        Debug.Log("[Cheat Sheets] Created sub section " + subSection.name);
#endif
                    }
                    parent = subSection;
                }

                // Store Attribute
                parent.attributes.Add(attribute);
#if DEBUG_MODE
                Debug.Log("[Cheat Sheets] Created item " + attribute.cheatSheetName);
#endif
            }
        }

        /// <summary>
        /// <para>Will unfold every section specified in <paramref name="item"/>.</para>
        /// <para><i>Can check if <paramref name="item"/> is null.</i></para>
        /// </summary>
        /// <param name="item"></param>
        public void UnfoldSections ( CustomCheatSheetAttribute item ) {
            if ( item == null ) {
                return;
            }

            VisualElement selector = this.root;
            foreach ( string sectionName in item.sections ) {
                Foldout foldout = selector.Q<Foldout>(sectionName);
                if ( foldout == null ) {
                    break;                  // Can happen when searching after selecting an item
                }

                foldout.value = true;
                selector = foldout.contentContainer;
            }
        }

        /// <summary>
        /// <para>Will highlight the choosen <paramref name="item"/>.</para>
        /// <para><i>Can check if <paramref name="item"/> is null.</i></para>
        /// </summary>
        /// <param name="item"></param>
        public void HighlightItem ( CustomCheatSheetAttribute item ) {
            if ( item == null ) {
                return;
            }

            Section section = this;
            foreach ( string sectionName in item.sections ) {
                if ( section == null ) {
                    break;                     // Can happen when there's a selection but the search result is empty
                }
                section = section.GetSection(sectionName);
            }

            if ( section == null ) {
                return;                     // Can happen when no tree has been generated!
            }
            VisualElement itemVe = section.root.Q<VisualElement>(item.cheatSheetName);

            if ( itemVe == null ) {
                return;             // Can happen when searching after selecting an item
            }

            VisualElement root         = itemVe.Q<VisualElement>("Root");
            root.style.backgroundColor = Color.grey;
        }
    }

    internal static class CheatSheetVisualElementBuilder {

        const string UXML_SECTION_TEMPLATE_RESOURCE_PATH = CheatSheetSelectorWindow.UXML_RESOURCE_PATH_ROOT + "CheatSheetSelectorSection";
        const string UXML_ITEM_TEMPLATE_RESOURCE_PATH    = CheatSheetSelectorWindow.UXML_RESOURCE_PATH_ROOT + "CheatSheetSelectorItem";

        static VisualTreeAsset sectionTemplate = Resources.Load<VisualTreeAsset>(UXML_SECTION_TEMPLATE_RESOURCE_PATH);
        static VisualTreeAsset itemTemplate    = Resources.Load<VisualTreeAsset>(UXML_ITEM_TEMPLATE_RESOURCE_PATH);

        public static VisualElement sectionElement {
            get {
                return CheatSheetVisualElementBuilder.sectionTemplate.CloneTree();
            }
        }

        public static VisualElement itemElement {
            get {
                return CheatSheetVisualElementBuilder.itemTemplate.CloneTree();
            }
        }

        public static VisualElement GetSectionVisualElement ( VisualElement parent, Section section, float hue ) {
            Color color = Color.HSVToRGB(hue, 1f, 1f);

            VisualElement existingElement = GetFoldout(parent, section.name);
            if ( existingElement != null ) {
                return existingElement;
            }

            VisualElement element = CheatSheetVisualElementBuilder.sectionElement;

            Foldout root = element.Q<Foldout>("Root");

            color.a = 0.2f;

            root.name = section.name;
            root.text = section.name;
            root.style.backgroundColor = color;

            return element;
        }

        public static VisualElement GetItemVisualElement ( CustomCheatSheetAttribute attribute, float hue ) {
            Color color = Color.HSVToRGB(hue, 1f, 1f);

            VisualElement element = CheatSheetVisualElementBuilder.itemElement;

            VisualElement root       = element.Q<VisualElement>("Root");
            VisualElement colorMark  = element.Q<VisualElement>("ColorMark");
            Label         pageNumber = element.Q<Label>("PageNumber");
            Label         label      = root.Q<Label>("Title");

            color.a = 1f;

            element.name                    = attribute.cheatSheetName;
            colorMark.style.backgroundColor = color;
            label.text                      = attribute.cheatSheetName;

            if ( attribute.showPageNumber ) {
                pageNumber.text = "Page " + attribute.pageNumber;
            } else {
                pageNumber.text = string.Empty;
            }

            element.Q<Button>("Root").clickable.clicked += () => {
                CheatSheetWindow window = EditorWindow.GetWindow<CheatSheetWindow>();
                window.selectedPage = attribute;
                window.OnNewCheatSheetSelection();
                EditorWindow.GetWindow<CheatSheetSelectorWindow>().Close();
            };

            return element;
        }

        internal static Foldout GetFoldout ( VisualElement parent, string name ) {
            return parent.Q<Foldout>(name);
        }
    }
}