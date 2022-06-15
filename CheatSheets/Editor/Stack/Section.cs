using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace CheatSheets.Stack {
    internal class Section {

        public VisualElement root = new VisualElement();

        public string name;

        public List<CustomCheatSheetAttribute> attributes = new List<CustomCheatSheetAttribute>();

        public List<Section> subSections = new List<Section>();

        protected Section () { }

        public Section ( string name ) {
            this.name = name;
        }

        public Section GetSection ( string sectionName ) {
            Section res = null;
            foreach ( Section subSection in this.subSections ) {
                if ( subSection.name.Equals(sectionName) ) {
                    res = subSection;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// <para>Recursive.</para>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public VisualElement GetVisualElement ( VisualElement parent, float hue, float hueIncrementation ) {
            this.root.Clear();

            // Draw Sections
            foreach ( Section subSection in this.subSections ) {
                VisualElement subSectionVisualElement = CheatSheetVisualElementBuilder.GetSectionVisualElement(this.root, subSection, hue);
                VisualElement container = subSectionVisualElement.Q<Foldout>(subSection.name).contentContainer;

                this.root.Add(subSectionVisualElement);

                float hueSubIncrementation = hueIncrementation * (1f / (subSection.subSections.Count + subSection.attributes.Count));
                container.Add(subSection.GetVisualElement(container, hue, hueSubIncrementation));
                hue += hueIncrementation;
            }

            // Draw Items
            foreach ( CustomCheatSheetAttribute attribute in this.attributes ) {
                this.root.Add(CheatSheetVisualElementBuilder.GetItemVisualElement(attribute, hue));
                hue += hueIncrementation;
            }

            return this.root;
        }

        /// <summary>
        /// <para>Will sort this section in alphabetical order.</para>
        /// </summary>
        public void Sort () {
            this.attributes  = this.attributes.OrderBy(o => o.pageNumber).ThenBy(o => o.cheatSheetName).ToList();
            this.subSections = this.subSections.OrderBy(s => s.name.Length).ThenBy(s => s.name).ToList();

            foreach ( Section subSection in this.subSections ) {
                subSection.Sort();
            }
        }
    }
}