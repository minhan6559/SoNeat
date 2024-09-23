using SplashKitSDK;
using SoNeat.src.Utils;
using SoNeat.src.GameLogic;

namespace SoNeat.src.Screen
{
    public class MainMenuState : IScreenState
    {
        private MyButton? _playBtn;
        private MyButton? _trainBtn;
        private MyButton? _exitBtn;
        private Bitmap? _title;
        private Bitmap? _chooseArrow;
        private EnvironmentManager? _environmentManager;

        public void EnterState()
        {
            _playBtn = new MyButton("assets/images/MainMenu/play.png", 581, 322);
            _trainBtn = new MyButton("assets/images/MainMenu/train.png", 537, 370);
            _exitBtn = new MyButton("assets/images/MainMenu/exit.png", 581, 418);

            _title = SplashKit.LoadBitmap("title", "assets/images/MainMenu/title.png");
            _chooseArrow = SplashKit.LoadBitmap("choose_arrow", "assets/images/MainMenu/choose_arrow.png");

            _environmentManager = new EnvironmentManager(0);
        }

        public void Update()
        {
            _environmentManager!.Update();

            if (_playBtn!.IsClicked())
            {
                GameScreenState gameScreen = new GameScreenState();
                gameScreen.LoadEnvironment(_environmentManager!);
                ScreenManager.Instance.SetState(gameScreen);
            }

            if (_trainBtn!.IsClicked())
            {
                TrainScreenState trainScreen = new TrainScreenState();
                trainScreen.LoadEnvironment(_environmentManager!);
                ScreenManager.Instance.SetState(trainScreen);
            }

            if (_exitBtn!.IsClicked())
            {
                SplashKit.CloseAllWindows();
            }
        }

        public void Draw()
        {
            _environmentManager!.Draw();
            _title!.Draw(415, 161);

            _playBtn!.Draw();
            if (_playBtn.IsHovered())
            {
                _chooseArrow!.Draw(_playBtn.X - 40, _playBtn.Y);
            }

            _trainBtn!.Draw();
            if (_trainBtn.IsHovered())
            {
                _chooseArrow!.Draw(_trainBtn.X - 40, _trainBtn.Y);
            }

            _exitBtn!.Draw();
            if (_exitBtn.IsHovered())
            {
                _chooseArrow!.Draw(_exitBtn.X - 40, _exitBtn.Y);
            }
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Main Menu State");
        }
    }
}