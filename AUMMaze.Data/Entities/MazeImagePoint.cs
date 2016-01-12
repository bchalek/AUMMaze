using AUMMaze.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUMMaze.Data.Entities
{
	public class MazeImagePoint
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Operation Operation { get; set; }

		public MazeImagePoint(int x, int y, int operation)
		{
			X = x;
			Y = y;
			Operation = (Operation)operation;
		}
		public MazeImagePoint(string line)
		{
			string[] parameters = line.Split(' ');
			X = int.Parse(parameters[0]);
			Y = int.Parse(parameters[1]);
			Operation = (Operation)int.Parse(parameters[2]);
		}
	}
}
