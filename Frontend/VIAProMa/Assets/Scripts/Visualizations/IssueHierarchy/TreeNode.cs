using System.Collections.Generic;

namespace i5.VIAProMa.Visualizations.IssueHierarchy
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; private set; }

        public List<TreeNode<T>> Children { get; private set; }

        public T Value { get; private set; }

        public TreeNode(TreeNode<T> parent, T value)
        {
            Children = new List<TreeNode<T>>();
            Parent = parent;
            Parent.Children.Add(this);
        }

        public void AddChild(TreeNode<T> child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public void SetParent(TreeNode<T> parent)
        {
            Parent = parent;
            parent.Children.Add(this);
        }
    }
}