using System;

namespace BinTree
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BinaryTree<int> Tree = new BinaryTree<int>(20);
            Tree.Add(10);
            Tree.Add(10);
            Tree.Add(25);
            Tree.Add(22);
            Tree.Add(26);
            Tree.Add(9);
            Tree.Add(9);
            Tree.Add(21);
            Tree.Add(23);
            Tree.Add(24);
            Tree.Detour(BinaryTree<int>.DetourType.STACK);

            Console.WriteLine(Tree.HasValue(22));
            Console.WriteLine("====\nУдаляем 22\n====");

            Tree.RemoveByValue(22);
            Tree.Detour(BinaryTree<int>.DetourType.STACK);

            Console.WriteLine(Tree.HasValue(22));
            Console.WriteLine(Tree.HasValue(20));
            Console.WriteLine(Tree.HasValue(10));

        }
    }
}