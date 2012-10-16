using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Test
{
	class Program
	{
		class CaseModel
		{
			public int Id { get; set; }
            public int ClosureCodeId { get; set; }
            public List<CaseModel> CM { get; set; }
		}

		static void Main(string[] args)
		{
            var db = Databases.GetSqlServerDatabase("");//"Data Source=PHLDEVCMSSQL01;Initial Catalog=Cms;User Id=CmsApplication;Password=cms;");

			//var c = db.BuildCommand(cb => cb.Procedures.ssam.CaseGet(CaseId: 2)).ExecuteEntity<CaseModel>();
            //var result = 
            //    db.BuildCommand(cb => cb)
            //    .ExecuteEntity<CaseModel>(rb => 
            //        rb.BindEntity(() => new CaseModel(), 
            //            r => r.BindRecord().BindChildEntity(() => new int(), e => e)
            //        ));
		}
	}
}
