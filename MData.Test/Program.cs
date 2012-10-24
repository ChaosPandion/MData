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
            var l = new List<List<List<IField>>>
            {
                new List<List<IField>>
                {
                    new List<IField>
                    {
                        new Field("A", typeof(int), 1),
                        new Field("B", typeof(int), 1),
                        new Field("C", typeof(int), 1)
                    },                
                    new List<IField>
                    {
                        new Field("A", typeof(int), 1),
                        new Field("B", typeof(int), 1),
                        new Field("C", typeof(int), 1)
                    }
                },                
                new List<List<IField>>
                {
                    new List<IField>
                    {
                        new Field("A", typeof(int), 1),
                        new Field("B", typeof(int), 1),
                        new Field("C", typeof(int), 1)
                    },                
                    new List<IField>
                    {
                        new Field("A", typeof(int), 1),
                        new Field("B", typeof(int), 1),
                        new Field("C", typeof(int), 1)
                    }
                }
            };
            var r = new Record(l);
            var a = r.ResultCount;
            var b = r.RecordCount;
            var c = r.NextRecord;
            var d = r.NextResult;

		}
	}
}