using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUMMaze.Services.Managers
{
	public class FileManager
	{
		public List<string> GetLinesFromFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				return File.ReadAllLines(filePath).ToList();
			}
			return new List<string>();
		}
	}
}
