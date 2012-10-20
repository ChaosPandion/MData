using MData.Foundation;
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
            Nullable<int> q = 2;
            var field = new Field("A", typeof(int), 122);
            dynamic f = field;
            decimal? x = f;
            int y = f;

		}
	}
}