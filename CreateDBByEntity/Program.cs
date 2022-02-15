using System;

namespace CreateDBByEntity
{
    class Program
    {
        static void Main(string[] args)
        {
            string db = "CTGGeoDB";
            CreateDb.CreateDatabase(db);
            CreateDb.CreateTable(db);
            CreateDb.AddComment(db);
            Console.WriteLine("请按任意键退出");
            Console.ReadLine();
        }
    }
}
