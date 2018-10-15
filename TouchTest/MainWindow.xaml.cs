using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NAudio;

namespace TouchTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //To do: Make it so that when nothing is touching the screen all midi notes are off.
    public partial class MainWindow : Window
    {
        const Int32 radius = 50;
        Dictionary<Ellipse, Int32> TouchPoints;
        NAudio.Midi.MidiOut outer = new NAudio.Midi.MidiOut(1);
       
        //outer.Send(NAudio.Midi.MidiMessage.StartNote(60, 100, 1).RawData); Play Middle C

        public MainWindow()
        {
            TouchPoints = new Dictionary<Ellipse, Int32>();
            
            for (int i = 0; i < 10; i++)
            {
                TouchPoints[new Ellipse()
                {
                    Height = radius,
                    Width = radius,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.LightBlue,
                    RenderTransform = new TranslateTransform(-radius / 2, -radius / 2)
                }] = -1;
            }

            InitializeComponent();

            /*StylusDown += MainWindow_StylusDown;
            StylusUp += MainWindow_StylusUp;
            StylusMove += MainWindow_StylusMove;*/

            KeyDown += MainWindow_KeyDown;

            createPiano();

            
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        /*private void MainWindow_StylusDown(object sender, StylusEventArgs e)
        {
            if (canvas1 != null)
            {
                var device = e.StylusDevice;

                device.Capture(canvas1);

                if (!TouchPoints.ContainsValue(device.Id))
                {
                    var temp = e.GetStylusPoints(canvas1).Last(); //Touchpoints
                    var ellipse = TouchPoints.FirstOrDefault(iE => iE.Value < 0);
                    if (ellipse.Key != null)
                    {
                        canvas1.Children.Add(ellipse.Key);
                        TouchPoints[ellipse.Key] = device.Id;
                    }
                }
            }
        }

        private void MainWindow_StylusMove(object sender, StylusEventArgs e)
        {
            if (canvas1 != null)
            {
                var device = e.StylusDevice;
                var temp = e.GetStylusPoints(canvas1).Last();
                var tp = new Point(temp.X, temp.Y);
                if (TouchPoints.ContainsValue(device.Id))
                {
                    var ellipse = TouchPoints.First(iE => iE.Value == device.Id);
                    Canvas.SetLeft(ellipse.Key, tp.X);
                    Canvas.SetTop(ellipse.Key, tp.Y);
                }
            }
        }

        private void MainWindow_StylusUp(object sender, StylusEventArgs e)
        {
            var device = e.StylusDevice;

            if (canvas1 != null && device.Captured == canvas1)
            {
                if (TouchPoints.ContainsValue(device.Id))
                {
                    var ellipse = TouchPoints.First(iE => iE.Value == device.Id);
                    canvas1.Children.Remove(ellipse.Key);
                    TouchPoints[ellipse.Key] = -1;
                }
            }
        }*/

        private void createPiano()
        {
            double screenWidth = SystemParameters.FullPrimaryScreenWidth;
            double screenHeight = SystemParameters.FullPrimaryScreenHeight;

            int[] whiteKeyLayout = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84};
            int[] blackKeyLayout = new int[] { 61, 63, 66, 68, 70, 73, 75, 78, 80, 82 };

            int count = 0;

            #region White Key Drawing
            for (double i = 0; i < screenWidth; i += screenWidth / 15)
            {
                Canvas note = new Canvas();
                note.Background = new SolidColorBrush(Colors.White);
                note.Width = screenWidth / 15;
                note.Height = screenHeight;

                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1, 1, 1, 1);

                border.Child = note;

                canvas1.Children.Add(border);

                Canvas.SetLeft(border, (screenWidth / 15) * count);
                border.Tag = whiteKeyLayout[count];
                note.Tag = whiteKeyLayout[count];

                #region Key Touch Events
                border.TouchDown += (s, ee) =>
                {
                    Border key = s as Border;
                    key.Background = new SolidColorBrush(Colors.LightGray);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 100, 1).RawData);
                };
                border.TouchUp += (s, ee) =>
                {
                    Border key = s as Border;
                    key.Background = new SolidColorBrush(Colors.White);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };
                border.TouchLeave += (s, ee) =>
                {
                    Border key = s as Border;
                    key.Background = new SolidColorBrush(Colors.White);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };

                note.TouchDown += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.LightGray);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 100, 1).RawData);
                };
                note.TouchUp += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.White);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };
                note.TouchLeave += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.White);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };
                #endregion
                count++;
            }
            #endregion

            count = 0;

            #region Black Key Drawing
            for(double i = 0; i < 10; i++)
            {
                Canvas note = new Canvas();
                note.Background = new SolidColorBrush(Colors.Black);
                note.Width = (screenWidth / 15) * .7;
                note.Height = screenHeight * .65;
                canvas1.Children.Add(note);

               switch(i)
                {
                    case 0:
                        Canvas.SetLeft(note, screenWidth / 15 - (screenWidth / 40));
                        break;
                    case 1:
                        Canvas.SetLeft(note, ((screenWidth*2) / 15) - (screenWidth / 40));
                        break;
                    case 2:
                        Canvas.SetLeft(note, ((screenWidth * 4) / 15) - (screenWidth / 40));
                        break;
                    case 3:
                        Canvas.SetLeft(note, ((screenWidth * 5) / 15) - (screenWidth / 40));
                        break;
                    case 4:
                        Canvas.SetLeft(note, ((screenWidth * 6) / 15) - (screenWidth / 40));
                        break;
                    case 5:
                        Canvas.SetLeft(note, ((screenWidth * 8) / 15) - (screenWidth / 40));
                        break;
                    case 6:
                        Canvas.SetLeft(note, ((screenWidth * 9) / 15) - (screenWidth / 40));
                        break;
                    case 7:
                        Canvas.SetLeft(note, ((screenWidth * 11) / 15) - (screenWidth / 40));
                        break;
                    case 8:
                        Canvas.SetLeft(note, ((screenWidth * 12) / 15) - (screenWidth / 40));
                        break;
                    case 9:
                        Canvas.SetLeft(note, ((screenWidth * 13) / 15) - (screenWidth / 40));
                        break;
                }

                note.Tag = blackKeyLayout[count];

                #region Key Touch Events
                note.TouchDown += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.DarkGray);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 100, 1).RawData);
                    //PitchWheelChange(127);

                };
                note.TouchUp += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.Black);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };
                note.TouchLeave += (s, ee) =>
                {
                    Canvas key = s as Canvas;
                    key.Background = new SolidColorBrush(Colors.Black);
                    outer.Send(NAudio.Midi.MidiMessage.StartNote((int)key.Tag, 0, 1).RawData);
                };
                #endregion
                count++;
            }
            #endregion
        }

        /*private void PitchWheelChange(int value)
        {
            int change = 0x2000 + value;  //  0x2000 == No Change
            char low = (char)(change & 0x7F);  // Low 7 bits
            char high = (char)((change >> 7) & 0x7F);  // High 7 bits

            for(int i = 0; i < 128; i++)
                outer.Send(NAudio.Midi.MidiMessage.ChangeControl(i, 100, 1).RawData);
        }*/
    }
}
