using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using POETradeHelper.Common.Contract;

using SharpHook;
using SharpHook.Native;

namespace POETradeHelper.Common
{
    [ExcludeFromCodeCoverage]
    public class UserInputSimulator : IUserInputSimulator
    {
        private readonly IClipboardHelper clipboardHelper;
        private readonly IEventSimulator eventSimulator;

        public UserInputSimulator(IClipboardHelper clipboardHelper, IEventSimulator eventSimulator)
        {
            this.clipboardHelper = clipboardHelper;
            this.eventSimulator = eventSimulator;
        }

        public void SendCopyCommand()
        {
            this.SendKeyWithModifier(KeyCode.VcLeftControl, KeyCode.VcC);
        }

        public async Task SendGotoHideoutCommand()
        {
            const string chatCommand = "/hideout";
            await this.SendChatCommand(chatCommand);
        }

        private async Task SendChatCommand(string chatCommand)
        {
            string clipBoardText = await this.clipboardHelper.GetTextAsync();
            
            await this.clipboardHelper.SetTextAsync(chatCommand);
            this.TypeKey(KeyCode.VcEnter);
            this.SendKeyWithModifier(KeyCode.VcLeftControl, KeyCode.VcA);
            this.Paste();
            this.TypeKey(KeyCode.VcEnter);
            await Task.Delay(200);

            await this.clipboardHelper.SetTextAsync(clipBoardText);
        }

        private void Paste()
        {
            this.SendKeyWithModifier(KeyCode.VcLeftControl, KeyCode.VcV);
        }

        private void SendKeyWithModifier(KeyCode modifier, KeyCode key)
        {
            this.eventSimulator.SimulateKeyPress(modifier);
            this.eventSimulator.SimulateKeyPress(key);
            this.eventSimulator.SimulateKeyRelease(modifier);
            this.eventSimulator.SimulateKeyRelease(key);
        }

        private void TypeKey(KeyCode keyCode)
        {
            this.eventSimulator.SimulateKeyPress(keyCode);
            this.eventSimulator.SimulateKeyRelease(keyCode);
        }
    }
}