using System.Windows.Controls;
using BoardGameFrontend.Managers;


namespace BoardGameFrontend.ChatLogManager
{
    public class MainChatLogManager
    {
        private ChatLobbyManager _chatListBox {get; set;}
        private ChatGameManager _gameChatManager {get; set;}
        public LobbyChatLogManager LobbyChatLogManager {get; set;}
        public PhaseChatLogManager PhaseChatLogManager {get; set;}
        public MiniPhaseChatLogManager MiniPhaseChatLogManager {get; set;}
        public HeroCardChatLogManager HeroCardChatLogManager {get; set;}
        public MercenaryChatLogManager MercenaryChatLogManager {get; set;}
        public OthersChatLogManager OthersChatLogManager {get; set;}
        public ArtifactChatLogManager ArtifactChatLogManager {get; set;}
        public TileChatLogManager TileChatLogManager {get; set;}
        public RoyalCardChatLogManager RoyalCardChatLogManager {get; set;}

        public MainChatLogManager(ChatLobbyManager chatListBox, ChatGameManager gameChatManager, Game game)
        {
            _chatListBox = chatListBox;
            _gameChatManager = gameChatManager;
            LobbyChatLogManager = new LobbyChatLogManager(chatListBox);
            PhaseChatLogManager = new PhaseChatLogManager(gameChatManager, game);
            MiniPhaseChatLogManager = new MiniPhaseChatLogManager(gameChatManager,game);
            HeroCardChatLogManager = new HeroCardChatLogManager(gameChatManager, game);
            MercenaryChatLogManager = new MercenaryChatLogManager(gameChatManager, game);
            OthersChatLogManager = new OthersChatLogManager(gameChatManager, game);
            ArtifactChatLogManager = new ArtifactChatLogManager(gameChatManager, game);
            TileChatLogManager = new TileChatLogManager(gameChatManager, game);
            RoyalCardChatLogManager = new RoyalCardChatLogManager(gameChatManager, game);
        }
    }
}