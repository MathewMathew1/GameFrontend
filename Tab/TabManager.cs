using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;

public class TabManager : INotifyPropertyChanged
{
    private Tab _selectedTab;
    private Game _game;
    private bool _switchTabs;
    private TabControl? _tabControl;

    private Dictionary<Tab, int> _tabIndexMapping;

    public Tab SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab != value)
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }
    }

    public void SetupTab(TabControl tabControl){
        _tabControl = tabControl;
    }

    public TabManager(Game game, bool switchTabs){
        _game = game;
        _switchTabs = switchTabs;

        _tabIndexMapping = new Dictionary<Tab, int>
        {
            { Tab.Artifacts, 0 },
            { Tab.HeroCards, 1 },
            { Tab.Board, 2 },
            { Tab.Mercenaries, 3 },
            { Tab.YourCards, 4 },
            { Tab.RoyalCard, 5 }
        };
    }

    public void ChangeSwitchTabs(bool switchTabs){
        _switchTabs = switchTabs;
    }

    public void SetTab(Tab tabName){
        SelectedTab = tabName;

        if(_game.IsUserControlledPlayersTurn && _switchTabs){
            if (_tabIndexMapping.ContainsKey(tabName) && _tabControl != null)
            {
                _tabControl.SelectedIndex = _tabIndexMapping[tabName];
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}