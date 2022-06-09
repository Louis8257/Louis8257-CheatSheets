using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Creator {
    internal abstract class VisualItem {

        public VisualElement root;

        [SerializeField]
        public bool serializedFoldout = true;

        public Tuple<string, TextField> name = new Tuple<string, TextField>();

        public virtual void RegenerateVisualElement () {
            this.OnNameChanged();
        }

        public void OnNameChanged () {
            this.name.UpdateValue();
            UpdateFoldoutText();
        }

        //public abstract void ApplyValuesToVisualElements();

        public abstract void UpdateFoldoutText ();

        public abstract void UpdateSerializedFoldout ();

        public string GetCleanName ( bool includePageNumber = true ) {
            string name = "";
            // Put uppercase characters
            string[] split = this.name.GetValue().Trim().Split(' ');
            for ( int ind = 0; ind < split.Length; ind++ ) {
                char[] chars = split[ind].ToCharArray();
                chars[0] = Char.ToUpper(chars[0]);
                string newString = new string(chars);
                split[ind] = newString;
                name += newString;
            }

            // Clean whitespaces and add page number if wanted
            name = Regex.Replace(name, @"\s+", "");

            return name;
        }

    }
}
