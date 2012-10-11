using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Test
{
	class Program
	{
		static void Main(string[] args)
		{
            var db = Database.GetSqlServerCompactInstance(@"Data Source=C:\Users\Matthew\Projects\MData\MData.Test\Databases\PlayingCardsDatabase.sdf");
            var x = db.ExecReader(ctx => ctx["tests"]());
		}
	}
}
