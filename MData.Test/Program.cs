using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MData.Foundation;
namespace MData.Test
{
	class Program
	{
		static void Main(string[] args)
		{
            //var db = SourceFactory.GetSqlServerCompactSource(@"Data Source=C:\Users\Matthew\Projects\MData\MData.Test\Databases\PlayingCardsDatabase.sdf");
            //var x = db.ExecuteToReader(configure: q => q.Timeout(1233), procedure: q => q.ssam.ActivityList(CaseID: 134));
            var fields = new[]
            {
                new Field("y", null, 1),
                new Field("z", null, 2),
                new Field("apple", null, 3),
                new Field("matthew", null, 4),
                new Field("shit", null, 5)
            };
            dynamic x = new Record(fields);
            var y = x.y;
            var z = x.z;

            var a = x.y;
            var b = x.z();
		}
	}
}
