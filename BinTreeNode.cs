using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinTree
{
    public class BinTreeNode<T> where T : IComparable<T>
    {
        public BinTreeNode<T>[] Child = { null, null };
        public bool IsRoot;
        public bool IsLeaf => Child[0] == null & Child[1] == null;
        BinTreeNode<T>? _parent;
        public BinTreeNode<T>? GetParent()
        {
            return _parent;
        }
        public T Value { get; set; }
        public BinTreeNode(T value, BinaryTree<T> BT)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            Value = value;
            IsRoot = true;
            BT.Root = this;
        }

        private BinTreeNode(T value, BinTreeNode<T> Parent)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            Value = value;
            _parent = Parent;
        }
        public enum DetourType
        {
            QUEUE,    // Через очередь
            STACK,    // Через стек

        }
        public void Detour(DetourType detourType = DetourType.QUEUE)
        {
            switch (detourType)
            {
                case DetourType.QUEUE:
                    Queue<BinTreeNode<T>> queue = new Queue<BinTreeNode<T>>();
                    queue.Enqueue(this);
                    while (queue.Any())
                    {
                        BinTreeNode<T> CurTree = queue.Dequeue();
                        Console.WriteLine(CurTree.Value);
                        if (CurTree.Child[0] != null)
                            queue.Enqueue(CurTree.Child[0]);
                        if (CurTree.Child[1] != null)
                            queue.Enqueue(CurTree.Child[1]);
                    }
                    break;
                case DetourType.STACK:
                    Stack<BinTreeNode<T>> stack = new Stack<BinTreeNode<T>>();
                    stack.Push(this);
                    while (stack.Any())
                    {
                        BinTreeNode<T> CurTree = stack.Pop();
                        Console.WriteLine(CurTree.Value);
                        if (CurTree.Child[1] != null)
                            stack.Push(CurTree.Child[1]);
                        if (CurTree.Child[0] != null)
                            stack.Push(CurTree.Child[0]);
                    }
                    break;

            }
        }

        public bool HasValue(T value)
        {
            Stack<BinTreeNode<T>> stack = new Stack<BinTreeNode<T>>();
            stack.Push(this);
            while (stack.Any())
            {
                BinTreeNode<T> CurTree = stack.Pop();
                if (value.CompareTo(CurTree.Value) == 0)
                {
                    return true;
                }
                if (CurTree.Child[1] != null)
                    stack.Push(CurTree.Child[1]);
                if (CurTree.Child[0] != null)
                    stack.Push(CurTree.Child[0]);
            }
            return false;
        }
        public BinTreeNode<T> MinRightChild()
        {
            if (Child[1] == null)
            {
                return this;
            }
            BinTreeNode<T> Min = Child[1];

            Queue<BinTreeNode<T>> queue = new Queue<BinTreeNode<T>>();
            queue.Enqueue(Child[1]);
            while (queue.Any())
            {
                BinTreeNode<T> CurTree = queue.Dequeue();
                if (CurTree.Value.CompareTo(Min.Value) < 0)
                {
                    Min = CurTree;
                }
                if (CurTree.Child[0] != null)
                    queue.Enqueue(CurTree.Child[0]);
                if (CurTree.Child[1] != null)
                    queue.Enqueue(CurTree.Child[1]);
            }
            return Min;
        }
        public BinTreeNode<T>? FindByValue(T value)
        {
            Stack<BinTreeNode<T>> stack = new Stack<BinTreeNode<T>>();
            stack.Push(this);
            while (stack.Any())
            {
                BinTreeNode<T> CurTree = stack.Pop();
                if (CurTree.Value.CompareTo(value) == 0)
                {
                    return CurTree;
                }
                if (CurTree.Child[1] != null)
                    stack.Push(CurTree.Child[1]);
                if (CurTree.Child[0] != null)
                    stack.Push(CurTree.Child[0]);
            }
            return null;
        }

        byte GetChildIDByObject(BinTreeNode<T> Children)
        {
            for (byte i = 0; i < 2; i++)
            {
                if (Child[i] == Children)
                {
                    return i;
                }
            }

            throw new Exception("Не найден потомок");
        }
        void ReplaseChildren(BinTreeNode<T> Children, BinTreeNode<T>? NewChildren)
        {
            Child[GetChildIDByObject(Children)] = NewChildren;
        }
        // Возможно стоит сделать: BinTree и BinTreeItem
        // Тогда при удалении последнего BinTreeItem, BinTree не потеряет смысла
        public void RemoveByValue(T value)
        {
            BinTreeNode<T>? ToDeleteMain = FindByValue(value);

            if (ToDeleteMain == null)
            {
                return; // Нечего удалять
            }


            if (ToDeleteMain.IsLeaf)
            {
                if (ToDeleteMain._parent != null)
                {
                    ToDeleteMain._parent.ReplaseChildren(ToDeleteMain, null);
                    return;
                }
                
                // Тут должно быть самоубийство дерева
                return;
            }
            if (ToDeleteMain.Child[0] == null)  // Без левого потомка, значит просто заменяем ссылку на правого потомка
            {
                ToDeleteMain.Child[1]._parent = ToDeleteMain._parent;
                if (ToDeleteMain._parent != null)
                {
                    ToDeleteMain._parent.ReplaseChildren(ToDeleteMain, ToDeleteMain.Child[1]);
                }
                return;
            }

            if (ToDeleteMain.Child[1] == null)// Аналогично с правым
            {
                ToDeleteMain.Child[0]._parent = ToDeleteMain._parent;
                if (ToDeleteMain._parent != null)
                {
                    ToDeleteMain._parent.ReplaseChildren(ToDeleteMain, ToDeleteMain.Child[0]);
                }
                return;
            }

            if (ToDeleteMain == null)
            {
                return;
            }


            // тут
            BinTreeNode<T> RMinV = ToDeleteMain.MinRightChild();

            var Childs = ToDeleteMain.Child;

            if (RMinV.IsLeaf)
            {
                RMinV._parent.ReplaseChildren(RMinV, null); //***
            }
            else
            {
                RMinV._parent.ReplaseChildren(RMinV, RMinV.Child[1]);   //***
            }
            if (ToDeleteMain._parent == null)
            {
                throw new NotImplementedException();
                // Тут будет новый рут, в лице RMinV
            }
            //*** - Я думал что при ToDelete = RMinV и ссылка в родителей ToDelete должна смениться на RMinV
            // Вот тут
            // У родителя ToDelete нет ребёнка ToDelete
            ToDeleteMain._parent.ReplaseChildren(ToDeleteMain, RMinV);
            //ToDelete = RMinV;
            RMinV.Child = Childs;
            //RMinV.Child[0] = Childs[0];
            //RMinV.Child[1] = Childs[1];
            return;

        }
        public void Add(T AddValue)
        {
            if (AddValue == null)
            {
                throw new ArgumentNullException();
            }

            if (AddValue.CompareTo(Value) < 0)  // Если значение меньше, то идём к левому потомку
            {
                if (Child[0] == null)
                {
                    Child[0] = new BinTreeNode<T>(AddValue, this);
                    return;
                }
                Child[0].Add(AddValue);
            }
            else // Если же значение >=, то идём к правому
            {
                if (Child[1] == null)
                {
                    Child[1] = new BinTreeNode<T>(AddValue, this);
                    return;
                }
                Child[1].Add(AddValue);
            }
        }
    }
}
