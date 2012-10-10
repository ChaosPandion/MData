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
			var db = Database.GetSqlServerInstance("Data Source=PHLDEVCMSSQL01;Initial Catalog=Cms;User Id=CmsApplication;Password=cms;");
			for (int i = 0; i < 100000; i++)
			{
                var x = db.Context.ssam.ActivityList(CaseId: 1234);
			}
		}
	}
}
