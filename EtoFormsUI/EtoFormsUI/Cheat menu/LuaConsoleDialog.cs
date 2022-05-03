using Civ2engine;
using Civ2engine.Scripting;
using Eto.Drawing;
using Eto.Forms;

namespace EtoFormsUI.Cheat_menu
{
    public class LuaConsoleDialog : Civ2customDialog
    {
        public LuaConsoleDialog(Main parent, bool enableRunCommand) : base(parent, parent.Width / 3, parent.Height / 3, title: "Lua Console")
        {
            var script = Game.Instance.Script;
            DefaultButton = new Civ2button("Run Script", 126, 30, new Font("Times new roman", 11)) { Enabled = false };
            AbortButton = new Civ2button("Close", 126, 30, new Font("Times new roman", 11));
            var commandBox = new TextBox
            {
                Font = new Font("Times new roman", 12),
                Width = Width - 24 - DefaultButton.Width - AbortButton.Width,
                Height = 30
            };
            var buttonRowY = Height - 40;
            Layout.Add(commandBox, 9, buttonRowY + 2);
            Layout.Add(DefaultButton, 11 + commandBox.Width, buttonRowY);
            Layout.Add(AbortButton, 13 + DefaultButton.Width + commandBox.Width, buttonRowY);

            var contentLabel = new Label
            {
                Text = script.Log,
                BackgroundColor = Colors.DarkGray,
            };

            var scrollArea = new Scrollable
            {
                Width = Width - 20,
                Height = Height - 46 - 38,
                Content = contentLabel
            };
            Layout.Add(scrollArea, 11,38);

            commandBox.Enabled = enableRunCommand;
            commandBox.TextChanged += (sender, args) =>
            {
                DefaultButton.Enabled = !string.IsNullOrWhiteSpace(commandBox.Text);
            };

            commandBox.KeyUp += (sender, args) =>
            {
                if (args.Key != Keys.Enter || string.IsNullOrWhiteSpace(commandBox.Text)) return;
                ExecuteImmediate(script, commandBox, contentLabel, scrollArea);
            };

            DefaultButton.Enabled = enableRunCommand;
            DefaultButton.Click += (sender, e) =>
            {
                ExecuteImmediate(script, commandBox, contentLabel, scrollArea);
            };

            AbortButton.Click += (sender, e) =>
            {
                foreach (var item in parent.Menu.Items) item.Enabled = true;
                Close(); 
            };
            
            Content = Layout;
        }

        private static void ExecuteImmediate(ScriptEngine script, TextBox commandBox, Label contentLabel, Scrollable scrollArea)
        {
            script.Execute(commandBox.Text);
            contentLabel.Text = script.Log;
            scrollArea.ScrollPosition = new Point(0, scrollArea.Content.Height);
        }
    }
}