using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using BoardGameFrontend.Helpers;

public class MusicManager
{
    private MediaPlayer _mediaPlayer;
    private static readonly string MusicFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
    private readonly Random _random = new Random();

    public MusicManager()
    {
        _mediaPlayer = new MediaPlayer();

        AudioSettings.Initialize(AllMusicsManager.GetAvailableTracks());
        AudioSettings.VolumeChanged += OnVolumeChanged;
        AudioSettings.MusicStatusChanged += OnMusicStatusChanged;
        AudioSettings.SelectedTrackChanged += OnSelectedTrackChanged;
        AudioSettings.ShuffleStatusChanged += OnShuffleStatusChanged;

        LoadTrack(AudioSettings.SelectedTrack); 
        if(AudioSettings.IsMusicEnabled){
            PlayMusic();
        }
        
    }

    private void LoadTrack(string trackName)
    {
        string musicFilePath = GetTrackPath(trackName);

        if (File.Exists(musicFilePath))
        {
            _mediaPlayer.Open(new Uri(musicFilePath, UriKind.Absolute));
        }
        else
        {
            throw new FileNotFoundException("Music file not found.", musicFilePath);
        }
    }

    private string GetRandomTrack()
    {
        var tracks = AudioSettings.AvailableTracks;
        string track; 
        while(true){
            track = tracks[_random.Next(tracks.Count)];
            if(track != AudioSettings.SelectedTrack || tracks.Count < 2) break;
        }
        
        return track;
    }

    public static string GetTrackPath(string trackName)
    {
        return Path.Combine(MusicFolder, $"{trackName}.mp3");
    }

    private void OnVolumeChanged(double newVolume)
    {
        _mediaPlayer.Volume = newVolume;
    }

    private void OnMusicStatusChanged(bool isEnabled)
    {
        if (isEnabled)
        {
            _mediaPlayer.Play();
        }
        else
        {
            _mediaPlayer.Pause();
        }
    }

    private void OnSelectedTrackChanged(string newTrack)
    {
        LoadTrack(newTrack); // Load the new track when it changes
        PlayMusic(); // Start playing the new track
    }

    public void PlayMusic()
    {
        _mediaPlayer.Stop();
        _mediaPlayer.MediaEnded += (sender, e) =>
        {
            _mediaPlayer.Position = TimeSpan.Zero;
            if (AudioSettings.ShuffleEnabled)
            {
                AudioSettings.SelectedTrack = GetRandomTrack(); 
            }else{
                 _mediaPlayer.Play();
            }

             
        };

        _mediaPlayer.Play();
    }

    public void StopMusic()
    {
        _mediaPlayer.Stop();
    }

    private void OnShuffleStatusChanged(bool shuffleEnabled)
    {
        if (shuffleEnabled)
        {
            AudioSettings.SelectedTrack = GetRandomTrack();
            LoadTrack(AudioSettings.SelectedTrack);
            PlayMusic();
        }
    }

    public void ClearMediaPlayer(){
        _mediaPlayer.Stop();
        _mediaPlayer.Close();
    }

    public void PauseMusic()
    {
        _mediaPlayer.Pause();
    }

    public void ResumeMusic()
    {
        _mediaPlayer.Play();
    }
}
