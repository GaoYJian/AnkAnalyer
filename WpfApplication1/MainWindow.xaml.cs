using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Stack<StrokeCollection> tempList = new Stack<StrokeCollection>();

        public MainWindow()
        {
            InitializeComponent();
            inkC.Strokes.StrokesChanged += Strokes_StrokesChanged;
        }

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            string[] re = Recognize(inkC.Strokes);
            for (int i = 0; i < re.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        b1.Content = re[i];
                        break;
                    case 1:
                        b2.Content = re[i];
                        break;
                    case 2:
                        b3.Content = re[i];
                        break;
                    case 3:
                        b4.Content = re[i];
                        break;
                    case 4:
                        b5.Content = re[i];
                        break;
                    default:
                        break;
                }
            }
            if (e.Added.Count > 0)
            {
                tempList.Push(e.Added);
            }
        }

        private void btnRe_Click(object sender, RoutedEventArgs e)
        {
            //InkAnalyzer theInkAnalyer = new InkAnalyzer();
            //theInkAnalyer.AddStrokes(inkC.Strokes);

            //theInkAnalyer.SetStrokesLanguageId(inkC.Strokes, 0x0804);
            //theInkAnalyer.SetStrokesType(inkC.Strokes, StrokeType.Writing);
            //AnalysisStatus status = theInkAnalyer.Analyze();
            //if (status.Successful)
            //{
            //    Tb1.Text = theInkAnalyer.GetRecognizedString();
            //    for (int i = 0; i < theInkAnalyer.GetAlternates().Count; i++)
            //    {
            //        Tb1.Text += theInkAnalyer.GetAlternates()[i].RecognizedString;
            //    }

            //}
            //else
            //{
            //    MessageBox.Show("识别失败");
            //}
            string[] re = Recognize(inkC.Strokes);
            if (re.Length > 0)
            {
                Tb1.Text = re[0];
            }
        }

        private void inkC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            #region 识别率比较差，不用
            //InkAnalyzer theInkAnalyer = new InkAnalyzer();
            //theInkAnalyer.AddStrokes(inkC.Strokes);

            //theInkAnalyer.SetStrokesLanguageId(inkC.Strokes, 0x0804);
            //theInkAnalyer.SetStrokesType(inkC.Strokes, StrokeType.Writing);
            //AnalysisStatus status = theInkAnalyer.Analyze();
            //if (status.Successful)
            //{
            //    //Tb1.Text = theInkAnalyer.GetRecognizedString();
            //    for (int i = 0; i < theInkAnalyer.GetAlternates().Count; i++)
            //    {
            //        switch (i)
            //        {
            //            case 0:
            //                b1.Content = theInkAnalyer.GetAlternates()[i].RecognizedString;
            //                break;
            //            case 1:
            //                b2.Content = theInkAnalyer.GetAlternates()[i].RecognizedString;
            //                break;
            //            case 2:
            //                b3.Content = theInkAnalyer.GetAlternates()[i].RecognizedString;
            //                break;
            //            case 3:
            //                b4.Content = theInkAnalyer.GetAlternates()[i].RecognizedString;
            //                break;
            //            case 4:
            //                b5.Content = theInkAnalyer.GetAlternates()[i].RecognizedString;
            //                break;
            //            default:
            //                break;
            //        }
            //    }

            //}
            //else
            //{
            //    MessageBox.Show("识别失败");
            //}
            #endregion

            //string[] re = Recognize(inkC.Strokes);
            //for (int i = 0; i < re.Length; i++)
            //{
            //    switch (i)
            //    {
            //        case 0:
            //            b1.Content = re[i];
            //            break;
            //        case 1:
            //            b2.Content = re[i];
            //            break;
            //        case 2:
            //            b3.Content = re[i];
            //            break;
            //        case 3:
            //            b4.Content = re[i];
            //            break;
            //        case 4:
            //            b5.Content = re[i];
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            Button l = sender as Button;
            if (l != null)
            {
                Tb1.Text += l.Content.ToString();
                inkC.Strokes.Clear();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inkC.Strokes.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Tb1.Text.Length > 0)
            {
                Tb1.Text = Tb1.Text.Substring(0, Tb1.Text.Length - 1);
            }
        }

        /// <summary>  
        /// 识别（将多个笔画集合成一起识别，提高单字的识别率）
        /// </summary>  
        /// <param name="strokes">笔迹集合</param>  
        /// <returns>候选词数组</returns>  
        public string[] Recognize(StrokeCollection strokes)
        {
            if (strokes == null || strokes.Count == 0)
                return null;

            var stroke = GetCombinedStore(strokes);

            var analyzer = new InkAnalyzer();
            analyzer.AddStroke(stroke, 0x0804);
            analyzer.SetStrokeType(stroke, StrokeType.Writing);

            var status = analyzer.Analyze();
            if (status.Successful)
            {
                return analyzer.GetAlternates()
                    .OfType<AnalysisAlternate>()
                    .Select(x => x.RecognizedString)
                    .ToArray();
            }

            analyzer.Dispose();

            return null;
        }

        private static Stroke GetCombinedStore(StrokeCollection strokes)
        {
            var points = new StylusPointCollection();
            foreach (var stroke in strokes)
            {
                points.Add(stroke.StylusPoints);
            }
            return new Stroke(points);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (tempList.Count > 0)
            {
                inkC.Strokes.Remove(tempList.Pop());
            }
        }  
    }
}
