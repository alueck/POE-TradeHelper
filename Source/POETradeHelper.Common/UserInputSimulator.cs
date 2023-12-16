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

        public void SendCopyAdvancedItemStringCommand() => this.SendKeyWithModifiers(KeyCode.VcC, KeyCode.VcLeftControl,  KeyCode.VcLeftAlt);

        public async Task SendGotoHideoutCommand()
        {
            const string chatCommand = "/hideout";
            await this.SendChatCommand(chatCommand);
        }

        private async Task SendChatCommand(string chatCommand)
        {
            string? clipBoardText = await this.clipboardHelper.GetTextAsync();

            await this.clipboardHelper.SetTextAsync(chatCommand);
            this.TypeKey(KeyCode.VcEnter);
            this.SendKeyWithModifiers(KeyCode.VcA, KeyCode.VcLeftControl);
            this.Paste();
            this.TypeKey(KeyCode.VcEnter);
            await Task.Delay(200);

            await this.clipboardHelper.SetTextAsync(clipBoardText);
        }

        private void Paste() => this.SendKeyWithModifiers(KeyCode.VcV, KeyCode.VcLeftControl);

        private void SendKeyWithModifiers(KeyCode key, params KeyCode[] modifiers)
        {
            this.SimulateKeyPress(modifiers);
            this.SimulateKeyPress(key);
            this.SimulateKeyRelease(modifiers);
            this.SimulateKeyRelease(key);
        }

        private void TypeKey(KeyCode keyCode)
        {
            this.SimulateKeyPress(keyCode);
            this.SimulateKeyRelease(keyCode);
        }

        private void SimulateKeyPress(params KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                this.eventSimulator.SimulateKeyPress(key);
            }
        }

        private void SimulateKeyRelease(params KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                this.eventSimulator.SimulateKeyRelease(key);
            }
        }
    }
}