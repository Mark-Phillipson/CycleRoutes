using System;

namespace CycleRoutes.Shared.Components
{
    public static class ApiKeyPromptService
    {
        private static bool _showPrompt = true;
        public static event Action? OnShowPrompt;
        public static bool ShowPrompt => _showPrompt;
        public static void HidePrompt()
        {
            _showPrompt = false;
            OnShowPrompt?.Invoke();
        }
        public static void Show()
        {
            _showPrompt = true;
            OnShowPrompt?.Invoke();
        }
    }
}
