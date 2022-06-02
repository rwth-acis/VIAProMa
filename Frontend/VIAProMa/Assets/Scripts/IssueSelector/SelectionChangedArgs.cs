using i5.VIAProMa.DataModel.API;
using System;

namespace i5.VIAProMa.IssueSelection
{
    /// <summary>
    /// Arguments of the generic item <typeparamref name="T"/> selection changed event
    /// It describes which generic item <typeparamref name="T"/> was either selected or deselected
    /// </summary>
    public class SelectionChangedArgs<T> : EventArgs
    {
        /// <summary>
        /// Creates a SelectionChangedArgs object with item <paramref name="item"/> 
        /// of type <typeparamref name="T"/> and if it was selected with <paramref name="selected"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="selected"></param>
        public SelectionChangedArgs(T item, bool selected)
        {
            ChangedItem = item;
            Selected = selected;
        }

        /// <summary>
        /// <typeparamref name="T"/> for which the selection status was changed
        /// </summary>
        public T ChangedItem { get; private set; }

        /// <summary>
        /// Indicates if <typeparamref name="T"/> was selected (true) or deselected (false)
        /// </summary>
        public bool Selected { get; private set; }
    }
}