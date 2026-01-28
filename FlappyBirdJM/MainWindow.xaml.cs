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
using static System.Formats.Asn1.AsnWriter;

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

		List<(Rectangle top, Rectangle bottom, double x, bool pontmegvan)> csovek;

		double gravity = 2;
		double jumpStrength = -15;
		double velocityY = 0;

		bool gameRunning = false;
		int normalRekord = 0;
		int kodRekord = 0;
		int esoRekord = 0;
		int kodosesoRekord = 0;
		double TerMagassag = 360;
		int pontszam = 0;

		enum PalyaTipus
		{
			Normal,
			Kod,
			Eso,
			KodosEso
		}

		PalyaTipus aktualisPalya;


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
			Vissza.Visibility= Visibility.Visible;
			Rekordok.Visibility = Visibility.Hidden;
			NormalRek.Visibility = Visibility.Visible;
			EsoRek.Visibility = Visibility.Visible;
			KodRek.Visibility = Visibility.Visible;
			KodEsoRek.Visibility = Visibility.Visible;
			NormalRek.Text = $"Normál Rekord: {normalRekord}";
			EsoRek.Text = $"Esős Rekord: {esoRekord}";
			KodRek.Text = $"Ködös Rekord: {kodRekord}";
			KodEsoRek.Text = $"Ködös eső Rekord: {kodosesoRekord}";
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
			aktualisPalya = PalyaTipus.Normal;
			gravity = 2;
			StartGame();
		}
		private void Eso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			aktualisPalya = PalyaTipus.Eso;
			EsoKezdes();
			gravity = 2.75;
			StartGame();
		}
		private void Kod_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			aktualisPalya = PalyaTipus.Kod;
			KodSzintDoboz.Visibility = Visibility.Visible;
			gravity = 2;
			KodKezdes();
			StartGame();
		}
		private void KodosEso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			cso.Visibility = Visibility.Visible;
			aktualisPalya = PalyaTipus.KodosEso;
			KodSzintDoboz.Visibility = Visibility.Visible;
			KodKezdes();
			EsoKezdes();
			gravity = 2.75;
			StartGame();
		}

		private void csovekKezsitese()
		{
			csovek = new List<(Rectangle, Rectangle, double, bool)>
		{
			(csoTop1, csoBottom1, 800, false),
			(csoTop2, csoBottom2, 800 + csoTerulet, false),
			(csoTop3, csoBottom3, 800 + csoTerulet * 2, false),
			(csoTop4, csoBottom4, 800 + csoTerulet * 3, false)
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

		private void StartGame()
		{
			gameRunning = true;
			pontszam = 0;
			PontText.Text = "0";
			PontText.Visibility = Visibility.Visible;

			csovekKezsitese();

			velocityY = 0;
			Canvas.SetTop(Birb, 175);

			gameTimer.Stop();
			gameTimer.Tick -= GameLoop;
			gameTimer.Interval = TimeSpan.FromMilliseconds(20);
			gameTimer.Tick += GameLoop;
			gameTimer.Start();

			this.KeyDown -= MainWindow_KeyDown;
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
			if (velocityY > 0) 
			{
				BirbRotate.Angle = Math.Min(BirbRotate.Angle + 4, 90);
			}

			Canvas.SetTop(Birb, newTop);

			for (int i = 0; i < csovek.Count; i++)
			{
				var (top, bottom, x, pontmegvan) = csovek[i];
				x -= csoSebesseg;

				Canvas.SetLeft(top, x);
				Canvas.SetLeft(bottom, x);

				if (!pontmegvan && x + top.Width < Canvas.GetLeft(Birb))
				{
					pontszam++;
					PontText.Text = pontszam.ToString();
					pontmegvan = true;
				}

				if (x < -60)
				{
					x = szelsoCso() + csoTerulet;
					csovekVissza(top, bottom, x);
					pontmegvan = false;
				}

				csovek[i] = (top, bottom, x, pontmegvan);
			}
			Utkozes();
			Szelek();
		}
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (!gameRunning) return;

			if (e.Key == Key.Space)
			{
				velocityY = jumpStrength;
			}
			BirbRotate.Angle = -25;
		}


		DispatcherTimer rainTimer = new DispatcherTimer();
		Random random = new Random();

		private void EsoKezdes()
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
		private Rect BirbRect()
		{
			double x = Canvas.GetLeft(Birb);
			double y = Canvas.GetTop(Birb);

			return new Rect(x + 6, y + 6, Birb.Width - 10, Birb.Height - 10);
		}
		private Rect CsoRect(Rectangle rect)
		{
			double x = Canvas.GetLeft(rect);
			double y = Canvas.GetTop(rect);

			const double margin = 4;

			return new Rect(
				 x + margin,
				 y + margin,
				 rect.Width - margin * 2,
				 rect.Height - margin * 2
			);
		}
		private void Utkozes()
		{
			Rect birbRect = BirbRect();

			foreach (var (top, bottom, _, _) in csovek)
			{
				Rect topRect = CsoRect(top);
				Rect bottomRect = CsoRect(bottom);

				if (birbRect.IntersectsWith(topRect) ||
					birbRect.IntersectsWith(bottomRect))
				{
					GameOver();
					return;
				}
			}
		}
		private void Szelek()
		{
			double top = Canvas.GetTop(Birb);

			if (top <= 0 || top + Birb.Height >= 360)
			{
				GameOver();
			}
		}
		private void GameOver()
		{
			gameRunning = false;
			gameTimer.Stop();
			rainTimer.Stop();

			MessageBox.Show("Game Over", "Flappy Bird");
			BirbRotate.Angle = 0;
			VisszaJatek();
			RekordFrissites();
		}
		private void VisszaJatek()
		{
			gameRunning = false;

			gameTimer.Stop();
			rainTimer.Stop();
			PontText.Visibility = Visibility.Hidden;

			Canvas.SetTop(Birb, 175);

			cso.Visibility = Visibility.Hidden;

			RainLayer.Children.Clear();

			Palyak.Visibility = Visibility.Visible;
			Rekordok.Visibility = Visibility.Visible;

			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;

			Vissza.Visibility = Visibility.Hidden;

			NormalRek.Visibility = Visibility.Hidden;
			EsoRek.Visibility = Visibility.Hidden;
			KodRek.Visibility = Visibility.Hidden;
			KodEsoRek.Visibility = Visibility.Hidden;

			this.KeyDown -= MainWindow_KeyDown;
			KodSzintDoboz.Visibility = Visibility.Hidden;
		}

		private void RekordFrissites()
		{
			switch (aktualisPalya)
			{
				case PalyaTipus.Normal:
					if (pontszam > normalRekord)
						normalRekord = pontszam;
					break;

				case PalyaTipus.Kod:
					if (pontszam > kodRekord)
						kodRekord = pontszam;
					break;

				case PalyaTipus.Eso:
					if (pontszam > esoRekord)
						esoRekord = pontszam;
					break;

				case PalyaTipus.KodosEso:
					if (pontszam > kodosesoRekord)
						kodosesoRekord = pontszam;
					break;
			}
		}

		private void KodKezdes()
		{
			DoubleAnimation KodAnim = new DoubleAnimation
			{
				From = 0.45,
				To = 0.55,
				Duration = TimeSpan.FromSeconds(4),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever
			};

			((LinearGradientBrush)KodSzintDoboz.Fill)
				.GradientStops[1]
				.BeginAnimation(GradientStop.OffsetProperty, KodAnim);
		}
	}
}
