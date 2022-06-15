using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Stack {
    /// <summary>
    /// <para>Class to find a specific <see cref="DiagramObjectEditor"/> for a <see cref="DiagramObject"/>.</para>
    /// <para><i>Inspired from this source: <see href="https://stackoverflow.com/questions/2362580/discovering-derived-types-using-reflection"/><br/>
    /// Also inspired from Conceptiode's reflector, one of my plugins ;).</i></para>
    /// </summary>
    [Serializable]
    internal class CheatSheetStack {
        /// <summary>
        /// <para>Stores any known <see cref="CheatSheetElement"/> with their sections.</para>
        /// </summary>
        internal CheatSheetTree tree;

        /// <summary>
        /// <para>List of <see cref="CustomCheatSheetAttribute"/> for fast search.</para>
        /// </summary>
        internal List<CustomCheatSheetAttribute> loadedAttributes;

        /// <summary>
        /// <para>Will rebuild the <see cref="tree"/> and rescan for every <see cref="CustomCheatSheetAttribute"/>.</para>
        /// </summary>
        /// <returns></returns>
        public void Build () {
            this.loadedAttributes = this.Scan();
            this.tree = new CheatSheetTree(this.loadedAttributes);
        }

        internal List<CustomCheatSheetAttribute> Scan () {
            List<Type> foundCheatSheetTypes = FindAllDerivedTypes<CheatSheetElement>();
            List<CustomCheatSheetAttribute> foundAttributes = new List<CustomCheatSheetAttribute>();

            //Debug.Log(string.Format("[CheatSheet] Found {0} types.", foundCheatSheetTypes.Count));

            // Scan every CheatSheet class
            foreach ( Type typeInAssembly in foundCheatSheetTypes ) {
                object[] attributes = typeInAssembly.GetCustomAttributes(typeof(CustomCheatSheetAttribute), true);

                //Debug.Log(string.Format("[CheatSheet] Found {0} attributes.", attributes.Length));

                if ( attributes == null || attributes.Length == 0 ) {
                    throw new AttributeMissingException(typeInAssembly); 
                }

                // Scan attributes and populate data
                foreach ( CustomCheatSheetAttribute attribute in attributes ) {
                    CheatSheetElement element = (CheatSheetElement)Activator.CreateInstance(typeInAssembly);
                    element._attribute = attribute;
                    element.visualTreeAsset = Resources.Load<VisualTreeAsset>(attribute.resourcePath);
                    if ( element.visualTreeAsset == null ) {
                        Debug.LogError(string.Format("[CheatSheets] Couldn't find visual tree asset! {0} is not a valid relative path or the file is missing!" +
                                                     "Be sure that the name of the UXML file is unique and no other files has the same name in the same directory."
                                                    , attribute.resourcePath));
                        continue;
                    }
                    attribute.element = element;
                    foundAttributes.Add(attribute);
                }

                // Link pages (needs a second loop because the page numbers might not be all loaded and sorted in order)
                foreach ( CustomCheatSheetAttribute attribute in foundAttributes ) {
                    if ( !attribute.HasPageNumber() ) {
                        continue;
                    }

                    if ( attribute.pageNumber > 1 ) {
                       attribute.previousPage = foundAttributes.SingleOrDefault(o => o.cheatSheetPath.Equals(attribute.cheatSheetPath) && o.pageNumber == attribute.pageNumber - 1);
                        // Exception situation
                        if ( attribute.previousPage == null ) {
                            throw new CantFindPageException(attribute.cheatSheetPath, attribute.pageNumber - 1);
                        }
                    }

                    // The script doesn't know if there's a next page so the value can be null
                    attribute.nextPage = foundAttributes.SingleOrDefault(o => o.cheatSheetPath.Equals(attribute.cheatSheetPath) && o.pageNumber == attribute.pageNumber + 1);
                }
            }

            return foundAttributes;
        }

        internal List<Type> FindAllDerivedTypes <T> (){
            Assembly assembly = Assembly.GetAssembly(typeof(T));
            Type derivedType = typeof(T);
            return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToList();
        }
    }
    internal class AttributeMissingException : Exception {
        public AttributeMissingException ( Type type ) : base(string.Format("[CheatSheets] The type {0} doesn't have a CustomCheatSheet attribute! Please add one.", type.FullName)) {
        }
    }

    internal class CantFindPageException : Exception {
        public CantFindPageException ( string cheatSheetPath, int pageNb ) : base(string.Format("[CheatSheets] Couldn't find page number {0} from \"{1}\"! You may have forgot to create this page in the attribute or the page is in another section.", pageNb, cheatSheetPath)) {
        }
    }
}