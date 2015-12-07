#pragma once

namespace CliboardQQSender
{
	HWND g_QQHwnd;

	void pushStringToClipboard(string source);

	void sendClipboardMessage();
}