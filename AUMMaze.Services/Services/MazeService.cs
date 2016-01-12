using AUMMaze.Data.Entities;
using AUMMaze.Data.Enums;
using AUMMaze.Services.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AUMMaze.Services
{
	public class MazeService
	{
		int width;
		int height;
		Point endPoint;
		List<Line> lineCollection;
		List<Step> mazePointsCollection;

		public System.Drawing.Bitmap DrawBitmap(List<MazeImagePoint> labiryntImagePointCollection, int step)
		{
			int width = labiryntImagePointCollection.Max(obj => obj.X + 1) * step;
			int height = labiryntImagePointCollection.Max(obj => obj.Y + 1) * step;
			System.Drawing.Bitmap maze = new System.Drawing.Bitmap(width, height);
			int startX = 0;
			int endX = width - 1;
			int startY = 0;
			int endY = height - 1;

			using (var editor = System.Drawing.Graphics.FromImage(maze))
			{
				editor.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black, 1), startX, startY, endX, endY);
			}

			foreach (MazeImagePoint labiryntImagePoint in labiryntImagePointCollection)
			{
				if (labiryntImagePoint.Operation == Operation.Nothing)
				{
					continue;
				}
				if (labiryntImagePoint.Operation == Operation.Right
					|| labiryntImagePoint.Operation == Operation.RightDown)
				{
					startX = labiryntImagePoint.X * step;
					endX = (labiryntImagePoint.X + 1) * step;
					startY = labiryntImagePoint.Y * step;
					endY = labiryntImagePoint.Y * step;
					using (var editor = System.Drawing.Graphics.FromImage(maze))
					{
						editor.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 3), startX, startY, endX, endY);
					}
				}
				if (labiryntImagePoint.Operation == Operation.Down
					|| labiryntImagePoint.Operation == Operation.RightDown)
				{
					startX = labiryntImagePoint.X * step;
					endX = labiryntImagePoint.X * step;
					startY = labiryntImagePoint.Y * step;
					endY = (labiryntImagePoint.Y + 1) * step;
					using (var editor = System.Drawing.Graphics.FromImage(maze))
					{
						editor.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 3), startX, startY, endX, endY);
					}
				}
			}
			return maze;
		}

		public List<Line> GetLines(List<MazeImagePoint> labiryntImagePointCollection)
		{
			List<Line> lineCollection = new List<Line>();
			foreach (MazeImagePoint labiryntImagePoint in labiryntImagePointCollection)
			{
				if (labiryntImagePoint.Operation == Operation.Nothing)
				{
					continue;
				}
				if (labiryntImagePoint.Operation == Operation.Right
					|| labiryntImagePoint.Operation == Operation.RightDown)
				{
					Line line = new Line();

					line.StartX = labiryntImagePoint.X;
					line.EndX = (labiryntImagePoint.X + 1);
					line.StartY = labiryntImagePoint.Y;
					line.EndY = labiryntImagePoint.Y;
					lineCollection.Add(line);
				}
				if (labiryntImagePoint.Operation == Operation.Down
					|| labiryntImagePoint.Operation == Operation.RightDown)
				{
					Line line = new Line();

					line.StartX = labiryntImagePoint.X;
					line.EndX = labiryntImagePoint.X;
					line.StartY = labiryntImagePoint.Y;
					line.EndY = (labiryntImagePoint.Y + 1);
					lineCollection.Add(line);
				}
			}
			return lineCollection;
		}

		public List<MazeImagePoint> GetPointsFromFile(string filePath)
		{
			List<MazeImagePoint> labiryntImagePointCollection = new List<MazeImagePoint>();

			FileManager fileManager = new FileManager();
			List<string> lines = fileManager.GetLinesFromFile(filePath);

			lines.ForEach(obj => labiryntImagePointCollection.Add(new MazeImagePoint(obj)));

			return labiryntImagePointCollection;
		}

		public System.Drawing.Bitmap DrawMaze(string inputFilePath, int step = 120)
		{
            List<MazeImagePoint> labiryntImagePointCollection = GetPointsFromFile(inputFilePath);
			System.Drawing.Bitmap maze = DrawBitmap(labiryntImagePointCollection, step);
			return maze;
		}

		public System.Drawing.Bitmap SolveAndDrawSolution(string inputFilePath, Point start, Point end, System.Drawing.Bitmap maze, int step = 120)
		{
			List<MazeImagePoint> labiryntImagePointCollection = GetPointsFromFile(inputFilePath);
			lineCollection = GetLines(labiryntImagePointCollection);
			width = labiryntImagePointCollection.Max(obj => obj.X + 1);
			height = labiryntImagePointCollection.Max(obj => obj.Y + 1);
			endPoint = end;
			mazePointsCollection = new List<Step>();
			NextStep(new Step()
			{
				PreviousPoint = null,
				ActualPoint = start,
				Value = 0
			});
			System.Drawing.Bitmap solvedMaze = (System.Drawing.Bitmap)maze.Clone();
            List<Point> solutionPoints = FindWayBack(start, end);

			for (int i = 0; i < solutionPoints.Count - 1; i++)
			{
				using (var editor = System.Drawing.Graphics.FromImage(solvedMaze))
				{
					float startX = (float)(solutionPoints[i].X * step) + (step / 2);
					float startY = (float)(solutionPoints[i].Y * step) + (step / 2);
					float endX = (float)(solutionPoints[i + 1].X * step) + (step / 2);
					float endY = (float)(solutionPoints[i + 1].Y * step) + (step / 2);
					editor.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red, 2), startX, startY, endX, endY);
				}
			}

			return solvedMaze;
		}
		
		private List<Point> FindWayBack(Point start, Point end)
		{
			List<Point> route = new List<Point>();
			if (mazePointsCollection.Any(obj => obj.ActualPoint.Equals(end)))
			{
				Point actualPoint = end;
				route.Add(actualPoint);
				while (!actualPoint.Equals(start))
				{
					actualPoint = mazePointsCollection.FirstOrDefault(obj => obj.ActualPoint.Equals(actualPoint)).PreviousPoint.Value;
					route.Insert(0, actualPoint);
				}
			}
			return route;
		}

		private void NextStep(Step currentStep)
		{
			mazePointsCollection.Add(currentStep);
			Point currentPoint = currentStep.ActualPoint;
			int value = currentStep.Value + 1;
			if (currentPoint.Equals(endPoint))
				return;

			Point newPoint;
			if (!lineCollection.Any(obj => obj.StartX == currentPoint.X
			&& obj.EndX == currentPoint.X + 1
			&& obj.StartY == currentPoint.Y
			&& obj.EndY == currentPoint.Y)
			&& currentPoint.Y - 1 >= 0
			)
			{
				newPoint = new Point(currentPoint.X, currentPoint.Y - 1);
				MakeNextStep(currentStep.PreviousPoint, new Step()
				{
					PreviousPoint = currentPoint,
					ActualPoint = newPoint,
					Value = value
				});
			}
			if (!lineCollection.Any(obj => obj.StartX == currentPoint.X
			&& obj.EndX == currentPoint.X + 1
			&& obj.StartY == currentPoint.Y + 1
			&& obj.EndY == currentPoint.Y + 1)
			&& currentPoint.Y + 1 < height
			)
			{
				newPoint = new Point(currentPoint.X, currentPoint.Y + 1);
				MakeNextStep(currentStep.PreviousPoint, new Step()
				{
					PreviousPoint = currentPoint,
					ActualPoint = newPoint,
					Value = value
				});
			}
			if (!lineCollection.Any(obj => obj.StartX == currentPoint.X
			&& obj.EndX == currentPoint.X
			&& obj.StartY == currentPoint.Y
			&& obj.EndY == currentPoint.Y + 1)
			&& currentPoint.X - 1 >= 0
			)
			{
				newPoint = new Point(currentPoint.X - 1, currentPoint.Y);
				MakeNextStep(currentStep.PreviousPoint, new Step()
				{
					PreviousPoint = currentPoint,
					ActualPoint = newPoint,
					Value = value
				});
			}
			if (!lineCollection.Any(obj => obj.StartX == currentPoint.X + 1
			&& obj.EndX == currentPoint.X + 1
			&& obj.StartY == currentPoint.Y
			&& obj.EndY == currentPoint.Y + 1)
			&& currentPoint.X + 1 < width
			)
			{
				newPoint = new Point(currentPoint.X + 1, currentPoint.Y);
				MakeNextStep(currentStep.PreviousPoint, new Step()
				{
					PreviousPoint = currentPoint,
					ActualPoint = newPoint,
					Value = value
				});
			}
		}

		private void MakeNextStep(Point? previousPoint, Step currentStep)
		{
			if (!currentStep.ActualPoint.Equals(previousPoint))
			{
				Step sameActualPointStep = mazePointsCollection.FirstOrDefault(obj => obj.ActualPoint.Equals(currentStep.ActualPoint));
				if (sameActualPointStep != null
					&& sameActualPointStep.Value <= currentStep.Value)
				{
					return;
				}
				else if (sameActualPointStep != null)
				{
					mazePointsCollection.Remove(sameActualPointStep);
				}
				NextStep(currentStep);
			}
		}
	}
}