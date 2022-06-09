using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CheatSheets.Creator {
    [Serializable]
    internal class Tuple<T1, T2> where T2 : BaseField<T1> {

        /// <summary>
        /// <para><i>When no <see cref="visualElement"/> is set, this field can set the other one later.</i></para>
        /// </summary>
        [SerializeField]
        T1 value;

        T2 visualElement;

        public Tuple() { }

        public Tuple ( T1 value ) {
            this.value = value;
        }

        public Tuple ( T2 visualElement ) {
            this.visualElement = visualElement;
        }

        public Tuple ( T1 value, T2 visualElement ) {
            this.value = value;
            this.visualElement = visualElement;
        }

        public void SetValue ( T1 value ) {
            this.value = value;

            if ( this.visualElement == null ) {
                return;
            }

            this.visualElement.value = this.value;
        }

        public void SetVisualElement ( T2 visualElement ) {
            this.visualElement = visualElement;
            this.visualElement.value = this.value;
            this.visualElement.RegisterCallback<KeyDownEvent>(e => this.UpdateValue());
            this.visualElement.RegisterValueChangedCallback(e => this.UpdateValue());
        }

        public T1 GetValue () {
            return this.value;
        }

        public T2 GetVisualElement () {
            return this.visualElement;
        }

        public void UpdateValue () {
            this.value = this.visualElement.value != null ? this.visualElement.value : default(T1);
        }

    }
}
