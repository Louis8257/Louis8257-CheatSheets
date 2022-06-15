using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets {
    /// <summary>
    /// <para>The base class for every cheat sheet.<br/>
    /// You can even use this class to add interactivity in your cheat sheets!</para>
    /// <para><b>Must be paired with, atleast, one <see cref="CustomCheatSheetAttribute"/>.</b></para>
    /// <para><b>Must be serializable or it will break the stack system!</b></para>
    /// </summary>
    public abstract class CheatSheetElement {

        public CheatSheetWindow window;

        internal VisualTreeAsset visualTreeAsset;

        internal VisualElement _rootVisualElement;

        /// <summary>
        /// <para>The linked data to a specific <see cref="CustomCheatSheetAttribute"/>.</para>
        /// <para>It contains the page number, the page name and neighbor pages. See <see cref="CustomCheatSheetAttribute"/> for more information.</para>
        /// </summary>
        [SerializeField]
        internal CustomCheatSheetAttribute _attribute;

        /// <summary>
        /// <inheritdoc cref="_attribute"/>
        /// </summary>
        public CustomCheatSheetAttribute attribute {
            get {
                return this._attribute;
            }
        }

        public VisualElement rootVisualElement {
            get {
                return this._rootVisualElement;
            }
        }

        /// <summary>
        /// <para>Is called each time the <see cref="visualTreeAsset"/> is cloned.</para>
        /// </summary>
        public virtual void OnEnable () { }

        /// <summary>
        /// <para>Is called each time the <see cref="visualTreeAsset"/> is destroyed.</para>
        /// <para>Call <see cref="Destroy"/> instead if you want to destroy this element's <see cref="rootVisualElement"/>.</para>
        /// </summary>
        public virtual void OnDisable () { }

        public void Destroy () {
            this.OnDisable();
            this._rootVisualElement = null;
        }

        /// <summary>
        /// <para>Is called in <see cref="CheatSheetWindow.OnGUI"/>.</para>
        /// </summary>
        public virtual void OnGUI () { }

        public virtual bool RequiresConstantRepaint () {
            return false;
        }

        public VisualElement CloneTree ( CheatSheetWindow window ) {
            if ( this._rootVisualElement != null ) {
                this.OnDisable();
                this._rootVisualElement = null;
                this.window             = null;
            }

            this._rootVisualElement = this.visualTreeAsset.CloneTree();
            this.window             = window;
            this.OnEnable();
            return this._rootVisualElement;
        }
    
    }

    /// <summary>
    /// <para>An attribute to define cheat sheets linked to this class.</para>
    /// <para>You can also create multiple <see cref="CustomCheatSheetAttribute"/>!</para>
    /// </summary>
    [Serializable]
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CustomCheatSheetAttribute : Attribute {

        public const char SECTION_SEPARATOR     = '/';
        public const char PAGE_NUMBER_SEPARATOR = '#';

        /// <summary>
        /// <para>The path in the <see cref="CheatSheetTree"/>.</para>
        /// <para><b>Doesn't contain the page number!</b></para>
        /// </summary>
        public string cheatSheetPath;

        public string resourcePath;

        /// <summary>
        /// <para>Will be shown in the <see cref="cheatSheetName"/> if <see cref="showPageNumber"/> is enabled.</para>
        /// <para><i>The sorting system also uses this variable.</i></para>
        /// </summary>
        public int pageNumber = 0;

        /// <summary>
        /// <para>Is used when the <see cref="pageNumber"/> is superior than 0.</para>
        /// </summary>
        public string pageName = "";

        /// <summary>
        /// <para>Will show the page number on the right of the <see cref="Button"/>.</para>
        /// <para><i>True by default.</i></para>
        /// </summary>
        public bool showPageNumber = true;

        /// <summary>
        /// <para>Should describe this cheat sheet or explicitly say the conserned topics.</para>
        /// </summary>
        public string[] keywords;
        
        [SerializeField]
        public CheatSheetElement element;

        public CustomCheatSheetAttribute previousPage = null;
        public CustomCheatSheetAttribute nextPage     = null;

        public string[] splitedPath {
            get {
                return cheatSheetPath.Split(SECTION_SEPARATOR);
            }
        }

        public string cheatSheetName {
            get {
                string name = this.splitedPath[this.splitedPath.Length - 1];

                if ( !string.IsNullOrEmpty(this.pageName) ) {
                    if ( !string.IsNullOrEmpty(name) ) {
                        name += " - ";
                    }
                    name += this.pageName;
                }

                return name;
            }
        }

        public string[] sections {
            get {
                string[] split = this.splitedPath;
                Array.Resize<string>(ref split, split.Length - 1);
                return split;
            }
        }

        public CustomCheatSheetAttribute () { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cheatSheetPath">
        /// <para>Can contain the character '/' for sections like "A/B/C" and etc.</para>
        /// <para>Must contain the name of the cheat sheet at the end.</para></param>
        /// <param name="sheetRelativePath">Must contain the name of the UXML file, excluding its extension.</param>
        public CustomCheatSheetAttribute ( string cheatSheetPath, string sheetRelativePath ) {
            this.cheatSheetPath = cheatSheetPath.Trim();
            this.resourcePath   = sheetRelativePath.Trim();

            if ( this.cheatSheetName.Contains(PAGE_NUMBER_SEPARATOR) ) {
                string[] split      = this.cheatSheetPath.Split(PAGE_NUMBER_SEPARATOR);
                string pagePart     = split[1];
                int    ind          = pagePart.IndexOf(' ');
                string pageName     = pagePart.Substring(ind + 1);
                this.pageName       = pageName;

                string s = pagePart.Substring(0, pagePart.Length - pageName.Length);
                if ( s.Length > 0 ) {
                    // If has no name
                    if ( string.IsNullOrEmpty(s) ) {
                        this.pageNumber = Convert.ToInt32(pagePart);
                    } else {
                        this.pageNumber = Convert.ToInt32(s);
                    }
                }
                
                this.cheatSheetPath = split[0];
            }

            if ( !this.HasPageNumber() ) {
                this.showPageNumber = false;
            }
        }

        public CustomCheatSheetAttribute GetNextPage () {
            return this.nextPage;
        }

        public CustomCheatSheetAttribute GetPreviousPage() {
            return this.previousPage;
        }

        public bool HasPreviousPage () {
            return this.previousPage != null;
        }
        public bool HasNextPage () {
            return this.nextPage != null;
        }

        public bool HasPageNumber () {
            return this.pageNumber > 0;
        }

        public CustomCheatSheetAttribute GetPage ( int pageNumber ) {
            CustomCheatSheetAttribute res = this;

            if ( this.pageNumber > pageNumber ) {
                res = this.HasPreviousPage() ? this.previousPage.GetPage(pageNumber) : this;
            } else if ( this.pageNumber < pageNumber ) {
                res = this.HasNextPage() ? this.nextPage.GetPage(pageNumber) : this;
            }

            return res;
        }

        public CustomCheatSheetAttribute GetFirstPage () {
            return this.GetPage(1);
        }

        /// <summary>
        /// <para>Recursive.</para>
        /// </summary>
        /// <returns></returns>
        public CustomCheatSheetAttribute GetLastPage () {
            return this.HasNextPage() ? this.nextPage.GetLastPage() : this;
        }

        public int GetLastPageNumber () {
            return this.GetLastPage().pageNumber;
        }

        /// <summary>
        /// <para>Will look at <see cref="cheatSheetName"/> and <see cref="keywords"/> for any string containing the <paramref name="keyword"/>.</para>
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public bool ContainsKeyword ( string keyword ) {
            bool hasKeyword = false;
            hasKeyword = this.cheatSheetName.Contains(keyword);

            if ( hasKeyword || this.keywords == null ) {
                return hasKeyword;
            }

            foreach ( string k in this.keywords ) {
                hasKeyword = k.Contains(keyword);
                if ( hasKeyword ) {
                    break;
                }
            }
            return hasKeyword;
        }
    }
}