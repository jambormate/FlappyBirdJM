using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FlappyBirdJM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		DispatcherTimer gameTimer = new DispatcherTimer();
		Random rand = new Random();

		double csoSebesseg = 4;
		double csoLyuk = 140;
		double csoTerulet = 280;

		List<(Rectangle top, Rectangle bottom, double x)> csovek;

		double gravity = 1.5;
		double jumpStrength = -15;
		double velocityY = 0;

		bool gameRunning = false;
		int normalRekord = 0;
		int kodRekord = 0;
		int esoRekord = 0;
		int kodosesoRekord = 0;
		double TerMagassag = 360;

		private void Palyak_Click(object sender, RoutedEventArgs e)
		{
			Palyak.Visibility = Visibility.Hidden;
			Rekordok.Visibility = Visibility.Hidden;
			Normal.Visibility = Visibility.Visible;
			Kod.Visibility = Visibility.Visible;
			Eso.Visibility = Visibility.Visible;
			Kodos_Eso.Visibility = Visibility.Visible;
			Vissza.Visibility = Visibility.Hidden;
		}

		private void Rekordok_Click(object sender, RoutedEventArgs e)
		{
			Palyak.Visibility = Visibility.Hidden;
			Rekordok.Visibility = Visibility.Hidden;
			NormalRek.Visibility = Visibility.Visible;
			EsoRek.Visibility = Visibility.Visible;
			KodRek.Visibility = Visibility.Visible;
			KodEsoRek.Visibility = Visibility.Visible;
		}

		private void Vissza_Click(object sender, RoutedEventArgs e)
		{
			Palyak.Visibility = Visibility.Visible;
			Rekordok.Visibility = Visibility.Visible;
			NormalRek.Visibility = Visibility.Hidden;
			EsoRek.Visibility = Visibility.Hidden;
			KodRek.Visibility = Visibility.Hidden;
			KodEsoRek.Visibility = Visibility.Hidden;
		}
		private void Normal_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			StartGame();
		}
		private void Eso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			StartRain();
			gravity = 2.5;
			StartGame();
		}
		private void Kod_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			StartGame();
		}
		private void KodosEso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			StartRain();
			gravity = 2.5;
			StartGame();
		}

		private void csovekKezsitese()
		{
			csovek = new List<(Rectangle, Rectangle, double)>
		{
			(csoTop1, csoBottom1, 800),
			(csoTop2, csoBottom2, 800 + csoTerulet),
			(csoTop3, csoBottom3, 800 + csoTerulet * 2),
			(csoTop4, csoBottom4, 800 + csoTerulet * 3)
		};

			foreach (var csok in csovek)
			{
				csovekVissza(csok.top, csok.bottom, csok.x);
			}
		}
		private double szelsoCso()
		{
			double maxX = 0;
			foreach (var pipe in csovek)
			{
				if (pipe.x > maxX)
					maxX = pipe.x;
			}
			return maxX;
		}

		private void StartGame()
		{
			gameRunning = true;
			csovekKezsitese();

			velocityY = 0;
			Canvas.SetTop(Birb, 175);


			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
			gameTimer.Start();

			this.KeyDown += MainWindow_KeyDown;
			this.Focus();
		}
		private void GameLoop(object sender, EventArgs e)
		{
			if (!gameRunning) return;


			velocityY += gravity;

			double currentTop = Canvas.GetTop(Birb);
			double newTop = currentTop + velocityY;
			double floor = 315;
			if (newTop >= floor)
			{
				newTop = floor;
				velocityY = 0;
			}
			if (newTop <= 0)
			{
				newTop = 0;
				velocityY = 0;
			}

			Canvas.SetTop(Birb, newTop);

			for (int i = 0; i < csovek.Count; i++)
			{
				var (top, bottom, x) = csovek[i];
				x -= csoSebesseg;

				Canvas.SetLeft(top, x);
				Canvas.SetLeft(bottom, x);

				if (x < -60)
				{
					x = szelsoCso() + csoTerulet;
					csovekVissza(top, bottom, x);
				}

				csovek[i] = (top, bottom, x);
			}
		}
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (!gameRunning) return;

			if (e.Key == Key.Space)
			{
				velocityY = jumpStrength;
			}
		}

		private void csovekVissza(Rectangle top, Rectangle bottom, double startX)
		{
			double minTopHeight = 40;
			double maxTopHeight = TerMagassag - csoLyuk - 40;

			double topHeight = rand.Next((int)minTopHeight, (int)maxTopHeight);

			double bottomHeight = TerMagassag - topHeight - csoLyuk;

			top.Height = topHeight;
			bottom.Height = bottomHeight;

			Canvas.SetLeft(top, startX);
			Canvas.SetTop(top, 0);

			Canvas.SetLeft(bottom, startX);
			Canvas.SetTop(bottom, topHeight + csoLyuk);
		}

		DispatcherTimer rainTimer = new DispatcherTimer();
		Random random = new Random();

		private void StartRain()
		{
			rainTimer.Interval = TimeSpan.FromMilliseconds(50);
			rainTimer.Tick += RainTimer_Tick;
			rainTimer.Start();
		}

		private void RainTimer_Tick(object sender, EventArgs e)
		{
			int dropsPerTick = 10;

			for (int i = 0; i < dropsPerTick; i++)
			{
				Line drop = new Line
				{
					Stroke = Brushes.LightBlue,
					StrokeThickness = 3,
					X1 = 0,
					Y1 = 0,
					X2 = 0,
					Y2 = 10,
					Opacity = 0.5
				};
				double startX = random.NextDouble() * Hatter.ActualWidth;
				Canvas.SetLeft(drop, startX);
				Canvas.SetTop(drop, 0);

				RainLayer.Children.Add(drop);
				DoubleAnimation fallAnimation = new DoubleAnimation
				{
					From = 0,
					To = Hatter.ActualHeight,
					Duration = TimeSpan.FromSeconds(1.2),
					FillBehavior = FillBehavior.Stop
				};

				fallAnimation.Completed += (s, a) => RainLayer.Children.Remove(drop);

				drop.BeginAnimation(Canvas.TopProperty, fallAnimation);
			}
		}
	}
}
