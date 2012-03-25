using System;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using Models;

namespace AudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        /// 
        static int currentTrackNumber= 0;
        static Playlist playlist;

        public AudioPlayer()
        : base()
        {

            //Load fromIsoStore&deserialize
            using ( IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication() )
            {
                using ( IsolatedStorageFileStream file = isoStorage.OpenFile("playlist.xml", FileMode.Open) )
                {
                    XmlSerializer serializer= new XmlSerializer(typeof(Playlist));
                    var reader = new StreamReader(file);

                    playlist = (Playlist)serializer.Deserialize(reader);
                }
            }
        }


        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch(playState)
            {
                case PlayState.TrackEnded:
                    PlayNext(player);
                    break;
                case PlayState.TrackReady:
                    player.Play();
                    break;
                default:
                    break;
            }

            NotifyComplete();
        }

        private void PlayNext(BackgroundAudioPlayer player)
        {
            var songsCount=playlist.Tracks.Count;

            if( ++currentTrackNumber>=songsCount)
            {
                currentTrackNumber= 0;
            }
            Play(player);
        }

        private void PlayPrev(BackgroundAudioPlayer player)
        {
            var songsCount=playlist.Tracks.Count;
            if( --currentTrackNumber< 0 )
            {
                currentTrackNumber=songsCount- 1;
            }
            Play(player);
        }

        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch( action )
            {
                case UserAction.FastForward:
                    player.FastForward();
                    break;
                case UserAction.Pause:
                    player.Pause();
                    break;
                case UserAction.Play:
                    if(player.PlayerState==PlayState.Paused)
                    {
                        player.Play();
                    }
                    else
                    {
                        Play(player);
                    }
                    break;
                case UserAction.Rewind:
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position= (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    PlayNext(player);
                    break;
                case UserAction.SkipPrevious:
                    PlayPrev(player);
                    break;
                case UserAction.Stop:
                    player.Stop();
                    break;
                default:
                    break;
            }

            NotifyComplete();
        }

        private void Play(BackgroundAudioPlayer player)
        {
            var currentTrack=playlist.Tracks[currentTrackNumber];
            Uri tileUri = ( currentTrack.Tile ==null?new Uri("Shared/Media/no-art.jpg",UriKind.Relative) :
            ( currentTrack.Tile.IsAbsoluteUri ?new Uri("Shared/Media/no-art.jpg",UriKind.Relative) :
            new Uri(currentTrack.TileString.Replace("/Images","Shared/Media"),UriKind.Relative) ) );

            var audioTrack=new AudioTrack(currentTrack.Source,
            currentTrack.Title,
            currentTrack.Artist,
            currentTrack.Album,
            tileUri,
            currentTrackNumber.ToString(),
            EnabledPlayerControls.All);
        player.Track=audioTrack;
        }

        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private AudioTrack GetNextTrack()
        {
            // TODO: add logic to get the next audio track

            AudioTrack track = null;

            // specify the track

            return track;
        }


        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private AudioTrack GetPreviousTrack()
        {
            // TODO: add logic to get the previous audio track

            AudioTrack track = null;

            // specify the track

            return track;
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnCancel()
        {
            NotifyComplete();
        }

        protected override void OnError(BackgroundAudioPlayer player,AudioTrack track,Exception error,bool isFatal)
        {
            NotifyComplete();
        }
    }
}
