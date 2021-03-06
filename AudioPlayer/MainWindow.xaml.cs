﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using AudioPlayer.classes;
using System.Windows.Threading;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private string status = "";
        private List<String> allPaths = new List<String>();
        MediaElement mediaElement;
        Dictionary<Int32, Song> allSongs = new Dictionary<int, Song>();
        private int indexOfPlayingSong = -1;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            mediaElement = new MediaElement();
            mediaElement.UnloadedBehavior = MediaState.Manual;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (mediaElement != null && status == "playing" && mediaElement.NaturalDuration.HasTimeSpan)
            {
                lblFullTimeSong.Content = mediaElement.Position.Minutes.ToString("00") + ":" + mediaElement.Position.Seconds.ToString("00");

                pbProgress.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;

                pbProgress.Value = mediaElement.Position.TotalSeconds;

            }
                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".mp3";
            openFileDialog.Filter = "mp3 (*.mp3)|*.mp3";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = true;

            Nullable<bool> result = openFileDialog.ShowDialog();

            String[] paths = {};

            if (result == true)
            {
                if (lvSongs.Items.Count > 0)
                {
                    lvSongs.Items.Clear();
                    allSongs.Clear();
                }
                paths = openFileDialog.FileNames;
            }

            int i = 1;
            Song song;
            foreach (String item in paths)
            {

                allPaths.Add(item);

                var file = TagLib.File.Create(item);
                string duration = file.Properties.Duration.ToString();
                DateTime dt = Convert.ToDateTime(duration);

                song = new Song() { Id = i.ToString(), Name = System.IO.Path.GetFileName(item.ToString()), Duration = dt.ToString("mm:ss"), Path = item };
                allSongs.Add(i, song);

                lvSongs.Items.Add(song);
                i++;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            if (status == "change" && mediaElement != null)
            {
                mediaElement.Stop();
                status = "stoped";
            }

            if (status == "playing")
            {
                mediaElement.Pause();
                status = "paused";

                this.btnPlay.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/images/glyphs/play.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                return;
            }

            if (status == "paused")
            {
                mediaElement.Play();
                status = "playing";
                this.btnPlay.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/images/glyphs/pause.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                return;
            }

            if (lvSongs.SelectedItems.Count > 0)
            {
                Song song = (Song)lvSongs.SelectedItems[0];
                string path = song.Path;

                mediaElement.Volume = sbVolume.Value;
                mediaElement.Source = new Uri(path);
                
                mediaElement.Play();

                indexOfPlayingSong = Int32.Parse(song.Id);

                lblFullTimeSong.Content = "00:00";
                lblTimeSong.Content = song.Duration;
                lblPlayedSong.Content = song.Name;
                status = "playing";
                this.btnPlay.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("/images/glyphs/pause.png", UriKind.Relative)),
                        VerticalAlignment = VerticalAlignment.Center
                    };
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (mediaElement != null)
            {
                mediaElement.Stop();

                lblFullTimeSong.Content = "00:00";
                lblTimeSong.Content = "00:00";
                lblPlayedSong.Content = "";

                status = "stoped";
                this.btnPlay.Content = new Image
                {
                    Source = new BitmapImage(new Uri("/images/glyphs/play.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }

        private void lvSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            status = "change";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            indexOfPlayingSong = indexOfPlayingSong < allPaths.Count ? indexOfPlayingSong++ : 0;

            mediaElement.Stop();
            lvSongs.SelectedIndex = indexOfPlayingSong;
            Button_Click_3(sender, e);

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            indexOfPlayingSong = indexOfPlayingSong > 0 ? --indexOfPlayingSong - 1 : allPaths.Count - 1;
            
            mediaElement.Stop();
            lvSongs.SelectedIndex = indexOfPlayingSong;
            Button_Click_3(sender, e);

        }

        private void lvSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Double click on empty space in ListView
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != lvSongs)
            {
                if (obj.GetType() == typeof(ListViewItem))
                {
                    Button_Click_3(sender, e);
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
            {
                mediaElement.Volume = sbVolume.Value;
            }
        }
    }
}
