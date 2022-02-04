using Eto.Drawing;
using Eto.Forms;

namespace EtoFormsUI
{
    public class LuaConsoleDialog : Civ2customDialog
    {
        public LuaConsoleDialog(Main parent) : base(parent, parent.Width / 3, parent.Height / 3, title: "Lua Console")
        {
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

            commandBox.TextChanged += (sender, args) =>
            {
                DefaultButton.Enabled = !string.IsNullOrWhiteSpace(commandBox.Text);
            };
            
            DefaultButton.Click += (sender, e) =>
            {
          
            };

            AbortButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                Close();
            };
            
            Content = Layout;
        }
    }
}