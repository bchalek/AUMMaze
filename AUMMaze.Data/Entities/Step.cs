using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AUMMaze.Data.Entities
{
	public class Step
	{
		public Point ActualPoint { get; set; }

		public Point? PreviousPoint { get; set; }

		public int Value { get; set; }
	}
}
