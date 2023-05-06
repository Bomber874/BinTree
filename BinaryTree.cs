using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinTree
{
    public class BinaryTree<T> where T : IComparable<T>
    {
        public BinTreeNode<T> Root;
        public BinaryTree(T Value)
        {
            Root = new BinTreeNode<T>(Value, this);
        }
    }
}
