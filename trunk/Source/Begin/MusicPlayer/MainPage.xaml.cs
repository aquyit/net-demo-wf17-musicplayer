// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Shell;
using Models;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;

namespace MusicPlayer
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Properties and Fields
        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
        }

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            UpdateAppBarStatus();
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            int activeTrackNumber = GetActiveTrackIndex();

            if (activeTrackNumber != -1)
            {
                lstTracks.SelectedIndex = activeTrackNumber;
            }
        }

        private int GetActiveTrackIndex()
        {
            int track = -1;
            if (null != BackgroundAudioPlayer.Instance.Track)
            {
                track = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag);
            }

            return track;
        }

        public Playlist ActivePlaylist
        {
            get { return (Playlist)GetValue(ActivePlaylistProperty); }
            set { SetValue(ActivePlaylistProperty, value); }
        }

        public static readonly DependencyProperty ActivePlaylistProperty =
        DependencyProperty.Register("ActivePlaylist", typeof(Playlist), typeof(MainPage), new PropertyMetadata(null));

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Stream playlistStream = Application.GetResourceStream(new Uri("Misc/Playlist.xml", UriKind.Relative)).Stream;

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Playlist));
            ActivePlaylist = (Playlist)serializer.Deserialize(playlistStream);

            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream file = isoStorage.OpenFile("playlist.xml", FileMode.OpenOrCreate))
                {
                    var writer = new StreamWriter(file);

                    serializer.Serialize(writer, ActivePlaylist);
                }
            }

            base.OnNavigatedTo(e);
        }


        private void UpdateAppBarStatus()
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    //Prev Button
                    if (GetActiveTrackIndex() > 0)
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

                    //Play/Pause Button
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = "pause";
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IconUri = new Uri("/Images/pause.png", UriKind.Relative);

                    //Stop Button
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;

                    //Next button
                    if (GetActiveTrackIndex() < ActivePlaylist.Tracks.Count - 1)
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                    break;

                case PlayState.Paused:
                    //Prev Button
                    if (GetActiveTrackIndex() > 0)
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

                    //Play/Pause Button
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = "play";
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IconUri = new Uri("/Images/play.png", UriKind.Relative);

                    //Stop Button
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;

                    //Next button
                    if (GetActiveTrackIndex() < ActivePlaylist.Tracks.Count - 1)
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                    break;

                case PlayState.Stopped:
                    //Prev Button
                    if (GetActiveTrackIndex() > 0)
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

                    //Play/Pause Button
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = "play";
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IconUri = new Uri("/Images/play.png", UriKind.Relative);

                    //Stop Button
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;

                    //Next button
                    if (GetActiveTrackIndex() < ActivePlaylist.Tracks.Count - 1)
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                    break;

                case PlayState.Unknown:
                    //Prev Button
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

                    //Play/Pause Button
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = "play";
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IconUri = new Uri("/Images/play.png", UriKind.Relative);

                    //Stop Button
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;

                    //Next button
                    if (GetActiveTrackIndex() < ActivePlaylist.Tracks.Count - 1)
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                    else
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                    break;

                default:
                    break;
            }
        }

        #region Events Handlers

        private void lstTracks_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSelection();
        }

        #endregion

        #region ApplicationBar Buttons Events

        private void appbar_prev(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipPrevious();
        }

        private void appbar_playpause(object sender, EventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
                BackgroundAudioPlayer.Instance.Pause();
            else
                BackgroundAudioPlayer.Instance.Play();
        }

        private void appbar_stop(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.Stop();
        }

        private void appbar_next(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipNext();
        }
        #endregion
    }
}