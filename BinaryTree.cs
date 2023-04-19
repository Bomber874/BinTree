using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinTree
{
    public class BinaryTree<T> where T : IComparable<T>

    {
        public BinaryTree<T>[] Child = { null, null };
        public bool IsRoot;
        public bool IsLeaf => Child[0] == null & Child[1] == null;
        BinaryTree<T> _root;
        BinaryTree<T> _parent;
        public BinaryTree<T> GetRoot()
        {
            return _root;
        }
        public BinaryTree<T> GetParent()
        {
            return _parent;
        }
        public T Value { get; set; }
        public BinaryTree(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            Value = value;
            IsRoot = true;
            _root = this;
        }

        private BinaryTree(T value, BinaryTree<T> Root, BinaryTree<T> Parent)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            Value = value;
            _root = Root;
            _parent = Parent;
        }
        public enum DetourType
        {
            QUEUE,    // Через очередь
            STACK,    // Через стек

        }
        public void Detour(DetourType detourType = DetourType.QUEUE)
        {
            if (!IsRoot)
            {
                _root.Detour(detourType);
                return;
            }
            switch (detourType)
            {
                case DetourType.QUEUE:
                    Queue<BinaryTree<T>> queue = new Queue<BinaryTree<T>>();
                    queue.Enqueue(this);
                    while (queue.Any())
                    {
                        BinaryTree<T> CurTree = queue.Dequeue();
                        Console.WriteLine(CurTree.Value);
                        if (CurTree.Child[0] != null)
                            queue.Enqueue(CurTree.Child[0]);
                        if (CurTree.Child[1] != null)
                            queue.Enqueue(CurTree.Child[1]);
                    }
                    break;
                case DetourType.STACK:
                    Stack<BinaryTree<T>> stack = new Stack<BinaryTree<T>>();
                    stack.Push(this);
                    while (stack.Any())
                    {
                        BinaryTree<T> CurTree = stack.Pop();
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
            Stack<BinaryTree<T>> stack = new Stack<BinaryTree<T>>();
            stack.Push(this);
            while (stack.Any())
            {
                BinaryTree<T> CurTree = stack.Pop();
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
        public BinaryTree<T> MinRightChild()
        {
            if (Child[1] == null)
            {
                return this;
            }
            BinaryTree<T> Min = Child[1];

            Queue <BinaryTree<T>> queue = new Queue<BinaryTree<T>>();
            queue.Enqueue(Child[1]);
            while (queue.Any())
            {
                BinaryTree<T> CurTree = queue.Dequeue();
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
        public BinaryTree<T>? FindByValue(T value)
        {
            Stack<BinaryTree<T>> stack = new Stack<BinaryTree<T>>();
            stack.Push(this);
            while (stack.Any())
            {
                BinaryTree<T> CurTree = stack.Pop();
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

        byte GetChildIDByObject(BinaryTree<T> Children)
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
        void ReplaseChildren(BinaryTree<T> Children, BinaryTree<T>? NewChildren)
        {
            Child[GetChildIDByObject(Children)] = NewChildren;
        }

        public void RemoveByValue(T value)
        {
            BinaryTree<T>? ToDelete = FindByValue(value);
            if (ToDelete == null)
            {
                return; // Нечего удалять
            }
            if (ToDelete.IsLeaf)
            {
                ToDelete = null;
                return;
            }
            if (ToDelete.Child[0] == null)  // Без левого потомка, значит просто заменяем ссылку на правого потомка
            {
                ToDelete = ToDelete.Child[1];
            }
            if (ToDelete.Child[1] == null)  // Аналогично с правым
            {
                ToDelete = ToDelete.Child[0];
            }

            BinaryTree<T> RMinV = ToDelete.MinRightChild();

            var Childs = ToDelete.Child;
            if (RMinV.IsLeaf)
            {
                RMinV._parent.ReplaseChildren(RMinV, null); //***
            }
            else
            {
                RMinV._parent.ReplaseChildren(RMinV, RMinV.Child[1]);   //***
            }
            //*** - Я думал что при ToDelete = RMinV и ссылка в родителей ToDelete должна смениться на RMinV
            ToDelete._parent.ReplaseChildren(ToDelete, RMinV); 
            //ToDelete = RMinV;
            RMinV.Child = Childs;
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
                    Child[0] = new BinaryTree<T>(AddValue, _root, this);
                    return;
                }
                Child[0].Add(AddValue);
            }
            else // Если же значение >=, то идём к правому
            {
                if (Child[1] == null)
                {
                    Child[1] = new BinaryTree<T>(AddValue, _root, this);
                    return;
                }
                Child[1].Add(AddValue);
            }
        }
    }
}
