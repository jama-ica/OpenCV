namespace OpenCV;
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
using OpenCvSharp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : System.Windows.Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void button_Click(object sender, RoutedEventArgs e)
	{
		// 画像の読み込み
		Mat image = Cv2.ImRead("a.png");
		Mat template = Cv2.ImRead("b.png", ImreadModes.Grayscale);

		// 画像をグレースケールに変換
		Mat grayImage = new Mat();
		Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

		// テンプレートマッチング
		Mat result = new Mat();
		Cv2.MatchTemplate(grayImage, template, result, TemplateMatchModes.CCoeffNormed);

		// 一致率のしきい値を設定し、2値化
		Mat binaryResult = new Mat();
		double threshold = 0.8;
		Cv2.Threshold(result, binaryResult, threshold, 1.0, ThresholdTypes.Binary);

		// 非ゼロピクセルの場所を検出
		//var locations = new OpenCvSharp.Point[10];
		Mat locations = new Mat();
		Cv2.FindNonZero(binaryResult, locations);  // locations の型を Point[] に設定

		// 検出した位置に四角形を描画し、ロゴを塗りつぶして任意のテキストを追加
		for (int i = 0; i < locations.Total(); i++)
		{
			var pt = locations.At<OpenCvSharp.Point>();

			// テンプレートのサイズを取得
			var roi = new OpenCvSharp.Rect(pt.X, pt.Y, template.Width, template.Height);

			// 背景色で検出した領域を塗りつぶし
			Cv2.Rectangle(image, roi, Scalar.White, -1);

			// 任意のテキストを追加
			Cv2.PutText(image, "New Text", new OpenCvSharp.Point(pt.X, pt.Y + template.Height / 2),
						HersheyFonts.HersheySimplex, 1, Scalar.Black, 2);
		}

		// 結果を保存または表示
		Cv2.ImWrite("output_image.jpg", image);
		Cv2.ImShow("Result", image);
		Cv2.WaitKey(0);
		Cv2.DestroyAllWindows();
	}
}