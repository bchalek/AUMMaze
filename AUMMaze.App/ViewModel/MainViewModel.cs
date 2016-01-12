using AUMMaze.App.Common;
using AUMMaze.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AUMMaze.App.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		#region Fields

		Window currentWindow;

		System.Drawing.Bitmap mazeBitmap;
		BitmapImage mazeImage;
		string fileName;

		int? startX;
		int? startY;
		int? endX;
		int? endY;

		#endregion Fields

		#region Properties

		MazeService MazeService { get; set; }

		public BitmapImage MazeImage
		{
			get
			{
				return mazeImage;
			}
			set
			{
				mazeImage = value;
				OnPropertyChanged("MazeImage");
			}
		}


		public int? StartX
		{
			get
			{
				return startX;
			}
			set
			{
				startX = value;
				OnPropertyChanged("StartX");
			}
		}

		public int? StartY
		{
			get
			{
				return startY;
			}
			set
			{
				startY = value;
				OnPropertyChanged("StartY");
			}
		}

		public int? EndX
		{
			get
			{
				return endX;
			}
			set
			{
				endX = value;
				OnPropertyChanged("EndX");
			}
		}

		public int? EndY
		{
			get
			{
				return endY;
			}

			set
			{
				endY = value;
				OnPropertyChanged("EndY");
			}
		}

		#endregion Properties

		#region Cstr

		public MainViewModel(Window currentWindow)
		{
			this.currentWindow = currentWindow;
			MazeService = new MazeService();
			CreateGetMazeCommand();
			CreateSolveMazeCommand();

		}

		#endregion Cstr

		#region Commands

		public ICommand GetMazeCommand
		{
			get;
			internal set;
		}

		public ICommand SolveMazeCommand
		{
			get;
			internal set;
		}

		#endregion Commands

		#region Private Methods

		private void CreateGetMazeCommand()
		{
			GetMazeCommand = new RelayCommand(GetMazeExecute);
		}

		private void GetMazeExecute()
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				bool? dialogResult = openFileDialog.ShowDialog();
				if (dialogResult.GetValueOrDefault())
				{
					fileName = openFileDialog.FileName;
					mazeBitmap = MazeService.DrawMaze(fileName);
					MazeImage = DrawBitmapImage(mazeBitmap);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private BitmapImage DrawBitmapImage(System.Drawing.Bitmap bitmap)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			ms.Position = 0;
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = ms;
			bitmapImage.EndInit();
			return bitmapImage;
		}

		private void CreateSolveMazeCommand()
		{
			SolveMazeCommand = new RelayCommand(SolveMazeExecute, CanSolveMazeExecute);
		}

		private void SolveMazeExecute()
		{
			try
			{
				System.Drawing.Bitmap newMazeBitmap = MazeService.SolveAndDrawSolution(fileName, new Point(StartX.Value, StartY.Value), new Point(EndX.Value, EndY.Value), mazeBitmap);

				MazeImage = DrawBitmapImage(newMazeBitmap);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public bool CanSolveMazeExecute()
		{
			return StartX.HasValue &&
				StartY.HasValue &&
				EndX.HasValue &&
				EndY.HasValue &&
				MazeImage != null;
		}

		#endregion Private Methods

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged Members
	}
}
