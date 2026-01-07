using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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

		double gravity = 1.5;
		double jumpStrength = -15;
		double velocityY = 0;

		bool gameRunning = false;
		int normalRekord = 0;
		int kodRekord = 0;
		int esoRekord = 0;
		int kodosesoRekord = 0;	

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
			StartGame();
		}
		private void Eso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			StartGame();
		}
		private void Kod_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			StartGame();
		}
		private void KodosEso_Click(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Hidden;
			Kod.Visibility = Visibility.Hidden;
			Eso.Visibility = Visibility.Hidden;
			Kodos_Eso.Visibility = Visibility.Hidden;
			StartGame();
		}

		private void StartGame()
		{
			gameRunning = true;

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
		}
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (!gameRunning) return;

			if (e.Key == Key.Space)
			{
				velocityY = jumpStrength;
			}
		}
	}
}